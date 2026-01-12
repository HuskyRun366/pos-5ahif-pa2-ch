namespace RobotProgrammingComplete.Models
{
    /// <summary>
    /// Aufzählung aller möglichen Elementtypen auf dem Spielfeld.
    /// Jede Zelle kann genau einen Elementtyp enthalten.
    /// </summary>
    public enum ElementType
    {
        /// <summary>Hindernis - blockiert den Roboter</summary>
        Obstacle,

        /// <summary>Wand - Begrenzung des Spielfelds</summary>
        Wall,

        /// <summary>Buchstabe - kann vom Roboter gesammelt werden</summary>
        Letter,

        /// <summary>Der Roboter selbst</summary>
        Robot
    }

    /// <summary>
    /// Repräsentiert ein einzelnes Element auf dem Spielfeld.
    /// Speichert Position, Typ und optional einen Buchstaben.
    /// </summary>
    public class RobotElement
    {
        /// <summary>X-Position auf dem Spielfeld (Spalte, 0 = links)</summary>
        public int X { get; set; } = 0;

        /// <summary>Y-Position auf dem Spielfeld (Zeile, 0 = oben)</summary>
        public int Y { get; set; } = 0;

        /// <summary>Der Buchstabe, falls Type == Letter (sonst leer)</summary>
        public string Letter { get; set; } = "";

        /// <summary>Der Typ des Elements (Roboter, Wand, Hindernis, Buchstabe)</summary>
        public ElementType Type { get; set; } = ElementType.Wall;
    }
}
