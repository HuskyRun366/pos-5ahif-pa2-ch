using System;
using System.Collections.Generic;
using System.Linq;

namespace RobotProgrammingComplete.Models
{
    /// <summary>
    /// Enthält den gesamten Zustand während der Programmausführung.
    /// Speichert die Roboterposition, Spielfelddaten und gesammelte Buchstaben.
    /// Wird vom Interpreter verwendet, um den Programmzustand zu verwalten.
    /// </summary>
    public class ExecutionContext
    {
        /// <summary>Aktuelle X-Position des Roboters (Spalte)</summary>
        public int RobotX { get; set; }

        /// <summary>Aktuelle Y-Position des Roboters (Zeile)</summary>
        public int RobotY { get; set; }

        /// <summary>Breite des Spielfelds in Zellen</summary>
        public int FieldWidth { get; set; }

        /// <summary>Höhe des Spielfelds in Zellen</summary>
        public int FieldHeight { get; set; }

        /// <summary>Liste aller Elemente auf dem Spielfeld</summary>
        public List<RobotElement> Elements { get; set; } = new();

        /// <summary>Liste der vom Roboter eingesammelten Buchstaben</summary>
        public List<string> CollectedLetters { get; set; } = new();

        /// <summary>Callback-Funktion die nach jedem Schritt aufgerufen wird (für UI-Updates)</summary>
        public Action? OnUpdate { get; set; }

        /// <summary>Verzögerung in Millisekunden zwischen den Schritten</summary>
        public int DelayMs { get; set; } = 1000;

        /// <summary>
        /// Sucht ein Element an der angegebenen Position.
        /// </summary>
        /// <param name="x">X-Koordinate (Spalte)</param>
        /// <param name="y">Y-Koordinate (Zeile)</param>
        /// <returns>Das Element an der Position, oder null falls keines vorhanden</returns>
        public RobotElement? GetElementAt(int x, int y)
        {
            // Durchsuche alle Elemente nach übereinstimmenden Koordinaten
            return Elements.FirstOrDefault(e => e.X == x && e.Y == y);
        }

        /// <summary>
        /// Prüft ob sich an der Position ein Hindernis oder eine Wand befindet.
        /// </summary>
        /// <param name="x">X-Koordinate (Spalte)</param>
        /// <param name="y">Y-Koordinate (Zeile)</param>
        /// <returns>True wenn die Position blockiert ist</returns>
        public bool IsObstacle(int x, int y)
        {
            // Randprüfung: Außerhalb des Spielfelds ist immer blockiert (Wandgrenzen)
            if (x <= 0 || x >= FieldWidth - 1 || y <= 0 || y >= FieldHeight - 1)
                return true;

            // Prüfe ob an der Position ein Hindernis-Element liegt
            var element = GetElementAt(x, y);
            return element?.Type == ElementType.Obstacle;
        }

        /// <summary>
        /// Prüft ob sich an der Position ein bestimmter Buchstabe befindet.
        /// </summary>
        /// <param name="x">X-Koordinate (Spalte)</param>
        /// <param name="y">Y-Koordinate (Zeile)</param>
        /// <param name="letter">Der gesuchte Buchstabe</param>
        /// <returns>True wenn der Buchstabe an der Position liegt</returns>
        public bool IsLetter(int x, int y, string letter)
        {
            var element = GetElementAt(x, y);

            // Element muss vom Typ Letter sein UND den gesuchten Buchstaben enthalten
            return element?.Type == ElementType.Letter && element.Letter == letter;
        }

        /// <summary>
        /// Wandelt eine Richtung in einen Offset-Vektor um.
        /// </summary>
        /// <param name="direction">Die Bewegungsrichtung</param>
        /// <returns>Tuple mit (deltaX, deltaY) - jeweils -1, 0 oder 1</returns>
        public (int dx, int dy) GetDirectionOffset(Direction direction)
        {
            // Switch-Expression: Wandelt Richtung in Koordinatenänderung um
            return direction switch
            {
                Direction.Up => (0, -1),     // Nach oben = Y wird kleiner
                Direction.Down => (0, 1),    // Nach unten = Y wird größer
                Direction.Left => (-1, 0),   // Nach links = X wird kleiner
                Direction.Right => (1, 0),   // Nach rechts = X wird größer
                _ => (0, 0)                  // Fallback: keine Bewegung
            };
        }

        /// <summary>
        /// Berechnet die Zielposition in einer bestimmten Richtung.
        /// </summary>
        /// <param name="direction">Die Richtung ausgehend von der Roboterposition</param>
        /// <returns>Die Koordinaten der Nachbarzelle in dieser Richtung</returns>
        public (int x, int y) GetPositionInDirection(Direction direction)
        {
            // Hole den Offset für die Richtung
            var (dx, dy) = GetDirectionOffset(direction);

            // Addiere Offset zur aktuellen Roboterposition
            return (RobotX + dx, RobotY + dy);
        }
    }
}
