using System.Collections.Generic;
using System.Threading.Tasks;
using RobotProgrammingComplete.Models;

namespace RobotProgrammingComplete.Interpreter.Expressions
{
    public class IfExpression : IExpression
    {
        private readonly Condition _condition;
        private readonly List<IExpression> _body;

        public IfExpression(Condition condition, List<IExpression> body)
        {
            _condition = condition;
            _body = body;
        }

        public async Task RunAsync(ExecutionContext context)
        {
            if (_condition.Evaluate(context))
            {
                foreach (var statement in _body)
                {
                    await statement.RunAsync(context);
                }
            }
        }
    }
}
