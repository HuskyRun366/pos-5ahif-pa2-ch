using SchiffeVersenken.Models;

namespace SchiffeVersenken.ViewModels
{
    /// <summary>
    /// ViewModel für eine einzelne Zelle.
    /// </summary>
    public class ZellenViewModel : ViewModelBase
    {
        private readonly Zelle _zelle;
        private readonly bool _zeigSchiffe;

        public int X => _zelle.X;
        public int Y => _zelle.Y;

        private string _anzeige = string.Empty;
        /// <summary>Visueller Zustand für Binding</summary>
        public string Anzeige
        {
            get => _anzeige;
            set => SetProperty(ref _anzeige, value);
        }

        private bool _istKlickbar;
        /// <summary>Kann angeklickt werden</summary>
        public bool IstKlickbar
        {
            get => _istKlickbar;
            set => SetProperty(ref _istKlickbar, value);
        }

        private bool _istVorschau;
        /// <summary>Zeigt Platzierungs-Vorschau an</summary>
        public bool IstVorschau
        {
            get => _istVorschau;
            set => SetProperty(ref _istVorschau, value);
        }

        private bool _istGueltigeVorschau;
        /// <summary>True wenn Vorschau-Position gültig ist</summary>
        public bool IstGueltigeVorschau
        {
            get => _istGueltigeVorschau;
            set => SetProperty(ref _istGueltigeVorschau, value);
        }

        public ZellenViewModel(Zelle zelle, bool zeigSchiffe)
        {
            _zelle = zelle;
            _zeigSchiffe = zeigSchiffe;
            Aktualisieren();
        }

        /// <summary>
        /// Aktualisiert die Anzeige basierend auf dem Zellen-Zustand.
        /// </summary>
        public void Aktualisieren()
        {
            Anzeige = _zelle.Zustand switch
            {
                ZellenZustand.Treffer => "Treffer",
                ZellenZustand.Daneben => "Daneben",
                ZellenZustand.Schiff when _zeigSchiffe => "Schiff",
                ZellenZustand.Unbekannt => "Unbekannt",
                _ => "Wasser"
            };

            IstKlickbar = !_zeigSchiffe &&
                (_zelle.Zustand == ZellenZustand.Unbekannt ||
                 _zelle.Zustand == ZellenZustand.Wasser);
        }

        /// <summary>
        /// Setzt die Vorschau zurück.
        /// </summary>
        public void VorschauZuruecksetzen()
        {
            IstVorschau = false;
            IstGueltigeVorschau = false;
        }
    }
}
