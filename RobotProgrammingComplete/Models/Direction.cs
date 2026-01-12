namespace RobotProgrammingComplete.Models
{
    /// <summary>
    /// Aufzählung aller möglichen Bewegungsrichtungen des Roboters.
    /// Diese Werte werden sowohl für Bewegungsbefehle als auch für
    /// Bedingungsprüfungen (z.B. "UP IS-A OBSTACLE") verwendet.
    /// </summary>
    public enum Direction
    {
        /// <summary>Nach oben bewegen (Y-Koordinate wird kleiner)</summary>
        Up,

        /// <summary>Nach unten bewegen (Y-Koordinate wird größer)</summary>
        Down,

        /// <summary>Nach links bewegen (X-Koordinate wird kleiner)</summary>
        Left,

        /// <summary>Nach rechts bewegen (X-Koordinate wird größer)</summary>
        Right
    }
}
