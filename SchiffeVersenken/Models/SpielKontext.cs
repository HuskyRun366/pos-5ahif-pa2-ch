namespace SchiffeVersenken.Models
{
    /// <summary>
    /// Enthält den gesamten Spielzustand.
    /// Zentrales Objekt für die Spiellogik und UI-Kommunikation.
    /// </summary>
    public class SpielKontext
    {
        /// <summary>Eigenes Spielfeld (mit eigenen Schiffen)</summary>
        public Spielfeld EigenesSpielfeld { get; set; } = new();

        /// <summary>Gegnerisches Spielfeld (Ansicht für Angriffe)</summary>
        public Spielfeld GegnerSpielfeld { get; set; } = new();

        /// <summary>Aktuelle Spielphase</summary>
        public SpielPhase Phase { get; set; } = SpielPhase.Platzierung;

        /// <summary>True wenn dieser Spieler der Host (Server) ist</summary>
        public bool IstHost { get; set; } = false;

        /// <summary>Spielername</summary>
        public string SpielerName { get; set; } = "Spieler";

        /// <summary>Gegnername</summary>
        public string GegnerName { get; set; } = "Gegner";

        /// <summary>Anzahl eigener Siege</summary>
        public int EigenePunkte { get; set; } = 0;

        /// <summary>Anzahl Gegner-Siege</summary>
        public int GegnerPunkte { get; set; } = 0;

        /// <summary>Callback für UI-Updates</summary>
        public Action? OnUpdate { get; set; }

        /// <summary>Callback für Statusmeldungen</summary>
        public Action<string>? OnStatusNachricht { get; set; }

        /// <summary>Callback wenn das Spiel endet</summary>
        public Action<bool>? OnSpielEnde { get; set; } // true = gewonnen

        /// <summary>
        /// Verarbeitet einen gegnerischen Schuss auf das eigene Feld.
        /// </summary>
        public SchussErgebnis GegnerSchuss(int x, int y)
        {
            var zelle = EigenesSpielfeld.GetZelle(x, y);
            if (zelle == null) return SchussErgebnis.Ungueltig;

            if (zelle.Zustand == ZellenZustand.Schiff)
            {
                zelle.Zustand = ZellenZustand.Treffer;
                if (zelle.Schiff != null)
                {
                    zelle.Schiff.Treffer++;
                    if (zelle.Schiff.IstVersenkt)
                    {
                        return SchussErgebnis.Versenkt;
                    }
                }
                return SchussErgebnis.Treffer;
            }
            else if (zelle.Zustand == ZellenZustand.Wasser)
            {
                zelle.Zustand = ZellenZustand.Daneben;
                return SchussErgebnis.Daneben;
            }

            return SchussErgebnis.Ungueltig;
        }

        /// <summary>
        /// Setzt den Spielkontext für ein neues Spiel zurück.
        /// </summary>
        public void Zuruecksetzen()
        {
            EigenesSpielfeld.Zuruecksetzen();
            GegnerSpielfeld.Zuruecksetzen();
            Phase = SpielPhase.Platzierung;

            // Gegnerfeld auf Unbekannt setzen
            for (int x = 0; x < Spielfeld.Groesse; x++)
            {
                for (int y = 0; y < Spielfeld.Groesse; y++)
                {
                    var zelle = GegnerSpielfeld.GetZelle(x, y);
                    if (zelle != null)
                    {
                        zelle.Zustand = ZellenZustand.Unbekannt;
                    }
                }
            }
        }
    }
}
