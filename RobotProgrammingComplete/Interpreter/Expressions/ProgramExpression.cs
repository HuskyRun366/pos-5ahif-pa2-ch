using System.Collections.Generic;
using System.Threading.Tasks;
using RobotProgrammingComplete.Models;

namespace RobotProgrammingComplete.Interpreter.Expressions
{
    public class ProgramExpression : IExpression
    {
        private readonly List<IExpression> _statements;

        public ProgramExpression(List<IExpression> statements)
        {
            _statements = statements;
        }

        public async Task RunAsync(ExecutionContext context)
        {
            foreach (var statement in _statements)
            {
                await statement.RunAsync(context);
            }
        }
    }
}
