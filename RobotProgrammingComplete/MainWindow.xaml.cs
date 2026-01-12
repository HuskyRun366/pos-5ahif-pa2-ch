using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using RobotProgrammingComplete.Interpreter;
using RobotProgrammingComplete.Interpreter.Expressions;
using RobotProgrammingComplete.Models;
using RobotProgrammingComplete.Services;

namespace RobotProgrammingComplete
{
    /// <summary>
    /// Hauptfenster der Roboter-Programmieranwendung.
    /// Enthält die UI-Logik für Laden, Starten und Stoppen von Programmen.
    /// </summary>
    public partial class MainWindow : Window
    {
        // Service zum Laden von Spielfeld-XML-Dateien
        private readonly XmlFieldLoader fieldLoader = new();

        // Lexer für die lexikalische Analyse (Tokenisierung)
        private readonly Lexer lexer = new();

        // Parser für die Syntaxanalyse (AST-Erstellung)
        private readonly Parser parser = new();

        // Kopie des Originalzustands für Reset-Funktion
        private List<RobotElement> originalElements = new();

        // Token-Quelle zum Abbrechen der Programmausführung
        private CancellationTokenSource? cancellationTokenSource;

        // Flag ob gerade ein Programm ausgeführt wird
        private bool isRunning = false;

