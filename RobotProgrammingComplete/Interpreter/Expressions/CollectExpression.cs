using System.Linq;
using System.Threading.Tasks;
using RobotProgrammingComplete.Models;

namespace RobotProgrammingComplete.Interpreter.Expressions
{
    /// <summary>
    /// Repräsentiert einen COLLECT-Befehl.
    /// Sammelt den Buchstaben an der aktuellen Roboterposition ein.
    /// </summary>
    public class CollectExpression : IExpression
    {
        /// <summary>
        /// Sammelt einen Buchstaben ein, falls einer an der Position liegt.
        /// </summary>
        public async Task RunAsync(ExecutionContext context)
        {
            // Suche nach einem Buchstaben an der aktuellen Roboterposition
            var letter = context.Elements.FirstOrDefault(e =>
                e.Type == ElementType.Letter &&   // Muss ein Buchstabe sein
                e.X == context.RobotX &&          // Gleiche X-Position
                e.Y == context.RobotY);           // Gleiche Y-Position

            // Falls ein Buchstabe gefunden wurde
            if (letter != null)
            {
                // Füge den Buchstaben zur Sammlung hinzu
                context.CollectedLetters.Add(letter.Letter);

                // Entferne den Buchstaben vom Spielfeld
                context.Elements.Remove(letter);
            }
            // Falls kein Buchstabe da: nichts tun (kein Fehler)

            // UI-Update auslösen und kurz warten für Animation
            context.OnUpdate?.Invoke();
            await Task.Delay(context.DelayMs);
        }
    }
}
