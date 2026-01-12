using RobotProgrammingComplete.Models;

namespace RobotProgrammingComplete.Interpreter.Expressions
{
    public class Condition
    {
        private readonly Direction _direction;
        private readonly string _targetType;

        public Condition(Direction direction, string targetType)
        {
            _direction = direction;
            _targetType = targetType;
        }

        public bool Evaluate(ExecutionContext context)
        {
            var (x, y) = context.GetPositionInDirection(_direction);

            if (_targetType == "OBSTACLE")
            {
                return context.IsObstacle(x, y);
            }
            else
            {
                // Check for specific letter
                return context.IsLetter(x, y, _targetType);
            }
        }
    }
}
