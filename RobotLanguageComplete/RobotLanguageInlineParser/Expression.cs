using System.Globalization;
using System.Windows;

namespace RobotLanguage
{
    /// <summary>
    /// Exception für Parser-Fehler
    /// </summary>
    public class ParseException : Exception
    {
        public ParseException(string message) : base(message) { }
    }

    /// <summary>
    /// Basisklasse für alle Ausdrücke im Syntax-Baum
    /// </summary>
    public abstract class Expression
    {
        /// <summary>
        /// Parst die Token-Liste und entfernt verarbeitete Tokens
        /// </summary>
        public abstract void Parse(ref List<Token> tokenList);

        /// <summary>
        /// Führt den Ausdruck aus
        /// </summary>
        public abstract void Execute(RobotCanvas canvas);

        /// <summary>
        /// Hilfsmethode: Parst eine Zahl und entfernt das Token
        /// </summary>
        protected double ParseNumber(ref List<Token> tokenList)
        {
            if (tokenList.Count == 0 || tokenList[0].Type != TokenType.NUMBER)
            {
                throw new ParseException("Zahl erwartet");
            }

            Token token = tokenList[0];
            tokenList.RemoveAt(0);

            if (double.TryParse(token.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }

            throw new ParseException($"Ungültige Zahl '{token.Value}'");
        }

        /// <summary>
        /// Hilfsmethode: Überspringt Newline-Tokens
        /// </summary>
        protected void SkipNewlines(ref List<Token> tokenList)
        {
            while (tokenList.Count > 0 && tokenList[0].Type == TokenType.NEWLINE)
            {
                tokenList.RemoveAt(0);
            }
        }
    }

    /// <summary>
    /// Programm - enthält eine Liste von Anweisungen
    /// </summary>
    public class ProgramExpression : Expression
    {
        public List<Expression> Statements { get; } = new List<Expression>();

        public override void Parse(ref List<Token> tokenList)
        {
            while (tokenList.Count > 0)
            {
                SkipNewlines(ref tokenList);

                if (tokenList.Count == 0 || tokenList[0].Type == TokenType.EOF)
                    break;

                // Je nach Token-Typ die richtige Expression erstellen und parsen
                if (tokenList[0].Type == TokenType.FORWARD)
                {
                    ForwardExpression expr = new ForwardExpression();
                    expr.Parse(ref tokenList);
                    Statements.Add(expr);
                }
                else if (tokenList[0].Type == TokenType.BACKWARD)
                {
                    BackwardExpression expr = new BackwardExpression();
                    expr.Parse(ref tokenList);
                    Statements.Add(expr);
                }
                else if (tokenList[0].Type == TokenType.LEFT)
                {
                    LeftExpression expr = new LeftExpression();
                    expr.Parse(ref tokenList);
                    Statements.Add(expr);
                }
                else if (tokenList[0].Type == TokenType.RIGHT)
                {
                    RightExpression expr = new RightExpression();
                    expr.Parse(ref tokenList);
                    Statements.Add(expr);
                }
                else
                {
                    throw new ParseException($"Unerwartetes Token '{tokenList[0].Value}'");
                }
            }
        }

        public override void Execute(RobotCanvas canvas)
        {
            foreach (var statement in Statements)
            {
                statement.Execute(canvas);
            }
        }
    }

    /// <summary>
    /// FORWARD-Befehl
    /// </summary>
    public class ForwardExpression : Expression
    {
        public double Distance { get; set; }

        public override void Parse(ref List<Token> tokenList)
        {
            // FORWARD Token konsumieren
            if (tokenList.Count > 0 && tokenList[0].Type == TokenType.FORWARD)
            {
                tokenList.RemoveAt(0);
            }

            // Distanz parsen
            Distance = ParseNumber(ref tokenList);
        }

        public override void Execute(RobotCanvas canvas)
        {
            double angleRad = canvas.CurrentAngle * Math.PI / 180;
            double newX = canvas.CurrentPosition.X + Distance * Math.Cos(angleRad);
            double newY = canvas.CurrentPosition.Y + Distance * Math.Sin(angleRad);
            Point newPos = new Point(newX, newY);
            canvas.AddLine(canvas.CurrentPosition, newPos);
            canvas.CurrentPosition = newPos;
        }
    }

    /// <summary>
    /// BACKWARD-Befehl
    /// </summary>
    public class BackwardExpression : Expression
    {
        public double Distance { get; set; }

        public override void Parse(ref List<Token> tokenList)
        {
            // BACKWARD Token konsumieren
            if (tokenList.Count > 0 && tokenList[0].Type == TokenType.BACKWARD)
            {
                tokenList.RemoveAt(0);
            }

            // Distanz parsen
            Distance = ParseNumber(ref tokenList);
        }

        public override void Execute(RobotCanvas canvas)
        {
            double angleRad = canvas.CurrentAngle * Math.PI / 180;
            double newX = canvas.CurrentPosition.X - Distance * Math.Cos(angleRad);
            double newY = canvas.CurrentPosition.Y - Distance * Math.Sin(angleRad);
            Point newPos = new Point(newX, newY);
            canvas.AddLine(canvas.CurrentPosition, newPos);
            canvas.CurrentPosition = newPos;
        }
    }

    /// <summary>
    /// LEFT-Befehl (nach links drehen)
    /// </summary>
    public class LeftExpression : Expression
    {
        public double Angle { get; set; }

        public override void Parse(ref List<Token> tokenList)
        {
            // LEFT Token konsumieren
            if (tokenList.Count > 0 && tokenList[0].Type == TokenType.LEFT)
            {
                tokenList.RemoveAt(0);
            }

            // Winkel parsen
            Angle = ParseNumber(ref tokenList);
        }

        public override void Execute(RobotCanvas canvas)
        {
            canvas.CurrentAngle -= Angle;
        }
    }

    /// <summary>
    /// RIGHT-Befehl (nach rechts drehen)
    /// </summary>
    public class RightExpression : Expression
    {
        public double Angle { get; set; }

        public override void Parse(ref List<Token> tokenList)
        {
            // RIGHT Token konsumieren
            if (tokenList.Count > 0 && tokenList[0].Type == TokenType.RIGHT)
            {
                tokenList.RemoveAt(0);
            }

            // Winkel parsen
            Angle = ParseNumber(ref tokenList);
        }

        public override void Execute(RobotCanvas canvas)
        {
            canvas.CurrentAngle += Angle;
        }
    }
}
