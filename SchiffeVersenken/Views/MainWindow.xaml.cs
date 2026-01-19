using System.Windows;
using SchiffeVersenken.Controls;
using SchiffeVersenken.ViewModels;

namespace SchiffeVersenken.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HauptViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new HauptViewModel();
            DataContext = _viewModel;
        }

        private void EigenesFeld_ZellenGeklickt(object? sender, ZellenKlickEventArgs e)
        {
            // Während Platzierungsphase: Schiff platzieren
            if (_viewModel.IstPlatzierungsPhase)
            {
                _viewModel.SchiffPlatzieren(e.X, e.Y);
            }
        }

        private void EigenesFeld_ZellenHover(object? sender, ZellenKlickEventArgs e)
        {
            // Vorschau während Platzierung anzeigen
            if (_viewModel.IstPlatzierungsPhase)
            {
                _viewModel.ZeigeVorschau(e.X, e.Y);
            }
        }

        private void EigenesFeld_ZellenHoverEnde(object? sender, EventArgs e)
        {
            _viewModel.VersteckeVorschau();
        }

        private void GegnerFeld_ZellenGeklickt(object? sender, ZellenKlickEventArgs e)
        {
            // Während Spielphase: Schießen
            if (_viewModel.KannSchiessen)
            {
                _viewModel.Schiessen(e.X, e.Y);
            }
        }

        private void Beenden_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Spielregeln_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "SCHIFFE VERSENKEN - Spielregeln\n\n" +
                "1. PLATZIERUNG:\n" +
                "   - Platziere 5 Schiffe auf deinem Spielfeld\n" +
                "   - Schlachtschiff (5), Kreuzer (4), 2x Zerstörer (3), U-Boot (2)\n" +
                "   - Schiffe dürfen sich nicht berühren (auch nicht diagonal)\n" +
                "   - Drücke 'R' zum Drehen\n\n" +
                "2. NETZWERK:\n" +
                "   - Ein Spieler startet als Server (Host)\n" +
                "   - Der andere verbindet sich als Client\n\n" +
                "3. SPIELABLAUF:\n" +
                "   - Der Host beginnt\n" +
                "   - Klicke auf das gegnerische Feld zum Schießen\n" +
                "   - Bei Treffer: Nochmal schießen\n" +
                "   - Bei Daneben: Gegner ist dran\n\n" +
                "4. SPIELENDE:\n" +
                "   - Wer zuerst alle gegnerischen Schiffe versenkt, gewinnt!",
                "Spielregeln",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
