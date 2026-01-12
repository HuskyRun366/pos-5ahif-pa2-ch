using RobotProgrammingComplete.Models;

namespace RobotProgrammingComplete.Interpreter.Expressions
{
    /// <summary>
    /// Repräsentiert eine Bedingung für IF- und UNTIL-Anweisungen.
    /// Prüft was sich in einer bestimmten Richtung befindet.
    /// Syntax: direction IS-A target (z.B. "UP IS-A OBSTACLE" oder "LEFT IS-A A")
    /// </summary>
    public class Condition
    {
        // Die zu prüfende Richtung (vom Roboter aus gesehen)
        private readonly Direction direction;

        // Das erwartete Ziel: "OBSTACLE" oder ein Buchstabe (A-Z)
        private readonly string targetType;

        /// <summary>
        /// Erstellt eine neue Bedingung.
        /// </summary>
        /// <param name="dir">Die Richtung zum Prüfen</param>
        /// <param name="target">Das erwartete Ziel (OBSTACLE oder Buchstabe)</param>
        public Condition(Direction dir, string target)
        {
            direction = dir;
            targetType = target;
        }

        /// <summary>
        /// Wertet die Bedingung aus.
        /// </summary>
        /// <param name="context">Der aktuelle Ausführungskontext</param>
        /// <returns>True wenn die Bedingung erfüllt ist</returns>
        public bool Evaluate(ExecutionContext context)
        {
            // Berechne die Position in der angegebenen Richtung
            var (x, y) = context.GetPositionInDirection(direction);

            // Prüfe je nach Zieltyp
            if (targetType == "OBSTACLE")
            {
                // Prüfe auf Hindernis oder Wand
                return context.IsObstacle(x, y);
            }
            else
            {
                // Prüfe auf bestimmten Buchstaben
                return context.IsLetter(x, y, targetType);
            }
        }
    }
}
