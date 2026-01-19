using SchiffeVersenken.Models;

namespace SchiffeVersenken.Services
{
    /// <summary>
    /// Enthält die Spiellogik und Regeln.
    /// </summary>
    public class SpielLogik
    {
        /// <summary>
        /// Verarbeitet einen Schuss und gibt das Ergebnis zurück.
        /// </summary>
        public (SchussErgebnis Ergebnis, Schiff? VersenktesSchiff) VerarbeiteSchuss(
            Spielfeld spielfeld, int x, int y)
        {
            var zelle = spielfeld.GetZelle(x, y);
            if (zelle == null)
                return (SchussErgebnis.Ungueltig, null);

            // Bereits beschossen?
            if (zelle.Zustand == ZellenZustand.Treffer ||
                zelle.Zustand == ZellenZustand.Daneben)
            {
                return (SchussErgebnis.Ungueltig, null);
            }

            if (zelle.Zustand == ZellenZustand.Schiff && zelle.Schiff != null)
            {
                // Treffer!
                zelle.Zustand = ZellenZustand.Treffer;
                zelle.Schiff.Treffer++;

                if (zelle.Schiff.IstVersenkt)
                {
                    return (SchussErgebnis.Versenkt, zelle.Schiff);
                }

                return (SchussErgebnis.Treffer, null);
            }
            else
            {
                // Daneben
                zelle.Zustand = ZellenZustand.Daneben;
                return (SchussErgebnis.Daneben, null);
            }
        }

        /// <summary>
        /// Prüft ob das Spiel beendet ist.
        /// </summary>
        public bool IstSpielBeendet(Spielfeld spielfeld)
        {
            return spielfeld.AlleSchiffeVersenkt;
        }

        /// <summary>
        /// Bestimmt den nächsten Zustand basierend auf dem Schussergebnis.
        /// </summary>
        public SpielPhase GetNaechstePhase(SpielPhase aktuellePhase, SchussErgebnis ergebnis)
        {
            if (aktuellePhase == SpielPhase.AmZug)
            {
                // Eigener Schuss
                return ergebnis switch
                {
                    SchussErgebnis.Treffer => SpielPhase.AmZug,    // Nochmal
                    SchussErgebnis.Versenkt => SpielPhase.AmZug,   // Nochmal
                    SchussErgebnis.Daneben => SpielPhase.GegnerAmZug,
                    _ => aktuellePhase
                };
            }
            else if (aktuellePhase == SpielPhase.GegnerAmZug)
            {
                // Gegner hat geschossen
                return ergebnis switch
                {
                    SchussErgebnis.Treffer => SpielPhase.GegnerAmZug,  // Gegner nochmal
                    SchussErgebnis.Versenkt => SpielPhase.GegnerAmZug, // Gegner nochmal
                    SchussErgebnis.Daneben => SpielPhase.AmZug,        // Jetzt wir
                    _ => aktuellePhase
                };
            }

            return aktuellePhase;
        }
    }
}
