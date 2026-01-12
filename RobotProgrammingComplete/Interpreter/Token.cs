namespace RobotProgrammingComplete.Interpreter
{
    public enum TokenType
    {
        Keyword,
        Letter,
        Number,
        OpenBrace,
        CloseBrace,
        Newline,
        Error
    }

    public class Token
    {
        public string Text { get; set; }
        public TokenType Type { get; set; }
        public int Line { get; set; }

        public Token(string text, TokenType type, int line)
        {
            Text = text;
            Type = type;
            Line = line;
        }
    }
}
