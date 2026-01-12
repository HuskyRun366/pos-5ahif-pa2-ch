using System.Linq;
using System.Threading.Tasks;
using RobotProgrammingComplete.Models;

namespace RobotProgrammingComplete.Interpreter.Expressions
{
    public class CollectExpression : IExpression
    {
        public async Task RunAsync(ExecutionContext context)
        {
            // Find letter at current robot position
            var letter = context.Elements.FirstOrDefault(e =>
                e.Type == ElementType.Letter &&
                e.X == context.RobotX &&
                e.Y == context.RobotY);

            if (letter != null)
            {
                context.CollectedLetters.Add(letter.Letter);
                context.Elements.Remove(letter);
            }

            context.OnUpdate?.Invoke();
            await Task.Delay(context.DelayMs);
        }
    }
}
