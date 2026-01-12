using System.Collections.Generic;
using RobotProgrammingComplete.Interpreter.Expressions;
using RobotProgrammingComplete.Models;

namespace RobotProgrammingComplete.Interpreter
{
    /// <summary>
    /// Syntaktische Analyse (Parsing) des Roboter-Programmcodes.
    /// Baut aus der Token-Liste einen abstrakten Syntaxbaum (AST) auf.
    /// Verwendet rekursiven Abstieg (Recursive Descent Parsing).
    /// </summary>
    public class Parser
    {
        // Die Liste der zu parsenden Tokens (vom Lexer erzeugt)
        private List<Token> tokens = new();

        // Aktuelle Position in der Token-Liste
        private int pos = 0;

        /// <summary>Liste aller Fehler, die während des Parsens auftreten</summary>
        public List<string> Errors { get; } = new();

        /// <summary>
        /// Gibt das aktuelle Token zurück, ohne die Position zu verändern.
        /// Liefert ein EOF-Token wenn das Ende erreicht ist.
        /// </summary>
        private Token? Current => pos < tokens.Count ? tokens[pos] : null;

        /// <summary>
        /// Parst den gesamten Quellcode und erstellt den AST.
        /// </summary>
        /// <param name="tokenList">Die Token-Liste vom Lexer</param>
        /// <returns>Das Programm als AST, oder null bei Fehlern</returns>
        public ProgramExpression? Parse(List<Token> tokenList)
        {
            // Initialisiere den Parser-Zustand
            tokens = tokenList;
            pos = 0;
            Errors.Clear();

            // Sammle alle geparsten Anweisungen
            var statements = new List<IExpression>();

            // Verarbeite alle Tokens bis zum Ende
            while (!IsAtEnd())
            {
                SkipNewlines();  // Überspringe leere Zeilen

                if (IsAtEnd()) break;  // Prüfe nochmal nach Zeilenumbrüchen

                // Parse die nächste Anweisung
                var stmt = ParseStatement();
                if (stmt != null)
                {
                    statements.Add(stmt);
                }
            }

            // Bei Fehlern kein Programm zurückgeben
            if (Errors.Count > 0)
                return null;

            // Erstelle das Programm-Objekt mit allen Anweisungen
            return new ProgramExpression(statements);
        }

        /// <summary>
        /// Parst eine einzelne Anweisung anhand des aktuellen Tokens.
        /// Dispatch zu den spezifischen Parse-Methoden.
        /// </summary>
        private IExpression? ParseStatement()
        {
            // MOVE-Befehl: Roboter bewegen
            if (Check("MOVE"))
                return ParseMove();

            // COLLECT-Befehl: Buchstabe einsammeln
            if (Check("COLLECT"))
                return ParseCollect();

            // REPEAT-Schleife: Anweisungen n-mal wiederholen
            if (Check("REPEAT"))
                return ParseRepeat();

            // UNTIL-Schleife: Anweisungen bis Bedingung wiederholen
            if (Check("UNTIL"))
                return ParseUntil();

            // IF-Anweisung: Bedingte Ausführung
            if (Check("IF"))
                return ParseIf();

            // Unbekanntes Token = Fehler
            var token = Current;
            AddError($"Unerwartetes Token '{token?.Text ?? "EOF"}'", token?.Line ?? 0);
            Advance();  // Token überspringen um Endlosschleife zu vermeiden
            return null;
        }

        /// <summary>
        /// Parst einen MOVE-Befehl: MOVE direction
        /// </summary>
        private MoveExpression? ParseMove()
        {
            int line = Current?.Line ?? 0;
            Advance();  // MOVE-Token konsumieren

            // Nach MOVE muss eine Richtung folgen
            if (!CheckKeyword("UP", "DOWN", "LEFT", "RIGHT"))
            {
                AddError("Erwartete Richtung (UP, DOWN, LEFT, RIGHT) nach MOVE", line);
                return null;
            }

            // Richtung parsen und Token konsumieren
            var direction = ParseDirection();
            Advance();

            return new MoveExpression(direction);
        }

        /// <summary>
        /// Parst einen COLLECT-Befehl: COLLECT
        /// </summary>
        private CollectExpression ParseCollect()
        {
            Advance();  // COLLECT-Token konsumieren
            return new CollectExpression();
        }

        /// <summary>
        /// Parst eine REPEAT-Schleife: REPEAT n { ... }
        /// </summary>
        private RepeatExpression? ParseRepeat()
        {
            int line = Current?.Line ?? 0;
            Advance();  // REPEAT-Token konsumieren

            // Nach REPEAT muss eine Zahl folgen
            if (Current?.Type != TokenType.Number)
            {
                AddError("Erwartete Zahl nach REPEAT", line);
                return null;
            }

            // Anzahl der Wiederholungen parsen
            int count = int.Parse(Current.Text);
            Advance();

            // Block mit Anweisungen parsen
            var block = ParseBlock();
            if (block == null) return null;

            return new RepeatExpression(count, block);
        }

