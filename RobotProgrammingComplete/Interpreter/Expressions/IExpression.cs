using System.Threading.Tasks;
using RobotProgrammingComplete.Models;

namespace RobotProgrammingComplete.Interpreter.Expressions
{
    /// <summary>
    /// Basis-Interface für alle Ausdrücke im abstrakten Syntaxbaum (AST).
    /// Jede Anweisung (MOVE, COLLECT, REPEAT, etc.) implementiert dieses Interface.
    /// </summary>
    public interface IExpression
    {
        /// <summary>
        /// Führt die Anweisung asynchron aus.
        /// Die Ausführung kann pausiert werden um Animationen sichtbar zu machen.
        /// </summary>
        /// <param name="context">Der Ausführungskontext mit Spielfeldzustand</param>
        Task RunAsync(ExecutionContext context);
    }
}
