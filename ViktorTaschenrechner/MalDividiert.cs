using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Russiantechnerrr
{
    public class MalDividiert:Anweisung
    {
        List<Anweisung> AnweisungList = new List<Anweisung>();
        List<Token> Operators = new List<Token>();
        public override void Parse(ref List<Token> Tokenlist)
        {
            Hoch hoch = new Hoch();
            hoch.Parse(ref Tokenlist);
            AnweisungList.Add(hoch);
            while (Tokenlist.Count > 0)
            {
                if (Tokenlist[0].type == Token.TokenType.maldividiert)
                {
                    Operators.Add(Tokenlist[0]);
                    Tokenlist.RemoveAt(0);
                    Hoch hoch1 = new Hoch();
                    hoch1.Parse(ref Tokenlist);
                    AnweisungList.Add(hoch1);
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
            val += AnweisungList[0].Run(ref Tokenlist);
            AnweisungList.RemoveAt(0);
            while (AnweisungList.Count > 0)
            {
                if (Operators.Count == 0)
                {
                    break;
                }
                if (Operators[0].value == "/")
                {
                    val /= AnweisungList[0].Run(ref Tokenlist);
                    Operators.RemoveAt(0);
                    AnweisungList.RemoveAt(0);
                }
                else if (Operators[0].value == "*")
                {
                    val *= AnweisungList[0].Run(ref Tokenlist);
                    Operators.RemoveAt(0);
                    AnweisungList.RemoveAt(0);
                }
            }

            return val;
        }
    }
}
