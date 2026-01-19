namespace SchiffeVersenken.Models
{
    /// <summary>
    /// Repräsentiert ein Schiff auf dem Spielfeld.
    /// </summary>
    public class Schiff
    {
        /// <summary>Typ des Schiffs (bestimmt die Länge)</summary>
        public SchiffTyp Typ { get; set; }

        /// <summary>Startposition X (Spalte)</summary>
        public int StartX { get; set; }

        /// <summary>Startposition Y (Zeile)</summary>
        public int StartY { get; set; }

        /// <summary>True = horizontal, False = vertikal</summary>
        public bool Horizontal { get; set; } = true;

        /// <summary>Länge des Schiffs (wird aus Typ abgeleitet)</summary>
        public int Laenge => (int)Typ;

        /// <summary>Anzahl der Treffer auf diesem Schiff</summary>
        public int Treffer { get; set; } = 0;

        /// <summary>Prüft ob das Schiff versenkt wurde</summary>
        public bool IstVersenkt => Treffer >= Laenge;

        /// <summary>
        /// Gibt alle Koordinaten zurück, die dieses Schiff belegt.
        /// </summary>
        public IEnumerable<(int X, int Y)> GetPositionen()
        {
            for (int i = 0; i < Laenge; i++)
            {
                int x = Horizontal ? StartX + i : StartX;
                int y = Horizontal ? StartY : StartY + i;
                yield return (x, y);
            }
        }

        /// <summary>
        /// Gibt alle Koordinaten zurück, die um das Schiff herum blockiert sind.
        /// (Schiffe dürfen sich nicht berühren, auch nicht diagonal)
        /// </summary>
        public IEnumerable<(int X, int Y)> GetSperrzone()
        {
            foreach (var (x, y) in GetPositionen())
            {
                // Alle 8 Nachbarn plus die Position selbst
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int nx = x + dx;
                        int ny = y + dy;
                        // Nur gültige Koordinaten (0-9)
                        if (nx >= 0 && nx < 10 && ny >= 0 && ny < 10)
                        {
                            yield return (nx, ny);
                        }
                    }
                }
            }
        }
    }
}
