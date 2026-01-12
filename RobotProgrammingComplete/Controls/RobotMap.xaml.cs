using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RobotProgrammingComplete.Models;

namespace RobotProgrammingComplete.Controls
{
    /// <summary>
    /// Benutzerdefiniertes Control zur Darstellung des Roboter-Spielfelds.
    /// Zeichnet ein Gitter mit Wänden, Hindernissen, Buchstaben und dem Roboter.
    /// </summary>
    public partial class RobotMap : UserControl
    {
        // Breite des Spielfelds in Zellen
        private int width = 10;

        // Höhe des Spielfelds in Zellen
        private int height = 10;

        // Liste aller Elemente auf dem Spielfeld
        private List<RobotElement> elements = new();

        // Vorgeladene Bilder für die verschiedenen Element-Typen
        private readonly BitmapImage wallImage;    // Wand-Textur
        private readonly BitmapImage robotImage;   // Roboter-Bild
        private readonly BitmapImage obstacleImage; // Hindernis-Bild

        /// <summary>
        /// Konstruktor: Initialisiert das Control und lädt die Bilder.
        /// </summary>
        public RobotMap()
        {
            InitializeComponent();

            // Lade die Bilder aus den eingebetteten Ressourcen
            // pack://application:,,, ist die WPF-Syntax für eingebettete Ressourcen
            wallImage = new BitmapImage(new Uri("pack://application:,,,/Resources/wall.jpg"));
            robotImage = new BitmapImage(new Uri("pack://application:,,,/Resources/robot.png"));
            obstacleImage = new BitmapImage(new Uri("pack://application:,,,/Resources/stone.png"));
        }

        /// <summary>
        /// Setzt die Größe des Spielfelds und zeichnet neu.
        /// </summary>
        /// <param name="x">Breite in Zellen</param>
        /// <param name="y">Höhe in Zellen</param>
        public void SetSize(int x, int y)
        {
            width = x;
            height = y;
            Redraw();  // Spielfeld neu zeichnen
        }

        /// <summary>
        /// Setzt die Liste der Spielfeld-Elemente und zeichnet neu.
        /// </summary>
        /// <param name="list">Die neuen Elemente</param>
        public void SetElements(List<RobotElement> list)
        {
            elements = list;
            Redraw();  // Spielfeld neu zeichnen
        }

        /// <summary>
        /// Setzt das Spielfeld auf die Standardgröße zurück.
        /// </summary>
        public void ClearLevel()
        {
            SetSize(5, 5);
            SetElements(new List<RobotElement>());
        }

        /// <summary>
        /// Event-Handler: Wird aufgerufen wenn sich die Control-Größe ändert.
        /// </summary>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Redraw();  // Bei Größenänderung neu zeichnen
        }

