using System.Collections.Generic;
using RobotProgrammingComplete.Interpreter.Expressions;
using RobotProgrammingComplete.Models;

namespace RobotProgrammingComplete.Interpreter
{
    public class Parser
    {
        private List<Token> _tokens = new();
        private int _pos = 0;
        public List<string> Errors { get; } = new();

        public ProgramExpression? Parse(List<Token> tokens)
        {
            _tokens = tokens;
            _pos = 0;
            Errors.Clear();

            var statements = new List<IExpression>();

            while (!IsAtEnd())
            {
                SkipNewlines();
                if (IsAtEnd()) break;

                var stmt = ParseStatement();
                if (stmt != null)
                {
                    statements.Add(stmt);
                }
            }

            if (Errors.Count > 0)
                return null;

            return new ProgramExpression(statements);
        }

        private IExpression? ParseStatement()
        {
            if (Check("MOVE"))
                return ParseMove();
            if (Check("COLLECT"))
                return ParseCollect();
            if (Check("REPEAT"))
                return ParseRepeat();
            if (Check("UNTIL"))
                return ParseUntil();
            if (Check("IF"))
                return ParseIf();

            var token = Current();
            AddError($"Unerwartetes Token '{token.Text}'", token.Line);
            Advance();
            return null;
        }

        private MoveExpression? ParseMove()
        {
            int line = Current().Line;
            Advance(); // MOVE

            if (!CheckKeyword("UP", "DOWN", "LEFT", "RIGHT"))
            {
                AddError("Erwartete Richtung (UP, DOWN, LEFT, RIGHT) nach MOVE", line);
                return null;
            }

            var direction = ParseDirection();
            Advance();
            return new MoveExpression(direction);
        }

        private CollectExpression ParseCollect()
        {
            Advance(); // COLLECT
            return new CollectExpression();
        }

        private RepeatExpression? ParseRepeat()
        {
            int line = Current().Line;
            Advance(); // REPEAT

            if (Current().Type != TokenType.Number)
            {
                AddError("Erwartete Zahl nach REPEAT", line);
                return null;
            }

            int count = int.Parse(Current().Text);
            Advance();

            var block = ParseBlock();
            if (block == null) return null;

            return new RepeatExpression(count, block);
        }

        private UntilExpression? ParseUntil()
        {
            int line = Current().Line;
            Advance(); // UNTIL

            var condition = ParseCondition();
            if (condition == null) return null;

            var block = ParseBlock();
            if (block == null) return null;

            return new UntilExpression(condition, block);
        }

        private IfExpression? ParseIf()
        {
            int line = Current().Line;
            Advance(); // IF

            var condition = ParseCondition();
            if (condition == null) return null;

            var block = ParseBlock();
            if (block == null) return null;

            return new IfExpression(condition, block);
        }

        private Condition? ParseCondition()
        {
            int line = Current().Line;

            if (!CheckKeyword("UP", "DOWN", "LEFT", "RIGHT"))
            {
                AddError("Erwartete Richtung in Bedingung", line);
                return null;
            }

            var direction = ParseDirection();
            Advance();

            if (!Check("IS-A"))
            {
                AddError("Erwartete IS-A in Bedingung", line);
                return null;
            }
            Advance(); // IS-A

            string targetType;
            if (Check("OBSTACLE"))
            {
                targetType = "OBSTACLE";
                Advance();
            }
            else if (Current().Type == TokenType.Letter)
            {
                targetType = Current().Text;
                Advance();
            }
            else
            {
                AddError("Erwartete OBSTACLE oder Buchstabe nach IS-A", line);
                return null;
            }

            return new Condition(direction, targetType);
        }

        private List<IExpression>? ParseBlock()
        {
            int line = Current().Line;
            SkipNewlines();

            if (Current().Type != TokenType.OpenBrace)
            {
                AddError("Erwartete '{' am Beginn des Blocks", line);
                return null;
            }
            Advance(); // {

            var statements = new List<IExpression>();

            while (!IsAtEnd() && Current().Type != TokenType.CloseBrace)
            {
                SkipNewlines();
                if (Current().Type == TokenType.CloseBrace) break;

                var stmt = ParseStatement();
                if (stmt != null)
                {
                    statements.Add(stmt);
                }
                SkipNewlines();
            }

            if (Current().Type != TokenType.CloseBrace)
            {
                AddError("Erwartete '}' am Ende des Blocks", line);
                return null;
            }
            Advance(); // }

            return statements;
        }

        private Direction ParseDirection()
        {
            return Current().Text switch
            {
                "UP" => Direction.Up,
                "DOWN" => Direction.Down,
                "LEFT" => Direction.Left,
                "RIGHT" => Direction.Right,
                _ => Direction.Up
            };
        }

        private void SkipNewlines()
        {
            while (!IsAtEnd() && Current().Type == TokenType.Newline)
                Advance();
        }

        private bool Check(string text) => !IsAtEnd() && Current().Text == text;

        private bool CheckKeyword(params string[] keywords)
        {
            if (IsAtEnd()) return false;
            foreach (var kw in keywords)
                if (Current().Text == kw) return true;
            return false;
        }

        private Token Current()
        {
            if (_pos >= _tokens.Count)
                return new Token("EOF", TokenType.Error, _tokens.Count > 0 ? _tokens[^1].Line : 1);
            return _tokens[_pos];
        }

        private void Advance()
        {
            if (_pos < _tokens.Count) _pos++;
        }

        private bool IsAtEnd() => _pos >= _tokens.Count;

        private void AddError(string message, int line)
        {
            Errors.Add($"Zeile {line}: {message}");
        }
    }
}
