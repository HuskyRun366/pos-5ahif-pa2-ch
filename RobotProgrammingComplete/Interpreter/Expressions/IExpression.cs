using System.Threading.Tasks;
using RobotProgrammingComplete.Models;

namespace RobotProgrammingComplete.Interpreter.Expressions
{
    public interface IExpression
    {
        Task RunAsync(ExecutionContext context);
    }
}
