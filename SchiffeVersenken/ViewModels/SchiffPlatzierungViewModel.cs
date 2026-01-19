using SchiffeVersenken.Models;
using SchiffeVersenken.Services;

namespace SchiffeVersenken.ViewModels
{
    /// <summary>
    /// ViewModel für die Schiffplatzierungsphase.
    /// </summary>
    public class SchiffPlatzierungViewModel : ViewModelBase
    {
        private readonly PlatzierungsValidator _validator;
        private readonly Spielfeld _spielfeld;

        // Schiffe die platziert werden müssen (gemäß offiziellen Regeln)
        private readonly Queue<SchiffTyp> _zuPlatzieren = new(new[]
        {
            SchiffTyp.Schlachtschiff,  // 1x 5er
            SchiffTyp.Kreuzer,          // 1x 4er
            SchiffTyp.Zerstoerer,       // 2x 3er
            SchiffTyp.Zerstoerer,
            SchiffTyp.UBoot             // 1x 2er
        });

        private SchiffTyp? _aktuellerTyp;
        /// <summary>Aktuell zu platzierender Schifftyp</summary>
        public SchiffTyp? AktuellerTyp
        {
            get => _aktuellerTyp;
            private set
            {
                if (SetProperty(ref _aktuellerTyp, value))
                {
                    OnPropertyChanged(nameof(AktuelleSchiffLaenge));
                    OnPropertyChanged(nameof(AktuellerSchiffName));
                }
            }
        }

        public int AktuelleSchiffLaenge => _aktuellerTyp.HasValue ? (int)_aktuellerTyp.Value : 0;

        public string AktuellerSchiffName => _aktuellerTyp switch
        {
            SchiffTyp.Schlachtschiff => "Schlachtschiff (5)",
            SchiffTyp.Kreuzer => "Kreuzer (4)",
            SchiffTyp.Zerstoerer => "Zerstörer (3)",
            SchiffTyp.UBoot => "U-Boot (2)",
            _ => "-"
        };

        private bool _horizontal = true;
        /// <summary>Aktuelle Ausrichtung</summary>
        public bool Horizontal
        {
            get => _horizontal;
            set
            {
                if (SetProperty(ref _horizontal, value))
                {
                    OnPropertyChanged(nameof(AusrichtungText));
                }
            }
        }

        public string AusrichtungText => Horizontal ? "Horizontal" : "Vertikal";

        public bool AlleSchiffePlatziert => _zuPlatzieren.Count == 0;

        public int VerbleibendeSchiffe => _zuPlatzieren.Count;

        public SchiffPlatzierungViewModel(PlatzierungsValidator validator, Spielfeld spielfeld)
        {
            _validator = validator;
            _spielfeld = spielfeld;

            // Erstes Schiff auswählen
            if (_zuPlatzieren.Count > 0)
            {
                AktuellerTyp = _zuPlatzieren.Peek();
            }
        }

        /// <summary>
        /// Versucht das aktuelle Schiff an der Position zu platzieren.
        /// </summary>
        public bool PlatzierenBei(int x, int y)
        {
            if (!_aktuellerTyp.HasValue) return false;

            var schiff = new Schiff
            {
                Typ = _aktuellerTyp.Value,
                StartX = x,
                StartY = y,
                Horizontal = _horizontal
            };

            // Validieren
            if (!_validator.IstGueltigePlatzierung(_spielfeld, schiff))
            {
                return false;
            }

            // Schiff platzieren
            _spielfeld.Schiffe.Add(schiff);
            foreach (var (sx, sy) in schiff.GetPositionen())
            {
                var zelle = _spielfeld.GetZelle(sx, sy);
                if (zelle != null)
                {
                    zelle.Zustand = ZellenZustand.Schiff;
                    zelle.Schiff = schiff;
                }
            }

            // Nächstes Schiff
            _zuPlatzieren.Dequeue();
            AktuellerTyp = _zuPlatzieren.Count > 0 ? _zuPlatzieren.Peek() : null;

            OnPropertyChanged(nameof(AlleSchiffePlatziert));
            OnPropertyChanged(nameof(VerbleibendeSchiffe));

            return true;
        }

        /// <summary>
        /// Dreht die Ausrichtung des aktuellen Schiffs.
        /// </summary>
        public void AktuellesSchiffDrehen()
        {
            Horizontal = !Horizontal;
        }

        /// <summary>
        /// Gibt Vorschau-Positionen für das aktuelle Schiff zurück.
        /// </summary>
        public (List<(int X, int Y)> Positionen, bool IstGueltig) GetVorschau(int x, int y)
        {
            if (!_aktuellerTyp.HasValue)
                return (new List<(int, int)>(), false);

            return _validator.GetPlatzierungsVorschau(_spielfeld, _aktuellerTyp.Value, x, y, _horizontal);
        }

        /// <summary>
        /// Setzt die Platzierung zurück.
        /// </summary>
        public void Zuruecksetzen()
        {
            _zuPlatzieren.Clear();
            _zuPlatzieren.Enqueue(SchiffTyp.Schlachtschiff);
            _zuPlatzieren.Enqueue(SchiffTyp.Kreuzer);
            _zuPlatzieren.Enqueue(SchiffTyp.Zerstoerer);
            _zuPlatzieren.Enqueue(SchiffTyp.Zerstoerer);
            _zuPlatzieren.Enqueue(SchiffTyp.UBoot);

            // Spielfeld leeren
            _spielfeld.Zuruecksetzen();

            AktuellerTyp = _zuPlatzieren.Peek();
            Horizontal = true;

            OnPropertyChanged(nameof(AlleSchiffePlatziert));
            OnPropertyChanged(nameof(VerbleibendeSchiffe));
        }
    }
}
