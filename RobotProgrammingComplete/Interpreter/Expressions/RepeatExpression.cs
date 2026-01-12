using System.Collections.Generic;
using System.Threading.Tasks;
using RobotProgrammingComplete.Models;

namespace RobotProgrammingComplete.Interpreter.Expressions
{
    /// <summary>
    /// Repräsentiert eine REPEAT-Schleife.
    /// Wiederholt einen Block von Anweisungen n-mal.
    /// Syntax: REPEAT n { ... }
    /// </summary>
    public class RepeatExpression : IExpression
    {
        // Anzahl der Wiederholungen
        private readonly int count;

        // Die zu wiederholenden Anweisungen
        private readonly List<IExpression> body;

        /// <summary>
        /// Erstellt eine neue REPEAT-Schleife.
        /// </summary>
        /// <param name="n">Anzahl der Wiederholungen</param>
        /// <param name="statements">Die Anweisungen im Schleifenkörper</param>
        public RepeatExpression(int n, List<IExpression> statements)
        {
            count = n;
            body = statements;
        }

        /// <summary>
        /// Führt den Schleifenkörper n-mal aus.
        /// </summary>
        public async Task RunAsync(ExecutionContext context)
        {
            // Wiederhole den Block count-mal
            for (int i = 0; i < count; i++)
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
