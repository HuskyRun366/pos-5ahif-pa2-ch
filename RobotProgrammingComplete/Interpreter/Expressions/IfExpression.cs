using System.Collections.Generic;
using System.Threading.Tasks;
using RobotProgrammingComplete.Models;

namespace RobotProgrammingComplete.Interpreter.Expressions
{
    /// <summary>
    /// Repräsentiert eine IF-Anweisung.
    /// Führt einen Block nur aus, wenn die Bedingung erfüllt ist.
    /// Syntax: IF condition { ... }
    /// </summary>
    public class IfExpression : IExpression
    {
        // Die zu prüfende Bedingung (z.B. "LEFT IS-A A")
        private readonly Condition condition;

        // Die bedingt auszuführenden Anweisungen
        private readonly List<IExpression> body;

        /// <summary>
        /// Erstellt eine neue IF-Anweisung.
        /// </summary>
        /// <param name="cond">Die zu prüfende Bedingung</param>
        /// <param name="statements">Die Anweisungen im Block</param>
        public IfExpression(Condition cond, List<IExpression> statements)
        {
            condition = cond;
            body = statements;
        }

        /// <summary>
        /// Führt den Block aus, falls die Bedingung wahr ist.
        /// </summary>
        public async Task RunAsync(ExecutionContext context)
        {
            // Prüfe die Bedingung
            if (condition.Evaluate(context))
            {
                // Bedingung ist wahr: Führe alle Anweisungen im Block aus
                foreach (var statement in body)
                {
                    await statement.RunAsync(context);
                }
            }
            // Bedingung ist falsch: Block wird übersprungen
        }
    }
}
