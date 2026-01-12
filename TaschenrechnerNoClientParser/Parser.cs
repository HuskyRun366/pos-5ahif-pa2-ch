using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Automation.Provider;
using TaschenrechnerCS.Nodes;

namespace TaschenrechnerCS
{
    internal class Parser
    {
        List<Token> tokenList;

        private Token? Current => pos < tokenList.Count ? tokenList[pos] : null;

        private int pos = 0;

        private Dictionary<String, Double> variableDict = [];
    

        public Parser(List<Token> tkL, Dictionary<String, Double> variables)
        {
            tokenList = tkL;
            variableDict = variables ?? new Dictionary<String, Double>();
        }

        private Token Consume()
        {
            
            return tokenList[pos++];
        }

        private bool Match(TokenTypes type)
        {
            return Current != null && Current.ToType == type;
        }

        public IExpression ParseExpression()
        {
            IExpression left = ParseTerm();
            while (Match(TokenTypes.Plus) || Match(TokenTypes.Minus))
            {
                TokenTypes operationType = Consume().ToType;
                IExpression right = ParseTerm();

                if (operationType == TokenTypes.Plus)
                {
                    left = new PlusNode(left, right);
                } else
                {
                    left = new MinusNode(left, right);
                }
            }
            return left;
        }

        private IExpression ParseTerm()
        {
            IExpression left = ParsePower();

            while (Match(TokenTypes.Divide) || Match(TokenTypes.Multiply))
            {
                TokenTypes operationType = Consume().ToType;
                IExpression right = ParsePower();

                if (operationType == TokenTypes.Divide)
                {
                    left = new DivNode(left, right);
                }
                else
                {
                    left = new MultNode(left, right);
                }
            }
            return left;
        }

        private IExpression ParsePower()
        {
            IExpression left = ParseFactor();

            if(Match(TokenTypes.Power))
            {
                Consume();
                IExpression right = ParsePower();
                return new PowerNode(left, right);
            }
            return left;
        }

        private IExpression ParseFactor()
        {
            if (Match(TokenTypes.Number))
            {
                string text = Consume().Value;
                text = text.Replace(',', '.');
                double temp = double.Parse(text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
                return new NumberNode(temp);
            }

            if (Match(TokenTypes.Variables))
            {
                string Name = Consume().Value;
                if (Match(TokenTypes.Equals)) {
                    Consume(); // =
                    IExpression left = ParseExpression();
                    return new AssignNode(Name, variableDict, left);
                    /*
                    if (Match(TokenTypes.Number))
                    {
                        string text = Consume().Value;
                        text = text.Replace(',', '.');
                        double temp = double.Parse(text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
                        variableDict[Name] = temp;
                        return new VariableNode(Name, variableDict);
                    }
                    if (Match(TokenTypes.Variables))
                    {
                        string text = Consume().Value;
                        variableDict[Name] = variableDict[text];
                        return new VariableNode(Name, variableDict);
                    }*/
                    
                }
                return new VariableNode(Name, variableDict);
            }

            if (Match(TokenTypes.BracketOpen))
            {
                Consume();
                IExpression expr = ParseExpression();

                if (!Match(TokenTypes.BracketClose))
                {
                    //MessageBox.Show("Fehlende schließende Klammer");
                    throw new MissingClosingBracketException("");
                }
                Consume();
                return expr;
            }

            //MessageBox.Show("Unerwartetes Token: " + Current.Value);
            throw new WrongTokenException(Current.Value);
        }
    }
}
