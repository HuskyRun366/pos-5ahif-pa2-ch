using System.Windows;
using System.Windows.Input;
using SchiffeVersenken.Models;
using SchiffeVersenken.Network;
using SchiffeVersenken.Services;

namespace SchiffeVersenken.ViewModels
{
    /// <summary>
    /// Haupt-ViewModel für das Battleship-Spiel.
    /// Koordiniert Spiellogik, Netzwerk und UI-Updates.
    /// </summary>
    public class HauptViewModel : ViewModelBase
    {
        private readonly SpielKontext _kontext;
        private readonly SpielLogik _spielLogik;
        private readonly PlatzierungsValidator _validator;

        // Netzwerk-Komponenten
        private NetzwerkServer? _server;
        private NetzwerkClient? _client;
        private bool _istVerbunden;
        private bool _gegnerBereit;

        /// <summary>ViewModel für eigenes Spielfeld</summary>
        public SpielfeldViewModel EigenesFeldVM { get; }

        /// <summary>ViewModel für Gegnerfeld</summary>
        public SpielfeldViewModel GegnerFeldVM { get; }

        /// <summary>ViewModel für Schiffplatzierung</summary>
        public SchiffPlatzierungViewModel PlatzierungVM { get; }

        private string _statusText = "Willkommen zu Schiffe Versenken!";
        /// <summary>Statusanzeige</summary>
        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        private SpielPhase _aktuellePhase = SpielPhase.Platzierung;
        /// <summary>Aktuelle Spielphase</summary>
        public SpielPhase AktuellePhase
        {
            get => _aktuellePhase;
            set
            {
                if (SetProperty(ref _aktuellePhase, value))
                {
                    OnPropertyChanged(nameof(IstPlatzierungsPhase));
                    OnPropertyChanged(nameof(IstSpielPhase));
                    OnPropertyChanged(nameof(KannSchiessen));
                    OnPropertyChanged(nameof(PhaseText));
                }
            }
        }

        public bool IstPlatzierungsPhase => AktuellePhase == SpielPhase.Platzierung;
        public bool IstSpielPhase => AktuellePhase == SpielPhase.AmZug || AktuellePhase == SpielPhase.GegnerAmZug;
        public bool KannSchiessen => AktuellePhase == SpielPhase.AmZug && _istVerbunden;

        public string PhaseText => AktuellePhase switch
        {
            SpielPhase.Platzierung => "Platzierung",
            SpielPhase.Warten => "Warten",
            SpielPhase.AmZug => "Du bist am Zug",
            SpielPhase.GegnerAmZug => "Gegner am Zug",
            SpielPhase.Ende => "Spiel beendet",
            _ => ""
        };

        private string _spielerName = "Spieler";
        public string SpielerName
        {
            get => _spielerName;
            set => SetProperty(ref _spielerName, value);
        }

        private string _gegnerName = "Gegner";
        public string GegnerName
        {
            get => _gegnerName;
            set => SetProperty(ref _gegnerName, value);
        }

        private int _eigenePunkte;
        public int EigenePunkte
        {
            get => _eigenePunkte;
            set => SetProperty(ref _eigenePunkte, value);
        }

        private int _gegnerPunkte;
        public int GegnerPunkte
        {
            get => _gegnerPunkte;
            set => SetProperty(ref _gegnerPunkte, value);
        }

        // Commands
        public ICommand ServerStartenCommand { get; }
        public ICommand VerbindenCommand { get; }
        public ICommand SchiffDrehenCommand { get; }
        public ICommand BereitCommand { get; }
        public ICommand NeuesSpielCommand { get; }
        public ICommand ZuruecksetzenCommand { get; }

