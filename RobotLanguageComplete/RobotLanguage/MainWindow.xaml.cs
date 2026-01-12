using Microsoft.Win32;
using System.Globalization;
using System.Windows;
using System.Xml.Linq;

namespace RobotLanguage
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Aufgabe 3: XML-Datei laden und Kreise im Custom-Widget darstellen
        /// </summary>
        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Aufgabe 3 - XML laden
            // 1. OpenFileDialog erstellen und anzeigen
            // 2. XML-Datei einlesen
            // 3. Kreise aus der XML-Datei parsen
            // 4. Kreise im robotCanvas darstellen (robotCanvas.AddCircle(...))

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "XML Dateien (*.xml)|*.xml|Alle Dateien (*.*)|*.*",
                Title = "XML-Datei auswählen"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    robotCanvas.Clear();
                    XDocument doc = XDocument.Load(filePath);
                    foreach (var circle in doc.Descendants("Circle"))
                    {
                        double x = double.Parse(circle.Attribute("X")?.Value ?? "0", CultureInfo.InvariantCulture);
                        double y = double.Parse(circle.Attribute("Y")?.Value ?? "0", CultureInfo.InvariantCulture);
                        double radius = double.Parse(circle.Attribute("Radius")?.Value ?? "0", CultureInfo.InvariantCulture);
                        bool isGoal = bool.Parse(circle.Attribute("IsGoal")?.Value ?? "false");
                        robotCanvas.AddCircle(x, y, radius, isGoal);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Laden der XML: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Aufgabe 3: Alle Inhalte aus dem Custom-Widget entfernen
        /// </summary>
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            robotCanvas.Clear();
        }

        /// <summary>
        /// Aufgabe 6 & 7: Code ausführen und Linien zeichnen
        /// </summary>
        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            string code = txtCode.Text;

            try
            {
                // Linien zurücksetzen für neuen Durchlauf
                robotCanvas.ClearLines();

                // Aufgabe 4 - Tokenize
                Tokenizer tokenizer = new Tokenizer();
                List<Token> tokens = tokenizer.Tokenize(code);

                // Aufgabe 5 - Parse
                Parser parser = new Parser(tokens);
                Expression ast = parser.Parse();

                // Aufgabe 6 - Run
                ast.Execute(robotCanvas);

                // Aufgabe 7 - Hindernisse/Goal prüfen
                bool hitObstacle = robotCanvas.CheckObstacleCollision();
                bool reachedGoal = robotCanvas.CheckGoalReached();

                if (hitObstacle)
                    MessageBox.Show("Fehler: Hindernis getroffen!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                else if (!reachedGoal)
                    MessageBox.Show("Fehler: Ziel nicht erreicht!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    MessageBox.Show("Aufgabe erfolgreich gelöst!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Aufgabe 6: Bei Fehlern MessageBox mit Fehlermeldung anzeigen
                MessageBox.Show($"Fehler beim Ausführen:\n{ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
