using System.Collections.Generic;
using System.Threading.Tasks;
using RobotProgrammingComplete.Models;

namespace RobotProgrammingComplete.Interpreter.Expressions
{
    public class RepeatExpression : IExpression
    {
        private readonly int _count;
        private readonly List<IExpression> _body;

        public RepeatExpression(int count, List<IExpression> body)
        {
            _count = count;
            _body = body;
        }

        public async Task RunAsync(ExecutionContext context)
        {
            for (int i = 0; i < _count; i++)
            {
                foreach (var statement in _body)
                {
                    await statement.RunAsync(context);
                }
            }
        }
    }
}
