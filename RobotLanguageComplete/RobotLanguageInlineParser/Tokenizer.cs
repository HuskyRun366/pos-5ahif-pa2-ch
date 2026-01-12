using System.Text.RegularExpressions;

namespace RobotLanguage
{
    /// <summary>
    /// Aufgabe 4: Tokenizer für die Robot-Sprache
    /// Die Schüler müssen die regulären Ausdrücke vervollständigen.
    /// </summary>
    public class Tokenizer
    {
        private string _input = "";
        private int _position = 0;
        private int _line = 1;
        private int _column = 1;

        #region Aufgabe 4: Reguläre Ausdrücke definieren

        // TODO: Aufgabe 4 - Reguläre Ausdrücke für die Token-Typen erstellen
        // Hinweis: Verwende Regex mit IgnoreCase für Befehle

        // Beispiel (bereits implementiert):
        private static readonly Regex WhitespaceRegex = new Regex(@"^[ \t]+", RegexOptions.Compiled);

        private static readonly Regex ForwardRegex = new Regex(@"^FORWARD\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex BackwardRegex = new Regex(@"^BACKWARD\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex LeftRegex = new Regex(@"^LEFT\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RightRegex = new Regex(@"^RIGHT\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex NumberRegex = new Regex(@"^[0-9]+(\.[0-9]+)?", RegexOptions.Compiled);
        private static readonly Regex NewlineRegex = new Regex(@"^(\r\n|\r|\n)", RegexOptions.Compiled);

        #endregion

        /// <summary>
        /// Tokenisiert den Eingabe-String
        /// </summary>
        public List<Token> Tokenize(string input)
        {
            _input = input;
            _position = 0;
            _line = 1;
            _column = 1;

            List<Token> tokens = new List<Token>();

            while (_position < _input.Length)
            {
                Token? token = NextToken();
                if (token != null)
                {
                    tokens.Add(token);
                }
            }

            tokens.Add(new Token(TokenType.EOF, "", _line, _column));
            return tokens;
        }

        /// <summary>
        /// Aufgabe 4: Nächstes Token lesen
        /// Die Schüler müssen diese Methode vervollständigen.
        /// </summary>
        private Token? NextToken()
        {
            if (_position >= _input.Length)
                return null;

            string remaining = _input.Substring(_position);

            // Whitespace überspringen (außer Newlines)
            Match whitespaceMatch = WhitespaceRegex.Match(remaining);
            if (whitespaceMatch.Success)
            {
                Advance(whitespaceMatch.Length);
                return NextToken();
            }

            // NEWLINE
            Match newlineMatch = NewlineRegex.Match(remaining);
            if (newlineMatch.Success)
            {
                Token token = new Token(TokenType.NEWLINE, newlineMatch.Value, _line, _column);
                Advance(newlineMatch.Length);
                return token;
            }

            // FORWARD
            Match forwardMatch = ForwardRegex.Match(remaining);
            if (forwardMatch.Success)
            {
                Token token = new Token(TokenType.FORWARD, forwardMatch.Value, _line, _column);
                Advance(forwardMatch.Length);
                return token;
            }

            // BACKWARD
            Match backwardMatch = BackwardRegex.Match(remaining);
            if (backwardMatch.Success)
            {
                Token token = new Token(TokenType.BACKWARD, backwardMatch.Value, _line, _column);
                Advance(backwardMatch.Length);
                return token;
            }

            // LEFT
            Match leftMatch = LeftRegex.Match(remaining);
            if (leftMatch.Success)
            {
                Token token = new Token(TokenType.LEFT, leftMatch.Value, _line, _column);
                Advance(leftMatch.Length);
                return token;
            }

            // RIGHT
            Match rightMatch = RightRegex.Match(remaining);
            if (rightMatch.Success)
            {
                Token token = new Token(TokenType.RIGHT, rightMatch.Value, _line, _column);
                Advance(rightMatch.Length);
                return token;
            }

            // NUMBER
            Match numberMatch = NumberRegex.Match(remaining);
            if (numberMatch.Success)
            {
                Token token = new Token(TokenType.NUMBER, numberMatch.Value, _line, _column);
                Advance(numberMatch.Length);
                return token;
            }

            // Unbekanntes Zeichen
            Token unknownToken = new Token(TokenType.UNKNOWN, remaining[0].ToString(), _line, _column);
            Advance(1);
            return unknownToken;
        }

        /// <summary>
        /// Position im Input vorwärts bewegen
        /// Diese Methode ist bereits vorbereitet.
        /// </summary>
        private void Advance(int count)
        {
            for (int i = 0; i < count && _position < _input.Length; i++)
            {
                if (_input[_position] == '\n')
                {
                    _line++;
                    _column = 1;
                }
                else
                {
                    _column++;
                }
                _position++;
            }
        }
    }
}