        /// <summary>
        /// Konstruktor: Initialisiert das Hauptfenster.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event-Handler: Lädt ein Spielfeld aus einer XML-Datei.
        /// </summary>
        private void LoadField_Click(object sender, RoutedEventArgs e)
        {
            // Datei-Dialog für XML-Dateien öffnen
            var dialog = new OpenFileDialog
            {
                Filter = "XML Dateien (*.xml)|*.xml|Alle Dateien (*.*)|*.*",
                Title = "Feld laden"
            };

            // Nur fortfahren wenn Benutzer eine Datei ausgewählt hat
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    // XML-Datei laden und parsen
                    fieldLoader.Load(dialog.FileName);

                    // Spielfeld-Control aktualisieren
                    robotMap.SetSize(fieldLoader.Width, fieldLoader.Height);
                    robotMap.SetElements(fieldLoader.Elements);

                    // Originalzustand für Reset speichern
                    SaveOriginalState();

                    // Status-Anzeige aktualisieren
                    txtStatus.Text = $"Feld geladen: {fieldLoader.Width}x{fieldLoader.Height}";
                    txtErrors.Text = "";
                }
                catch (Exception ex)
                {
                    // Fehlermeldung bei Ladeproblemen anzeigen
                    MessageBox.Show($"Fehler beim Laden: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Event-Handler: Lädt ein Roboter-Programm aus einer Textdatei.
        /// </summary>
        private void LoadProgram_Click(object sender, RoutedEventArgs e)
        {
            // Datei-Dialog für Textdateien öffnen
            var dialog = new OpenFileDialog
            {
                Filter = "Text Dateien (*.txt)|*.txt|Alle Dateien (*.*)|*.*",
                Title = "Programm laden"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    // Programmtext in die TextBox laden
                    txtProgram.Text = File.ReadAllText(dialog.FileName);
                    txtStatus.Text = "Programm geladen";
                    txtErrors.Text = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Laden: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Event-Handler: Beendet die Anwendung.
        /// </summary>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Event-Handler: Startet die Programmausführung.
        /// Diese Methode ist async, damit die UI während der Ausführung reagiert.
        /// </summary>
        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            // Verhindern dass mehrere Programme gleichzeitig laufen
            if (isRunning) return;

            // Fehleranzeige zurücksetzen
            txtErrors.Text = "";

            // === Schritt 1: Programm parsen ===
            // Zuerst den Quelltext in Tokens zerlegen (Lexer)
            var tokens = lexer.Tokenize(txtProgram.Text);

            // Dann aus den Tokens einen AST erstellen (Parser)
            var program = parser.Parse(tokens);

            // Bei Syntaxfehlern abbrechen
            if (program == null)
            {
                txtErrors.Text = string.Join("\n", parser.Errors);
                txtStatus.Text = "Syntaxfehler gefunden";
                return;
            }

            // === Schritt 2: Spielfeld zurücksetzen ===
            ResetField();

            // === Schritt 3: Roboter-Position finden ===
            var robot = fieldLoader.Elements.FirstOrDefault(el => el.Type == ElementType.Robot);
            if (robot == null)
            {
                txtErrors.Text = "Kein Roboter im Feld gefunden!";
                return;
            }

            // === Schritt 4: Ausführungskontext erstellen ===
            // Der Kontext enthält alle Informationen die das Programm braucht
            var context = new RobotProgrammingComplete.Models.ExecutionContext
            {
                RobotX = robot.X,
                RobotY = robot.Y,
                FieldWidth = fieldLoader.Width,
                FieldHeight = fieldLoader.Height,
                Elements = fieldLoader.Elements,
                DelayMs = 1000  // 1 Sekunde Pause zwischen Schritten
            };

            // Callback für UI-Updates nach jedem Schritt
            // Dispatcher.Invoke ist nötig weil wir von einem anderen Thread kommen
            context.OnUpdate = () => Dispatcher.Invoke(() =>
            {
                robotMap.Redraw();  // Spielfeld neu zeichnen
                txtCollected.Text = $"Gesammelt: {string.Join(", ", context.CollectedLetters)}";
            });

            // === Schritt 5: UI für laufendes Programm anpassen ===
            isRunning = true;
            cancellationTokenSource = new CancellationTokenSource();
            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;
            txtStatus.Text = "Programm läuft...";

            // === Schritt 6: Programm ausführen ===
            try
            {
                // Programm asynchron ausführen
                await program.RunAsync(context);

                // Erfolgreich beendet
                txtStatus.Text = "Programm beendet";
                txtCollected.Text = $"Gesammelt: {string.Join(", ", context.CollectedLetters)}";
            }
            catch (TaskCanceledException)
            {
                // Benutzer hat abgebrochen
                txtStatus.Text = "Programm abgebrochen";
            }
            catch (Exception ex)
            {
                // Laufzeitfehler anzeigen
                txtErrors.Text = $"Laufzeitfehler: {ex.Message}";
                txtStatus.Text = "Fehler bei Ausführung";
            }
            finally
            {
                // UI zurücksetzen egal wie das Programm endete
                isRunning = false;
                btnStart.IsEnabled = true;
                btnStop.IsEnabled = false;
            }
        }

        /// <summary>
        /// Event-Handler: Stoppt die laufende Programmausführung.
        /// </summary>
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            // Abbruch-Signal senden
            cancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// Event-Handler: Setzt das Spielfeld auf den Originalzustand zurück.
        /// </summary>
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetField();
            txtCollected.Text = "Gesammelt: -";
            txtStatus.Text = "Feld zurückgesetzt";
        }

        /// <summary>
        /// Speichert den aktuellen Spielfeldzustand als Kopie.
        /// Wird für die Reset-Funktion benötigt.
        /// </summary>
        private void SaveOriginalState()
        {
            // Erstelle tiefe Kopie aller Elemente
            // (nicht nur Referenzen, sondern neue Objekte)
            originalElements = fieldLoader.Elements.Select(el => new RobotElement
            {
                X = el.X,
                Y = el.Y,
                Letter = el.Letter,
                Type = el.Type
            }).ToList();
        }

        /// <summary>
        /// Stellt den Originalzustand des Spielfelds wieder her.
        /// </summary>
        private void ResetField()
        {
            // Aktuelle Elemente löschen
            fieldLoader.Elements.Clear();

            // Originale Elemente als Kopien wiederherstellen
            foreach (var el in originalElements)
            {
                fieldLoader.Elements.Add(new RobotElement
                {
                    X = el.X,
                    Y = el.Y,
                    Letter = el.Letter,
                    Type = el.Type
                });
            }

            // Spielfeld-Control aktualisieren
            robotMap.SetElements(fieldLoader.Elements);
        }
    }
}
