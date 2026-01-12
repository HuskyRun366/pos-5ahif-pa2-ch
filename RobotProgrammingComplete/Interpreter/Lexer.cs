using System.Collections.Generic;
using System.Text;

namespace RobotProgrammingComplete.Interpreter
{
    /// <summary>
    /// Lexikalische Analyse (Tokenisierung) des Roboter-Programmcodes.
    /// Wandelt den Quelltext zeichenweise in eine Liste von Tokens um.
    /// Dies ist der erste Schritt der Programmverarbeitung.
    /// </summary>
    public class Lexer
    {
        // Liste aller gültigen Schlüsselwörter der Roboter-Sprache
        // Diese Wörter haben eine spezielle Bedeutung und werden als Keyword-Tokens erkannt
        private static readonly HashSet<string> Keywords = new()
        {
            "MOVE",      // Bewegungsbefehl
            "COLLECT",   // Buchstabe einsammeln
            "REPEAT",    // Wiederholungsschleife
            "UNTIL",     // Schleife bis Bedingung erfüllt
            "IF",        // Bedingte Anweisung
            "IS-A",      // Vergleichsoperator für Bedingungen
            "UP",        // Richtung: nach oben
            "DOWN",      // Richtung: nach unten
            "LEFT",      // Richtung: nach links
            "RIGHT",     // Richtung: nach rechts
            "OBSTACLE"   // Zieltyp: Hindernis
        };

        /// <summary>
        /// Zerlegt den Eingabetext in einzelne Tokens.
        /// </summary>
        /// <param name="input">Der zu analysierende Quelltext</param>
        /// <returns>Liste aller erkannten Tokens in der Reihenfolge ihres Auftretens</returns>
        public List<Token> Tokenize(string input)
        {
            var tokens = new List<Token>();  // Ergebnisliste für alle gefundenen Tokens
            int pos = 0;                      // Aktuelle Leseposition im Text
            int line = 1;                     // Aktuelle Zeilennummer für Fehlermeldungen

            // Durchlaufe den gesamten Eingabetext Zeichen für Zeichen
            while (pos < input.Length)
            {
                char c = input[pos];  // Aktuelles Zeichen holen

                // === Whitespace ignorieren ===
                // Leerzeichen, Tabs und Wagenrücklauf überspringen
                if (c == ' ' || c == '\t' || c == '\r')
                {
                    pos++;  // Zum nächsten Zeichen springen
                    continue;
                }

                // === Zeilenumbruch verarbeiten ===
                // Zeilenumbrüche sind wichtig für die Zeilenzählung in Fehlermeldungen
                if (c == '\n')
                {
                    tokens.Add(new Token("\n", TokenType.Newline, line));
                    line++;  // Zeilenzähler erhöhen
                    pos++;
                    continue;
                }

                // === Öffnende Klammer ===
                if (c == '{')
                {
                    tokens.Add(new Token("{", TokenType.OpenBrace, line));
                    pos++;
                    continue;
                }

                // === Schließende Klammer ===
                if (c == '}')
                {
                    tokens.Add(new Token("}", TokenType.CloseBrace, line));
                    pos++;
                    continue;
                }

                // === Zahlen erkennen ===
                // Sammle alle aufeinanderfolgenden Ziffern
                if (char.IsDigit(c))
                {
                    var sb = new StringBuilder();  // StringBuilder für effiziente Zeichenverkettung

                    // Lies alle aufeinanderfolgenden Ziffern
                    while (pos < input.Length && char.IsDigit(input[pos]))
                    {
                        sb.Append(input[pos]);
                        pos++;
                    }

                    tokens.Add(new Token(sb.ToString(), TokenType.Number, line));
                    continue;
                }

                // === Wörter erkennen (Schlüsselwörter oder Buchstaben) ===
                // Auch Bindestriche erlauben (für IS-A)
                if (char.IsLetter(c) || c == '-')
                {
                    var sb = new StringBuilder();

                    // Lies alle aufeinanderfolgenden Buchstaben und Bindestriche
                    while (pos < input.Length && (char.IsLetter(input[pos]) || input[pos] == '-'))
                    {
                        sb.Append(input[pos]);
                        pos++;
                    }

                    // Wandle in Großbuchstaben um (Sprache ist case-insensitive)
                    string word = sb.ToString().ToUpper();

                    // Prüfe ob das Wort ein Schlüsselwort ist
                    if (Keywords.Contains(word))
                    {
                        tokens.Add(new Token(word, TokenType.Keyword, line));
                    }
                    // Einzelne Buchstaben sind Sammel-Ziele (z.B. "A", "B", "C")
                    else if (word.Length == 1 && char.IsLetter(word[0]))
                    {
                        tokens.Add(new Token(word, TokenType.Letter, line));
                    }
                    // Unbekanntes Wort = Fehler
                    else
                    {
                        tokens.Add(new Token(word, TokenType.Error, line));
                    }
                    continue;
                }

                // === Unbekanntes Zeichen = Fehler ===
                tokens.Add(new Token(c.ToString(), TokenType.Error, line));
                pos++;
            }

            return tokens;
        }
    }
}
