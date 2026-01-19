using System.Collections.ObjectModel;
using SchiffeVersenken.Models;

namespace SchiffeVersenken.ViewModels
{
    /// <summary>
    /// ViewModel für ein einzelnes Spielfeld (eigenes oder gegnerisches).
    /// Ermöglicht "Two Views on One Model" durch verschiedene Ansichtsmodi.
    /// </summary>
    public class SpielfeldViewModel : ViewModelBase
    {
        private readonly Spielfeld _spielfeld;
        private readonly bool _istEigenesfeld;

        /// <summary>
        /// Zellen als flache Liste für ItemsControl-Binding.
        /// </summary>
        public ObservableCollection<ZellenViewModel> Zellen { get; } = new();

        private bool _istAktiv;
        /// <summary>True wenn dieses Feld gerade interaktiv ist</summary>
        public bool IstAktiv
        {
            get => _istAktiv;
            set => SetProperty(ref _istAktiv, value);
        }

        private string _titel = string.Empty;
        /// <summary>Titel über dem Spielfeld</summary>
        public string Titel
        {
            get => _titel;
            set => SetProperty(ref _titel, value);
        }

        /// <summary>Spaltenbeschriftung A-J</summary>
        public ObservableCollection<string> SpaltenBeschriftung { get; } = new();

        /// <summary>Zeilenbeschriftung 1-10</summary>
        public ObservableCollection<string> ZeilenBeschriftung { get; } = new();

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="spielfeld">Das zugrundeliegende Spielfeld-Model</param>
        /// <param name="istEigenesfeld">True für eigenes Feld (Schiffe sichtbar)</param>
        public SpielfeldViewModel(Spielfeld spielfeld, bool istEigenesfeld)
        {
            _spielfeld = spielfeld;
            _istEigenesfeld = istEigenesfeld;

            Titel = istEigenesfeld ? "Eigene Flotte" : "Gegnerische Gewässer";

            // Beschriftungen initialisieren
            for (int i = 0; i < Spielfeld.Groesse; i++)
            {
                SpaltenBeschriftung.Add(((char)('A' + i)).ToString());
                ZeilenBeschriftung.Add((i + 1).ToString());
            }

            // Zellen initialisieren (Reihenfolge: y dann x für korrekte Grid-Darstellung)
            for (int y = 0; y < Spielfeld.Groesse; y++)
            {
                for (int x = 0; x < Spielfeld.Groesse; x++)
                {
                    var zelle = spielfeld.GetZelle(x, y)!;
                    Zellen.Add(new ZellenViewModel(zelle, istEigenesfeld));
                }
            }
        }

        /// <summary>
        /// Aktualisiert alle Zellen-ViewModels.
        /// </summary>
        public void Aktualisieren()
        {
            foreach (var zellenVM in Zellen)
            {
                zellenVM.Aktualisieren();
            }
        }

        /// <summary>
        /// Gibt die ZellenViewModel an der Position zurück.
        /// </summary>
        public ZellenViewModel? GetZelle(int x, int y)
        {
            int index = y * Spielfeld.Groesse + x;
            if (index < 0 || index >= Zellen.Count)
                return null;
            return Zellen[index];
        }

        /// <summary>
        /// Setzt die Vorschau für alle Zellen zurück.
        /// </summary>
        public void VorschauZuruecksetzen()
        {
            foreach (var zelle in Zellen)
            {
                zelle.VorschauZuruecksetzen();
            }
        }

        /// <summary>
        /// Zeigt eine Platzierungs-Vorschau an.
        /// </summary>
        public void ZeigeVorschau(List<(int X, int Y)> positionen, bool istGueltig)
        {
            VorschauZuruecksetzen();

            foreach (var (x, y) in positionen)
            {
                var zelle = GetZelle(x, y);
                if (zelle != null)
                {
                    zelle.IstVorschau = true;
                    zelle.IstGueltigeVorschau = istGueltig;
                }
            }
        }
    }
}
