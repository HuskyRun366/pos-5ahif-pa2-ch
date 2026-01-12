using System.Collections.Generic;
using System.Text;

namespace RobotProgrammingComplete.Interpreter
{
    public class Lexer
    {
        private static readonly HashSet<string> Keywords = new()
        {
            "MOVE", "COLLECT", "REPEAT", "UNTIL", "IF",
            "IS-A", "UP", "DOWN", "LEFT", "RIGHT", "OBSTACLE"
        };

        public List<Token> Tokenize(string input)
        {
            var tokens = new List<Token>();
            int pos = 0;
            int line = 1;

            while (pos < input.Length)
            {
                char c = input[pos];

                if (c == ' ' || c == '\t' || c == '\r')
                {
                    pos++;
                    continue;
                }

                if (c == '\n')
                {
                    tokens.Add(new Token("\n", TokenType.Newline, line));
                    line++;
                    pos++;
                    continue;
                }

                if (c == '{')
                {
                    tokens.Add(new Token("{", TokenType.OpenBrace, line));
                    pos++;
                    continue;
                }

                if (c == '}')
                {
                    tokens.Add(new Token("}", TokenType.CloseBrace, line));
                    pos++;
                    continue;
                }

                if (char.IsDigit(c))
                {
                    var sb = new StringBuilder();
                    while (pos < input.Length && char.IsDigit(input[pos]))
                    {
                        sb.Append(input[pos]);
                        pos++;
                    }
                    tokens.Add(new Token(sb.ToString(), TokenType.Number, line));
                    continue;
                }

                if (char.IsLetter(c) || c == '-')
                {
                    var sb = new StringBuilder();
                    while (pos < input.Length && (char.IsLetter(input[pos]) || input[pos] == '-'))
                    {
                        sb.Append(input[pos]);
                        pos++;
                    }
                    string word = sb.ToString().ToUpper();

                    if (Keywords.Contains(word))
                    {
                        tokens.Add(new Token(word, TokenType.Keyword, line));
                    }
                    else if (word.Length == 1 && char.IsLetter(word[0]))
                    {
                        tokens.Add(new Token(word, TokenType.Letter, line));
                    }
                    else
                    {
                        tokens.Add(new Token(word, TokenType.Error, line));
                    }
                    continue;
                }

                tokens.Add(new Token(c.ToString(), TokenType.Error, line));
                pos++;
            }

            return tokens;
        }
    }
}
