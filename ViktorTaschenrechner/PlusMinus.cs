using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Russiantechnerrr
{
    public class PlusMinus:Anweisung
    {
        List<Anweisung> AnweisungList = new List<Anweisung>();
        List<Token> Operators = new List<Token>();
        public override void Parse(ref List<Token> Tokenlist)
        {
            MalDividiert malDividiert = new MalDividiert();
            malDividiert.Parse(ref Tokenlist);
            AnweisungList.Add(malDividiert);
            while (Tokenlist.Count > 0)
            {
                Console.WriteLine("du hurensohn");
                if (Tokenlist[0].type == Token.TokenType.plusminus)
                {
                    Operators.Add(Tokenlist[0]);
                    Tokenlist.RemoveAt(0);
                    MalDividiert malDividiert1 = new MalDividiert();
                    malDividiert1.Parse(ref Tokenlist);
                    AnweisungList.Add(malDividiert1);
                }
                else
                {
                    break;
                }
            }
        }
        public override float Run(ref List<Token> Tokenlist)
        {
            float val = 0;
            val  = AnweisungList[0].Run(ref Tokenlist);
            AnweisungList.RemoveAt(0);
            while (AnweisungList.Count > 0)
            {
                if (Operators.Count == 0)
                {
                    break;
                }
                if (Operators[0].value == "+")
                {
                    val += AnweisungList[0].Run(ref Tokenlist);
                    Operators.RemoveAt(0);
                    AnweisungList.RemoveAt(0);
                }
                else if (Operators[0].value == "-")
                {
                    val -= AnweisungList[0].Run(ref Tokenlist);
                    Operators.RemoveAt(0);
                    AnweisungList.RemoveAt(0);
                }
            }

            return val;
        }
    
    }
}
