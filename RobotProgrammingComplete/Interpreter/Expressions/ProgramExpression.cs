using System.Collections.Generic;
using System.Threading.Tasks;
using RobotProgrammingComplete.Models;

namespace RobotProgrammingComplete.Interpreter.Expressions
{
    /// <summary>
    /// Repräsentiert das gesamte Programm als Wurzelknoten des AST.
    /// Enthält eine Liste aller Top-Level-Anweisungen.
    /// </summary>
    public class ProgramExpression : IExpression
    {
        // Liste aller Anweisungen im Programm
        private readonly List<IExpression> statements;

        /// <summary>
        /// Erstellt ein neues Programm aus einer Liste von Anweisungen.
        /// </summary>
        /// <param name="stmts">Die Liste der auszuführenden Anweisungen</param>
        public ProgramExpression(List<IExpression> stmts)
        {
            statements = stmts;
        }

        /// <summary>
        /// Führt alle Anweisungen nacheinander aus.
        /// </summary>
        public async Task RunAsync(ExecutionContext context)
        {
            // Durchlaufe alle Anweisungen sequentiell
            foreach (var statement in statements)
            {
                await statement.RunAsync(context);  // Warte auf jede Anweisung
            }
        }
    }
}
