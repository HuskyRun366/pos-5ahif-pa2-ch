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
    public partial class MainWindow : Window
    {
        private readonly XmlFieldLoader _fieldLoader = new();
        private readonly Lexer _lexer = new();
        private readonly Parser _parser = new();

        private List<RobotElement> _originalElements = new();
        private CancellationTokenSource? _cancellationTokenSource;
        private bool _isRunning = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadField_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "XML Dateien (*.xml)|*.xml|Alle Dateien (*.*)|*.*",
                Title = "Feld laden"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _fieldLoader.Load(dialog.FileName);
                    robotMap.SetSize(_fieldLoader.Width, _fieldLoader.Height);
                    robotMap.SetElements(_fieldLoader.Elements);
                    SaveOriginalState();

                    txtStatus.Text = $"Feld geladen: {_fieldLoader.Width}x{_fieldLoader.Height}";
                    txtErrors.Text = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Laden: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadProgram_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Text Dateien (*.txt)|*.txt|Alle Dateien (*.*)|*.*",
                Title = "Programm laden"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
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

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            if (_isRunning) return;

            txtErrors.Text = "";

            // Parse program
            var tokens = _lexer.Tokenize(txtProgram.Text);
            var program = _parser.Parse(tokens);

            if (program == null)
            {
                txtErrors.Text = string.Join("\n", _parser.Errors);
                txtStatus.Text = "Syntaxfehler gefunden";
                return;
            }

            // Reset to original state before running
            ResetField();

            // Find robot position
            var robot = _fieldLoader.Elements.FirstOrDefault(el => el.Type == ElementType.Robot);
            if (robot == null)
            {
                txtErrors.Text = "Kein Roboter im Feld gefunden!";
                return;
            }

            // Create execution context
            var context = new RobotProgrammingComplete.Models.ExecutionContext
            {
                RobotX = robot.X,
                RobotY = robot.Y,
                FieldWidth = _fieldLoader.Width,
                FieldHeight = _fieldLoader.Height,
                Elements = _fieldLoader.Elements,
                DelayMs = 1000
            };
            context.OnUpdate = () => Dispatcher.Invoke(() =>
            {
                robotMap.Redraw();
                txtCollected.Text = $"Gesammelt: {string.Join(", ", context.CollectedLetters)}";
            });

            _isRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();

            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;
            txtStatus.Text = "Programm läuft...";

            try
            {
                await program.RunAsync(context);
                txtStatus.Text = "Programm beendet";
                txtCollected.Text = $"Gesammelt: {string.Join(", ", context.CollectedLetters)}";
            }
            catch (TaskCanceledException)
            {
                txtStatus.Text = "Programm abgebrochen";
            }
            catch (Exception ex)
            {
                txtErrors.Text = $"Laufzeitfehler: {ex.Message}";
                txtStatus.Text = "Fehler bei Ausführung";
            }
            finally
            {
                _isRunning = false;
                btnStart.IsEnabled = true;
                btnStop.IsEnabled = false;
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetField();
            txtCollected.Text = "Gesammelt: -";
            txtStatus.Text = "Feld zurückgesetzt";
        }

        private void SaveOriginalState()
        {
            _originalElements = _fieldLoader.Elements.Select(el => new RobotElement
            {
                X = el.X,
                Y = el.Y,
                Letter = el.Letter,
                Type = el.Type
            }).ToList();
        }

        private void ResetField()
        {
            _fieldLoader.Elements.Clear();
            foreach (var el in _originalElements)
            {
                _fieldLoader.Elements.Add(new RobotElement
                {
                    X = el.X,
                    Y = el.Y,
                    Letter = el.Letter,
                    Type = el.Type
                });
            }
            robotMap.SetElements(_fieldLoader.Elements);
        }
    }
}
