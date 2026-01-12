using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Russiantechnerrr
{
    public class Token
    {
        public enum TokenType{fehler, zahl, variable, plusminus, maldividiert, hoch, lkl, rkl};

        public TokenType type;
        public string value;

        public Token()
        {
            type = TokenType.fehler;
            value = "ERROR";
        }
    }
}