        public HauptViewModel()
        {
            _kontext = new SpielKontext();
            _spielLogik = new SpielLogik();
            _validator = new PlatzierungsValidator();

            // Gegnerfeld auf Unbekannt setzen
            for (int x = 0; x < Spielfeld.Groesse; x++)
            {
                for (int y = 0; y < Spielfeld.Groesse; y++)
                {
                    var zelle = _kontext.GegnerSpielfeld.GetZelle(x, y);
                    if (zelle != null)
                    {
                        zelle.Zustand = ZellenZustand.Unbekannt;
                    }
                }
            }

            // ViewModels für die zwei Ansichten initialisieren
            EigenesFeldVM = new SpielfeldViewModel(_kontext.EigenesSpielfeld, istEigenesfeld: true);
            GegnerFeldVM = new SpielfeldViewModel(_kontext.GegnerSpielfeld, istEigenesfeld: false);
            PlatzierungVM = new SchiffPlatzierungViewModel(_validator, _kontext.EigenesSpielfeld);

            // Kontext-Callbacks registrieren
            _kontext.OnUpdate = () => AktualisierenUI();
            _kontext.OnStatusNachricht = msg => StatusText = msg;
            _kontext.OnSpielEnde = gewonnen => ZeigeSpielEnde(gewonnen);

            // Commands initialisieren
            ServerStartenCommand = new RelayCommand(ServerStarten, () => !_istVerbunden);
            VerbindenCommand = new RelayCommand(VerbindenDialog, () => !_istVerbunden);
            SchiffDrehenCommand = new RelayCommand(PlatzierungVM.AktuellesSchiffDrehen, () => IstPlatzierungsPhase);
            BereitCommand = new RelayCommand(SpielBereit, () => PlatzierungVM.AlleSchiffePlatziert && _istVerbunden);
            NeuesSpielCommand = new RelayCommand(NeuesSpiel);
            ZuruecksetzenCommand = new RelayCommand(SchiffePlatzierungZuruecksetzen, () => IstPlatzierungsPhase);
        }

        /// <summary>
        /// Startet den TCP-Server für Netzwerkspiele.
        /// </summary>
        private async void ServerStarten()
        {
            StatusText = "Warte auf Gegner (Port 5000)...";
            _kontext.IstHost = true;

            try
            {
                _server = new NetzwerkServer();
                _server.OnNachrichtEmpfangen = VerarbeiteNetzwerkNachricht;
                _server.OnVerbindungGetrennt = OnVerbindungGetrennt;
                await _server.StartenAsync(5000);

                _istVerbunden = true;
                StatusText = "Gegner verbunden! Platziere deine Schiffe.";

                // Begrüßung senden
                await _server.SendenAsync(new NetzwerkNachricht
                {
                    Typ = NachrichtTyp.VerbindungAkzeptiert,
                    SpielerName = SpielerName
                });
            }
            catch (Exception ex)
            {
                StatusText = $"Server-Fehler: {ex.Message}";
            }
        }

        /// <summary>
        /// Zeigt den Verbindungsdialog an.
        /// </summary>
        private void VerbindenDialog()
        {
            // Einfache Eingabe über MessageBox (in echter App: eigener Dialog)
            var ip = Microsoft.VisualBasic.Interaction.InputBox(
                "Geben Sie die IP-Adresse des Servers ein:",
                "Verbinden",
                "127.0.0.1");

            if (!string.IsNullOrWhiteSpace(ip))
            {
                Verbinden(ip);
            }
        }

        /// <summary>
        /// Verbindet zu einem Server.
        /// </summary>
        private async void Verbinden(string ipAdresse)
        {
            StatusText = $"Verbinde zu {ipAdresse}...";
            _kontext.IstHost = false;

            try
            {
                _client = new NetzwerkClient();
                _client.OnNachrichtEmpfangen = VerarbeiteNetzwerkNachricht;
                _client.OnVerbindungGetrennt = OnVerbindungGetrennt;
                await _client.VerbindenAsync(ipAdresse, 5000);

                _istVerbunden = true;
                StatusText = "Verbunden! Platziere deine Schiffe.";

                // Begrüßung senden
                await _client.SendenAsync(new NetzwerkNachricht
                {
                    Typ = NachrichtTyp.Verbinden,
                    SpielerName = SpielerName
                });
            }
            catch (Exception ex)
            {
                StatusText = $"Verbindungsfehler: {ex.Message}";
            }
        }

