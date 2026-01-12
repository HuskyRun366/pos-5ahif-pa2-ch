using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Russiantechnerrr
{
    public class Hoch:Anweisung
    {
        List<Hoch> AnweisungList = new List<Hoch>();
        Token Value = new Token();
        public override void Parse(ref List<Token> Tokenlist)
        {
         
            if (Tokenlist[0].type == Token.TokenType.zahl || Tokenlist[0].type == Token.TokenType.variable)
            {
                if (Tokenlist[0].type == Token.TokenType.zahl)
                { 
                    Value.value = Tokenlist[0].value;
                    Value.type = Tokenlist[0].type;
                    Tokenlist.RemoveAt(0);
                 }
                else
                {
                    Value.value = Anweisung.variables[Tokenlist[0].value[0]].ToString();
                    Tokenlist.RemoveAt(0);
                }
            }

            while (Tokenlist.Count > 0)
            {
                if (Tokenlist[0].type == Token.TokenType.hoch) 
                {
                    Tokenlist.RemoveAt(0);
                }
                else if (Tokenlist[0].type == Token.TokenType.zahl || Tokenlist[0].type == Token.TokenType.variable)
                {
                    Hoch hoch = new Hoch();
                    hoch.Parse(ref Tokenlist);
                    AnweisungList.Add(hoch);
                }
                else if (Tokenlist[0].type == Token.TokenType.lkl)
                {
                    Tokenlist.RemoveAt(0);
                    PlusMinus pm = new PlusMinus();
                    pm.Parse(ref Tokenlist);
                    Value.type = Token.TokenType.zahl;
                    Value.value = pm.Run(ref Tokenlist).ToString();

                }
                else if (Tokenlist[0].type == Token.TokenType.rkl)
                {
                    Tokenlist.RemoveAt(0);
                    break;
                }
                else
                        {
                    break;
                }
            }
        }
        public override float Run(ref List<Token> Tokenlist)
        {
            /*while(AnweisungList.Count > 0)
            {
                Value.value = Math.Pow(Convert.ToSingle(Value.value), Convert.ToSingle(AnweisungList[0].Value.value)).ToString();
                AnweisungList.RemoveAt(0);
            }
            return Convert.ToSingle(Value.value);*/
            float val = 0;
            while (AnweisungList.Count > 0)
            {
                val += Convert.ToSingle(Math.Pow(Convert.ToSingle(Value.value), AnweisungList[0].Run(ref Tokenlist)));
                AnweisungList.RemoveAt(0);
            }
            if (val == 0)
            {
                val = Convert.ToSingle(Value.value);
            }
            return val;
        }
    }
}
