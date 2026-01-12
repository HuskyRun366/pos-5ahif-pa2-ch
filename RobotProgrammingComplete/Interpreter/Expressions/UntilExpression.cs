using System.Collections.Generic;
using System.Threading.Tasks;
using RobotProgrammingComplete.Models;

namespace RobotProgrammingComplete.Interpreter.Expressions
{
    /// <summary>
    /// Repräsentiert eine UNTIL-Schleife.
    /// Wiederholt einen Block solange bis die Bedingung erfüllt ist.
    /// Syntax: UNTIL condition { ... }
    /// </summary>
    public class UntilExpression : IExpression
    {
        // Die Abbruchbedingung (z.B. "UP IS-A OBSTACLE")
        private readonly Condition condition;

        // Die zu wiederholenden Anweisungen
        private readonly List<IExpression> body;

        /// <summary>
        /// Erstellt eine neue UNTIL-Schleife.
        /// </summary>
        /// <param name="cond">Die Bedingung zum Beenden der Schleife</param>
        /// <param name="statements">Die Anweisungen im Schleifenkörper</param>
        public UntilExpression(Condition cond, List<IExpression> statements)
        {
            condition = cond;
            body = statements;
        }

        /// <summary>
        /// Führt den Schleifenkörper aus, bis die Bedingung wahr wird.
        /// </summary>
        public async Task RunAsync(ExecutionContext context)
        {
            // Wiederhole solange die Bedingung NICHT erfüllt ist
            // (UNTIL = bis die Bedingung wahr wird, also while NOT condition)
            while (!condition.Evaluate(context))
            {
                // Führe jede Anweisung im Block aus
                foreach (var statement in body)
                {
                    await statement.RunAsync(context);
                }
            }
        }
    }
}