        /// <summary>
        /// Zeichnet das gesamte Spielfeld neu.
        /// Dies ist die Hauptmethode für die Visualisierung.
        /// </summary>
        public void Redraw()
        {
            // Canvas leeren (alle vorherigen Zeichnungen entfernen)
            canvas.Children.Clear();

            // Aktuelle Größe des Controls ermitteln
            double pw = ActualWidth;   // Pixel-Breite
            double ph = ActualHeight;  // Pixel-Höhe

            // Abbrechen falls Control noch nicht gerendert wurde
            if (pw <= 0 || ph <= 0) return;

            // === Seitenverhältnis berechnen und anpassen ===
            // Damit das Spielfeld nicht verzerrt wird, passen wir die Größe an
            double pa = pw / ph;                  // Pixel-Seitenverhältnis
            double a = (double)width / height;    // Spielfeld-Seitenverhältnis

            // Anpassung um Seitenverhältnis beizubehalten
            if (pa > a)
            {
                // Control ist zu breit -> Breite reduzieren
                pw = ph * a;
            }
            else
            {
                // Control ist zu hoch -> Höhe reduzieren
                ph = pw / a;
            }

            // Größe einer einzelnen Zelle berechnen
            double d = pw / width;

            // === Rand-Wände zeichnen ===
            // Obere und untere Wand
            for (int i = 0; i < width; i++)
            {
                DrawImage(wallImage, i * d, 0, d, d);         // Obere Reihe
                DrawImage(wallImage, i * d, ph - d, d, d);    // Untere Reihe
            }
            // Linke und rechte Wand (ohne Ecken, die sind schon gezeichnet)
            for (int j = 1; j < height - 1; j++)
            {
                DrawImage(wallImage, 0, j * d, d, d);         // Linke Spalte
                DrawImage(wallImage, pw - d, j * d, d, d);    // Rechte Spalte
            }

            // === Spielfeld-Elemente zeichnen ===
            foreach (var element in elements)
            {
                // Position in Pixeln berechnen
                double x = element.X * d;
                double y = element.Y * d;

                // Je nach Element-Typ unterschiedlich zeichnen
                switch (element.Type)
                {
                    case ElementType.Robot:
                        DrawImage(robotImage, x, y, d, d);
                        break;
                    case ElementType.Wall:
                        DrawImage(wallImage, x, y, d, d);
                        break;
                    case ElementType.Obstacle:
                        DrawImage(obstacleImage, x, y, d, d);
                        break;
                    case ElementType.Letter:
                        DrawLetter(element.Letter, x, y, d, d);
                        break;
                }
            }

            // === Gitterlinien zeichnen ===
            var pen = new SolidColorBrush(Colors.Black);

            // Vertikale Linien
            for (int i = 0; i <= width; i++)
            {
                var line = new Line
                {
                    X1 = i * d, Y1 = 0,      // Startpunkt (oben)
                    X2 = i * d, Y2 = ph,     // Endpunkt (unten)
                    Stroke = pen,
                    StrokeThickness = 2
                };
                canvas.Children.Add(line);
            }

            // Horizontale Linien
            for (int j = 0; j <= height; j++)
            {
                var line = new Line
                {
                    X1 = 0, Y1 = j * d,      // Startpunkt (links)
                    X2 = pw, Y2 = j * d,     // Endpunkt (rechts)
                    Stroke = pen,
                    StrokeThickness = 2
                };
                canvas.Children.Add(line);
            }
        }

        /// <summary>
        /// Zeichnet ein Bild an der angegebenen Position.
        /// </summary>
        /// <param name="image">Das zu zeichnende Bild</param>
        /// <param name="x">X-Position in Pixeln</param>
        /// <param name="y">Y-Position in Pixeln</param>
        /// <param name="w">Breite in Pixeln</param>
        /// <param name="h">Höhe in Pixeln</param>
        private void DrawImage(BitmapImage image, double x, double y, double w, double h)
        {
            // Image-Element erstellen
            var img = new Image
            {
                Source = image,
                Width = w,
                Height = h
            };

            // Position auf dem Canvas setzen
            Canvas.SetLeft(img, x);
            Canvas.SetTop(img, y);

            // Zum Canvas hinzufügen
            canvas.Children.Add(img);
        }

        /// <summary>
        /// Zeichnet einen Buchstaben an der angegebenen Position.
        /// </summary>
        /// <param name="letter">Der anzuzeigende Buchstabe</param>
        /// <param name="x">X-Position in Pixeln</param>
        /// <param name="y">Y-Position in Pixeln</param>
        /// <param name="w">Breite in Pixeln</param>
        /// <param name="h">Höhe in Pixeln</param>
        private void DrawLetter(string letter, double x, double y, double w, double h)
        {
            // TextBlock für den Buchstaben erstellen
            var text = new TextBlock
            {
                Text = letter,
                FontSize = Math.Max(10, h - 5),  // Schriftgröße dynamisch, mindestens 10
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black,
                Width = w,
                Height = h,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Border als Container (für bessere Zentrierung)
            var border = new Border
            {
                Width = w,
                Height = h,
                Child = text
            };

            // Position auf dem Canvas setzen
            Canvas.SetLeft(border, x);
            Canvas.SetTop(border, y);

            // Zum Canvas hinzufügen
            canvas.Children.Add(border);
        }
    }
}
