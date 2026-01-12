using System.Linq;
using System.Threading.Tasks;
using RobotProgrammingComplete.Models;

namespace RobotProgrammingComplete.Interpreter.Expressions
{
    /// <summary>
    /// Repräsentiert einen MOVE-Befehl.
    /// Bewegt den Roboter um ein Feld in die angegebene Richtung.
    /// </summary>
    public class MoveExpression : IExpression
    {
        // Die Bewegungsrichtung (UP, DOWN, LEFT, RIGHT)
        private readonly Direction direction;

        /// <summary>
        /// Erstellt einen neuen Bewegungsbefehl.
        /// </summary>
        /// <param name="dir">Die Richtung in die sich der Roboter bewegen soll</param>
        public MoveExpression(Direction dir)
        {
            direction = dir;
        }

        /// <summary>
        /// Führt die Bewegung aus, falls kein Hindernis im Weg ist.
        /// </summary>
        public async Task RunAsync(ExecutionContext context)
        {
            // Berechne die Zielposition basierend auf der Richtung
            var (dx, dy) = context.GetDirectionOffset(direction);
            int newX = context.RobotX + dx;
            int newY = context.RobotY + dy;

            // Prüfe ob die Zielposition frei ist (keine Wand oder Hindernis)
            if (!context.IsObstacle(newX, newY))
            {
                // Finde das Roboter-Element in der Element-Liste
                var robot = context.Elements.FirstOrDefault(e => e.Type == ElementType.Robot);
                if (robot != null)
                {
                    // Aktualisiere die Roboterposition
                    robot.X = newX;
                    robot.Y = newY;
                    context.RobotX = newX;
                    context.RobotY = newY;
                }
            }
            // Falls blockiert: Roboter bleibt stehen (keine Fehlermeldung)

            // UI-Update auslösen und kurz warten für Animation
            context.OnUpdate?.Invoke();
            await Task.Delay(context.DelayMs);
        }
    }
}
