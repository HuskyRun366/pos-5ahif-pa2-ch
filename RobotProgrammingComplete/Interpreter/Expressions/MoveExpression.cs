using System.Linq;
using System.Threading.Tasks;
using RobotProgrammingComplete.Models;

namespace RobotProgrammingComplete.Interpreter.Expressions
{
    public class MoveExpression : IExpression
    {
        private readonly Direction _direction;

        public MoveExpression(Direction direction)
        {
            _direction = direction;
        }

        public async Task RunAsync(ExecutionContext context)
        {
            var (dx, dy) = context.GetDirectionOffset(_direction);
            int newX = context.RobotX + dx;
            int newY = context.RobotY + dy;

            // Check for collision with wall or obstacle
            if (!context.IsObstacle(newX, newY))
            {
                // Update robot position in elements list
                var robot = context.Elements.FirstOrDefault(e => e.Type == ElementType.Robot);
                if (robot != null)
                {
                    robot.X = newX;
                    robot.Y = newY;
                    context.RobotX = newX;
                    context.RobotY = newY;
                }
            }

            context.OnUpdate?.Invoke();
            await Task.Delay(context.DelayMs);
        }
    }
}
