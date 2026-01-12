namespace RobotLanguage
{
    /// <summary>
    /// Aufgabe 4: Token-Typen für die Robot-Sprache
    /// </summary>
    public enum TokenType
    {
        // Befehle
        FORWARD,    // Vorwärts bewegen
        BACKWARD,   // Rückwärts bewegen
        LEFT,       // Nach links drehen
        RIGHT,      // Nach rechts drehen
        
        // Kontrollstrukturen (optional, für Erweiterung)
        REPEAT,     // Wiederholung
        
        // Werte
        NUMBER,     // Zahlenwert
        
        // Struktur
        NEWLINE,    // Zeilenumbruch
        EOF,        // Ende der Eingabe
        
        // Fehler
        UNKNOWN     // Unbekanntes Token
    }

    /// <summary>
    /// Token-Klasse: Repräsentiert ein einzelnes Token
    /// </summary>
    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }

        public Token(TokenType type, string value, int line = 0, int column = 0)
        {
            Type = type;
            Value = value;
            Line = line;
            Column = column;
        }

        public override string ToString()
        {
            return $"[{Type}: '{Value}' at {Line}:{Column}]";
        }
    }
}
