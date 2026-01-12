namespace RobotLanguage
{
    /// <summary>
    /// Aufgabe 5: Parser für die Robot-Sprache
    /// Die Schüler müssen die Parse-Methoden implementieren.
    /// </summary>
    public class Parser
    {
        private List<Token> _tokens;
        private int _position;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
            _position = 0;
        }

        /// <summary>
        /// Aktuelles Token
        /// </summary>
        private Token Current => _position < _tokens.Count ? _tokens[_position] : _tokens[^1];

        /// <summary>
        /// Prüft ob aktuelles Token vom angegebenen Typ ist
        /// </summary>
        private bool Check(TokenType type) => Current.Type == type;

        /// <summary>
        /// Zum nächsten Token wechseln
        /// </summary>
        private Token Advance()
        {
            Token token = Current;
            if (_position < _tokens.Count - 1)
                _position++;
            return token;
        }

        /// <summary>
        /// Erwartet ein Token vom angegebenen Typ, wirft Exception wenn nicht gefunden
        /// </summary>
        private Token Expect(TokenType type, string message)
        {
            if (Check(type))
                return Advance();
            throw new ParseException($"Zeile {Current.Line}, Spalte {Current.Column}: {message}. Gefunden: {Current.Type}");
        }

        /// <summary>
        /// Aufgabe 5: Hauptmethode - parst das gesamte Programm
        /// </summary>
        public Expression Parse()
        {
            ProgramExpression program = new ProgramExpression();

            while (!Check(TokenType.EOF))
            {
                // Newlines überspringen
                while (Check(TokenType.NEWLINE))
                    Advance();

                if (Check(TokenType.EOF))
                    break;

                Expression stmt = ParseStatement();
                program.Statements.Add(stmt);
            }

            return program;
        }

        /// <summary>
        /// Aufgabe 5: Einzelne Anweisung parsen
        /// </summary>
        private Expression ParseStatement()
        {
            if (Check(TokenType.FORWARD))
            {
                Advance();
                double distance = ParseNumber();
                return new ForwardExpression(distance);
            }
            else if (Check(TokenType.BACKWARD))
            {
                Advance();
                double distance = ParseNumber();
                return new BackwardExpression(distance);
            }
            else if (Check(TokenType.LEFT))
            {
                Advance();
                double angle = ParseNumber();
                return new LeftExpression(angle);
            }
            else if (Check(TokenType.RIGHT))
            {
                Advance();
                double angle = ParseNumber();
                return new RightExpression(angle);
            }

            throw new ParseException($"Zeile {Current.Line}: Unerwartetes Token '{Current.Value}'");
        }

        /// <summary>
        /// Aufgabe 5: Zahl parsen
        /// </summary>
        private double ParseNumber()
        {
            Token token = Expect(TokenType.NUMBER, "Zahl erwartet");
            
            if (double.TryParse(token.Value, System.Globalization.NumberStyles.Any, 
                System.Globalization.CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }
            
            throw new ParseException($"Zeile {token.Line}: Ungültige Zahl '{token.Value}'");
        }
    }

    /// <summary>
    /// Exception für Parser-Fehler
    /// </summary>
    public class ParseException : Exception
    {
        public ParseException(string message) : base(message) { }
    }
}
