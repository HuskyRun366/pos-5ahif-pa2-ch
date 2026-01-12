namespace RobotProgrammingComplete.Interpreter
{
    /// <summary>
    /// Aufzählung aller möglichen Token-Typen.
    /// Der Lexer weist jedem erkannten Token einen dieser Typen zu.
    /// </summary>
    public enum TokenType
    {
        /// <summary>Schlüsselwort der Sprache (MOVE, REPEAT, IF, etc.)</summary>
        Keyword,

        /// <summary>Einzelner Buchstabe (A-Z) für Sammel-Ziele</summary>
        Letter,

        /// <summary>Ganzzahl (für REPEAT-Anzahl)</summary>
        Number,

        /// <summary>Öffnende geschweifte Klammer '{'</summary>
        OpenBrace,

        /// <summary>Schließende geschweifte Klammer '}'</summary>
        CloseBrace,

        /// <summary>Zeilenumbruch (wird für die Zeilenzählung benötigt)</summary>
        Newline,

        /// <summary>Fehlerhafte/unbekannte Eingabe</summary>
        Error
    }

    /// <summary>
    /// Repräsentiert ein einzelnes Token aus der lexikalischen Analyse.
    /// Ein Token ist die kleinste bedeutungsvolle Einheit im Quellcode.
    /// </summary>
    public class Token
    {
        /// <summary>Der ursprüngliche Text des Tokens (z.B. "MOVE", "5", "{")</summary>
        public string Text { get; set; }

        /// <summary>Der Typ des Tokens (Keyword, Number, etc.)</summary>
        public TokenType Type { get; set; }

        /// <summary>Die Zeilennummer im Quellcode (für Fehlermeldungen)</summary>
        public int Line { get; set; }

        /// <summary>
        /// Erstellt ein neues Token.
        /// </summary>
        /// <param name="text">Der Textinhalt des Tokens</param>
        /// <param name="type">Der Token-Typ</param>
        /// <param name="line">Die Zeilennummer im Quellcode</param>
        public Token(string text, TokenType type, int line)
        {
            Text = text;   // Speichere den Original-Text
            Type = type;   // Speichere den erkannten Typ
            Line = line;   // Speichere die Zeilennummer für Fehlermeldungen
        }
    }
}