        /// <summary>
        /// Parst eine UNTIL-Schleife: UNTIL condition { ... }
        /// </summary>
        private UntilExpression? ParseUntil()
        {
            int line = Current?.Line ?? 0;
            Advance();  // UNTIL-Token konsumieren

            // Bedingung parsen
            var condition = ParseCondition();
            if (condition == null) return null;

            // Block mit Anweisungen parsen
            var block = ParseBlock();
            if (block == null) return null;

            return new UntilExpression(condition, block);
        }

        /// <summary>
        /// Parst eine IF-Anweisung: IF condition { ... }
        /// </summary>
        private IfExpression? ParseIf()
        {
            int line = Current?.Line ?? 0;
            Advance();  // IF-Token konsumieren

            // Bedingung parsen
            var condition = ParseCondition();
            if (condition == null) return null;

            // Block mit Anweisungen parsen
            var block = ParseBlock();
            if (block == null) return null;

            return new IfExpression(condition, block);
        }

        /// <summary>
        /// Parst eine Bedingung: direction IS-A target
        /// Beispiel: UP IS-A OBSTACLE oder LEFT IS-A A
        /// </summary>
        private Condition? ParseCondition()
        {
            int line = Current?.Line ?? 0;

            // Zuerst muss eine Richtung kommen
            if (!CheckKeyword("UP", "DOWN", "LEFT", "RIGHT"))
            {
                AddError("Erwartete Richtung in Bedingung", line);
                return null;
            }

            // Richtung parsen
            var direction = ParseDirection();
            Advance();

            // Dann muss IS-A kommen
            if (!Check("IS-A"))
            {
                AddError("Erwartete IS-A in Bedingung", line);
                return null;
            }
            Advance();  // IS-A konsumieren

            // Schließlich das Ziel: OBSTACLE oder ein Buchstabe
            string targetType;
            if (Check("OBSTACLE"))
            {
                targetType = "OBSTACLE";
                Advance();
            }
            else if (Current?.Type == TokenType.Letter)
            {
                targetType = Current.Text;
                Advance();
            }
            else
            {
                AddError("Erwartete OBSTACLE oder Buchstabe nach IS-A", line);
                return null;
            }

            return new Condition(direction, targetType);
        }

        /// <summary>
        /// Parst einen Block von Anweisungen: { statement* }
        /// </summary>
        private List<IExpression>? ParseBlock()
        {
            int line = Current?.Line ?? 0;
            SkipNewlines();  // Zeilenumbrüche vor der Klammer überspringen

            // Block muss mit '{' beginnen
            if (Current?.Type != TokenType.OpenBrace)
            {
                AddError("Erwartete '{' am Beginn des Blocks", line);
                return null;
            }
            Advance();  // '{' konsumieren

            var statements = new List<IExpression>();

            // Anweisungen sammeln bis '}' erreicht wird
            while (!IsAtEnd() && Current?.Type != TokenType.CloseBrace)
            {
                SkipNewlines();

                // Nochmal prüfen nach Zeilenumbrüchen
                if (Current?.Type == TokenType.CloseBrace) break;

                // Anweisung parsen
                var stmt = ParseStatement();
                if (stmt != null)
                {
                    statements.Add(stmt);
                }
                SkipNewlines();
            }

            // Block muss mit '}' enden
            if (Current?.Type != TokenType.CloseBrace)
            {
                AddError("Erwartete '}' am Ende des Blocks", line);
                return null;
            }
            Advance();  // '}' konsumieren

            return statements;
        }

        /// <summary>
        /// Wandelt das aktuelle Token in eine Direction um.
        /// </summary>
        private Direction ParseDirection()
        {
            // Switch-Expression für die Richtungs-Zuordnung
            return Current?.Text switch
            {
                "UP" => Direction.Up,
                "DOWN" => Direction.Down,
                "LEFT" => Direction.Left,
                "RIGHT" => Direction.Right,
                _ => Direction.Up  // Fallback (sollte nie erreicht werden)
            };
        }

        /// <summary>
        /// Überspringt alle aufeinanderfolgenden Zeilenumbruch-Tokens.
        /// </summary>
        private void SkipNewlines()
        {
            while (!IsAtEnd() && Current?.Type == TokenType.Newline)
                Advance();
        }

        /// <summary>
        /// Prüft ob das aktuelle Token den angegebenen Text hat.
        /// </summary>
        private bool Check(string text) => !IsAtEnd() && Current?.Text == text;

        /// <summary>
        /// Prüft ob das aktuelle Token eines der angegebenen Schlüsselwörter ist.
        /// </summary>
        private bool CheckKeyword(params string[] keywords)
        {
            if (IsAtEnd()) return false;

            // Prüfe jedes Schlüsselwort
            foreach (var kw in keywords)
                if (Current?.Text == kw) return true;

            return false;
        }

        /// <summary>
        /// Bewegt die Position zum nächsten Token.
        /// </summary>
        private void Advance()
        {
            if (pos < tokens.Count) pos++;
        }

        /// <summary>
        /// Prüft ob das Ende der Token-Liste erreicht ist.
        /// </summary>
        private bool IsAtEnd() => pos >= tokens.Count;

        /// <summary>
        /// Fügt eine Fehlermeldung zur Fehlerliste hinzu.
        /// </summary>
        private void AddError(string message, int line)
        {
            Errors.Add($"Zeile {line}: {message}");
        }
    }
}
