using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Russiantechnerrr
{
    public  class Anweisung
    {
        public virtual void Parse(ref List<Token> Tokenlist) { }
        public virtual float Run(ref List<Token> Tokenlist) { return 0; }

        public static Dictionary<char, float> variables;
    }
}