        /// <summary>
        /// Wird aufgerufen wenn die Verbindung getrennt wird.
        /// </summary>
        private void OnVerbindungGetrennt()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _istVerbunden = false;
                StatusText = "Verbindung getrennt!";
                AktuellePhase = SpielPhase.Ende;
            });
        }

        /// <summary>
        /// Verarbeitet einen Schuss des Spielers.
        /// </summary>
        public async void Schiessen(int x, int y)
        {
            if (!KannSchiessen) return;

            // Prüfen ob Zelle bereits beschossen
            var zelle = GegnerFeldVM.GetZelle(x, y);
            if (zelle == null || !zelle.IstKlickbar) return;

            // Nachricht an Gegner senden
            var nachricht = new NetzwerkNachricht
            {
                Typ = NachrichtTyp.Schuss,
                X = x,
                Y = y
            };

            await SendeNachrichtAsync(nachricht);
            StatusText = $"Schuss auf {(char)('A' + x)}{y + 1}...";
        }

        /// <summary>
        /// Platziert ein Schiff während der Platzierungsphase.
        /// </summary>
        public void SchiffPlatzieren(int x, int y)
        {
            if (!IstPlatzierungsPhase) return;

            if (PlatzierungVM.PlatzierenBei(x, y))
            {
                EigenesFeldVM.Aktualisieren();
                EigenesFeldVM.VorschauZuruecksetzen();

                if (PlatzierungVM.AlleSchiffePlatziert)
                {
                    StatusText = _istVerbunden
                        ? "Alle Schiffe platziert! Klicke 'Bereit' zum Starten."
                        : "Alle Schiffe platziert! Warte auf Verbindung.";
                }
                else
                {
                    StatusText = $"Platziere: {PlatzierungVM.AktuellerSchiffName}";
                }
            }
        }

        /// <summary>
        /// Zeigt eine Platzierungsvorschau an.
        /// </summary>
        public void ZeigeVorschau(int x, int y)
        {
            if (!IstPlatzierungsPhase || PlatzierungVM.AlleSchiffePlatziert) return;

            var (positionen, gueltig) = PlatzierungVM.GetVorschau(x, y);
            EigenesFeldVM.ZeigeVorschau(positionen, gueltig);
        }

        /// <summary>
        /// Versteckt die Platzierungsvorschau.
        /// </summary>
        public void VersteckeVorschau()
        {
            EigenesFeldVM.VorschauZuruecksetzen();
        }

        /// <summary>
        /// Setzt die Schiffeplatzierung zurück.
        /// </summary>
        private void SchiffePlatzierungZuruecksetzen()
        {
            PlatzierungVM.Zuruecksetzen();
            EigenesFeldVM.Aktualisieren();
            StatusText = $"Platziere: {PlatzierungVM.AktuellerSchiffName}";
        }

        /// <summary>
        /// Signalisiert Spielbereitschaft nach Platzierung.
        /// </summary>
        private async void SpielBereit()
        {
            AktuellePhase = SpielPhase.Warten;
            StatusText = "Warte auf Gegner...";

            var nachricht = new NetzwerkNachricht { Typ = NachrichtTyp.Bereit };
            await SendeNachrichtAsync(nachricht);

            // Falls Gegner bereits bereit ist
            if (_gegnerBereit)
            {
                SpielStarten();
            }
        }

        /// <summary>
        /// Startet das Spiel wenn beide Spieler bereit sind.
        /// </summary>
        private void SpielStarten()
        {
            // Host beginnt
            AktuellePhase = _kontext.IstHost ? SpielPhase.AmZug : SpielPhase.GegnerAmZug;
            StatusText = AktuellePhase == SpielPhase.AmZug
                ? "Du bist am Zug! Klicke auf das gegnerische Feld."
                : "Gegner ist am Zug...";
        }

        /// <summary>
        /// Verarbeitet eingehende Netzwerk-Nachrichten.
        /// </summary>
        private void VerarbeiteNetzwerkNachricht(NetzwerkNachricht nachricht)
        {
            // Dispatcher.Invoke für UI-Thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                switch (nachricht.Typ)
                {
                    case NachrichtTyp.Verbinden:
                        GegnerName = nachricht.SpielerName;
                        break;

                    case NachrichtTyp.VerbindungAkzeptiert:
                        GegnerName = nachricht.SpielerName;
                        break;

                    case NachrichtTyp.Bereit:
                        GegnerBereit();
                        break;

                    case NachrichtTyp.Schuss:
                        VerarbeiteGegnerSchuss(nachricht.X, nachricht.Y);
                        break;

                    case NachrichtTyp.SchussErgebnis:
                        VerarbeiteSchussErgebnis(nachricht);
                        break;

                    case NachrichtTyp.SpielEnde:
                        ZeigeSpielEnde(nachricht.Daten == "Verloren"); // Gegner verloren = wir gewonnen
                        break;
                }
            });
        }

        /// <summary>
        /// Wird aufgerufen wenn der Gegner bereit ist.
        /// </summary>
        private void GegnerBereit()
        {
            _gegnerBereit = true;

            if (AktuellePhase == SpielPhase.Warten)
            {
                SpielStarten();
            }
            else
            {
                StatusText = "Gegner ist bereit! Platziere deine Schiffe.";
            }
        }

        /// <summary>
        /// Verarbeitet einen Schuss des Gegners.
        /// </summary>
        private async void VerarbeiteGegnerSchuss(int x, int y)
        {
            var ergebnis = _kontext.GegnerSchuss(x, y);
            EigenesFeldVM.Aktualisieren();

            // Ergebnis zurücksenden
            var antwort = new NetzwerkNachricht
            {
                Typ = NachrichtTyp.SchussErgebnis,
                X = x,
                Y = y,
                Ergebnis = ergebnis
            };

            await SendeNachrichtAsync(antwort);

            // Prüfen ob Spiel verloren
            if (_kontext.EigenesSpielfeld.AlleSchiffeVersenkt)
            {
                AktuellePhase = SpielPhase.Ende;
                StatusText = "Alle Schiffe versenkt! Du hast verloren.";
                GegnerPunkte++;

                await SendeNachrichtAsync(new NetzwerkNachricht
                {
                    Typ = NachrichtTyp.SpielEnde,
                    Daten = "Verloren"
                });
            }
            else if (ergebnis == SchussErgebnis.Daneben)
            {
                // Bei Daneben: Eigener Zug
                AktuellePhase = SpielPhase.AmZug;
                StatusText = "Gegner hat verfehlt! Du bist am Zug.";
            }
            else
            {
                StatusText = ergebnis == SchussErgebnis.Versenkt
                    ? $"Gegner hat ein Schiff versenkt! ({_kontext.EigenesSpielfeld.VerbleibendeSchiffe} übrig)"
                    : "Gegner hat getroffen!";
            }
        }

        /// <summary>
        /// Verarbeitet das Ergebnis eines eigenen Schusses.
        /// </summary>
        private void VerarbeiteSchussErgebnis(NetzwerkNachricht nachricht)
        {
            var zelle = _kontext.GegnerSpielfeld.GetZelle(nachricht.X, nachricht.Y);
            if (zelle == null) return;

            zelle.Zustand = nachricht.Ergebnis switch
            {
                SchussErgebnis.Treffer => ZellenZustand.Treffer,
                SchussErgebnis.Versenkt => ZellenZustand.Treffer,
                _ => ZellenZustand.Daneben
            };

            GegnerFeldVM.Aktualisieren();

            switch (nachricht.Ergebnis)
            {
                case SchussErgebnis.Treffer:
                    StatusText = "Treffer! Nochmal schießen!";
                    // Bleibt am Zug
                    break;

                case SchussErgebnis.Versenkt:
                    StatusText = "Versenkt! Nochmal schießen!";
                    // Bleibt am Zug
                    break;

                case SchussErgebnis.Daneben:
                    StatusText = "Daneben! Gegner ist dran.";
                    AktuellePhase = SpielPhase.GegnerAmZug;
                    break;
            }
        }

        private void AktualisierenUI()
        {
            EigenesFeldVM.Aktualisieren();
            GegnerFeldVM.Aktualisieren();
        }

        private void ZeigeSpielEnde(bool gewonnen)
        {
            AktuellePhase = SpielPhase.Ende;
            if (gewonnen)
            {
                EigenePunkte++;
                StatusText = "Glückwunsch! Du hast gewonnen!";
            }
            else
            {
                GegnerPunkte++;
                StatusText = "Schade! Du hast verloren.";
            }

            MessageBox.Show(
                gewonnen ? "Du hast gewonnen!" : "Du hast verloren!",
                "Spiel beendet",
                MessageBoxButton.OK,
                gewonnen ? MessageBoxImage.Information : MessageBoxImage.Exclamation);
        }

        private void NeuesSpiel()
        {
            _kontext.Zuruecksetzen();
            PlatzierungVM.Zuruecksetzen();
            EigenesFeldVM.Aktualisieren();
            GegnerFeldVM.Aktualisieren();

            _gegnerBereit = false;
            AktuellePhase = SpielPhase.Platzierung;
            StatusText = $"Neues Spiel! Platziere: {PlatzierungVM.AktuellerSchiffName}";
        }

        private async Task SendeNachrichtAsync(NetzwerkNachricht nachricht)
        {
            if (_server != null && _server.IstVerbunden)
                await _server.SendenAsync(nachricht);
            else if (_client != null && _client.IstVerbunden)
                await _client.SendenAsync(nachricht);
        }
    }
}
