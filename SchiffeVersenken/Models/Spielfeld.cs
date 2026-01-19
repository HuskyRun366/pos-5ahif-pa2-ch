namespace SchiffeVersenken.Models
{
    /// <summary>
    /// Repräsentiert ein 10x10 Spielfeld mit Zellen und Schiffen.
    /// </summary>
    public class Spielfeld
    {
        /// <summary>Konstante Spielfeldgröße</summary>
        public const int Groesse = 10;

        /// <summary>2D-Array aller Zellen</summary>
        public Zelle[,] Zellen { get; private set; }

        /// <summary>Liste aller platzierten Schiffe</summary>
        public List<Schiff> Schiffe { get; private set; } = new();

        /// <summary>
        /// Konstruktor: Initialisiert leeres Spielfeld.
        /// </summary>
        public Spielfeld()
        {
            Zellen = new Zelle[Groesse, Groesse];

            for (int x = 0; x < Groesse; x++)
            {
                for (int y = 0; y < Groesse; y++)
                {
                    Zellen[x, y] = new Zelle { X = x, Y = y };
                }
            }
        }

        /// <summary>
        /// Gibt die Zelle an der angegebenen Position zurück.
        /// </summary>
        public Zelle? GetZelle(int x, int y)
        {
            if (x < 0 || x >= Groesse || y < 0 || y >= Groesse)
                return null;
            return Zellen[x, y];
        }

        /// <summary>
        /// Prüft ob alle Schiffe versenkt wurden.
        /// </summary>
        public bool AlleSchiffeVersenkt => Schiffe.Count > 0 && Schiffe.All(s => s.IstVersenkt);

        /// <summary>
        /// Zählt die verbleibenden (nicht versenkten) Schiffe.
        /// </summary>
        public int VerbleibendeSchiffe => Schiffe.Count(s => !s.IstVersenkt);

        /// <summary>
        /// Setzt das Spielfeld zurück.
        /// </summary>
        public void Zuruecksetzen()
        {
            Schiffe.Clear();
            for (int x = 0; x < Groesse; x++)
            {
                for (int y = 0; y < Groesse; y++)
                {
                    Zellen[x, y].Zustand = ZellenZustand.Wasser;
                    Zellen[x, y].Schiff = null;
                }
            }
        }
    }
}
