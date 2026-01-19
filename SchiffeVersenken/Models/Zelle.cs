namespace SchiffeVersenken.Models
{
    /// <summary>
    /// Repräsentiert eine einzelne Zelle auf dem 10x10 Spielfeld.
    /// </summary>
    public class Zelle
    {
        /// <summary>X-Position (Spalte, 0-9)</summary>
        public int X { get; set; }

        /// <summary>Y-Position (Zeile, 0-9)</summary>
        public int Y { get; set; }

        /// <summary>Aktueller Zustand der Zelle</summary>
        public ZellenZustand Zustand { get; set; } = ZellenZustand.Wasser;

        /// <summary>Referenz auf das Schiff (falls vorhanden)</summary>
        public Schiff? Schiff { get; set; }

        /// <summary>
        /// Prüft ob diese Zelle beschossen werden kann.
        /// </summary>
        public bool KannBeschossenWerden =>
            Zustand == ZellenZustand.Unbekannt ||
            Zustand == ZellenZustand.Wasser ||
            Zustand == ZellenZustand.Schiff;
    }
}
