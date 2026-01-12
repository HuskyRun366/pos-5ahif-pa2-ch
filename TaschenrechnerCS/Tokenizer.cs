using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace TaschenrechnerCS
{
    public enum TokenTypes
    {
        Number,
        Plus,
        Minus,
        Divide,
        Multiply,
        Power,
        Variables,
        BracketOpen,
        BracketClose,
        Equals
    }

    class Tokenizer
    {
        private List<Token> tokens = new List<Token>();
        public String unknownTK;

        public Tokenizer(string input) 
        {
            foreach (Match m in Regex.Matches(input, @"\d+\,?\d*"))
            {
                Token tok = new Token(m.Index, m.Value, TokenTypes.Number);
                tokens.Add(tok);
            }

            foreach (Match m in Regex.Matches(input, @"\+"))
            {
                Token tok = new Token(m.Index, m.Value, TokenTypes.Plus);
                tokens.Add(tok);
            }

            foreach (Match m in Regex.Matches(input, @"\-"))
            {
                Token tok = new Token(m.Index, m.Value, TokenTypes.Minus);
                tokens.Add(tok);
            }

            foreach (Match m in Regex.Matches(input, @"\/"))
            {
                Token tok = new Token(m.Index, m.Value, TokenTypes.Divide);
                tokens.Add(tok);
            }

            foreach (Match m in Regex.Matches(input, @"\*"))
            {
                Token tok = new Token(m.Index, m.Value, TokenTypes.Multiply);
                tokens.Add(tok);
            }

            foreach (Match m in Regex.Matches(input, @"\("))
            {
                Token tok = new Token(m.Index, m.Value, TokenTypes.BracketOpen);
                tokens.Add(tok);
            }

            foreach (Match m in Regex.Matches(input, @"\)"))
            {
                Token tok = new Token(m.Index, m.Value, TokenTypes.BracketClose);
                tokens.Add(tok);
            }

            foreach (Match m in Regex.Matches(input, @"\^"))
            {
                Token tok = new Token(m.Index, m.Value, TokenTypes.Power);
                tokens.Add(tok);
            }

            foreach (Match m in Regex.Matches(input, @"[a-z]"))
            {
                Token tok = new Token(m.Index, m.Value, TokenTypes.Variables);
                tokens.Add(tok);
            }

            foreach (Match m in Regex.Matches(input, @"\="))
            {
                Token tok = new Token(m.Index, m.Value, TokenTypes.Equals);
                tokens.Add(tok);
            }

            MatchCollection invalidTokens = Regex.Matches(input, @"[^\d\,\+\=\-\/\*\(\)\^a-z]");
            if (invalidTokens.Count > 0)
            {
                string unknownTokens = string.Join("", invalidTokens.Select(m => m.Value));
                MessageBox.Show("Eingabe: " + input + "\n Unbekannte Tokens: " + unknownTokens);
                unknownTK = unknownTokens;
                tokens.Clear();
            }
        }

        public List<Token> getListOrderedByIndex ()
        {
            return [.. tokens.OrderBy(tok =>  tok.Index)];
        }
    }
}
