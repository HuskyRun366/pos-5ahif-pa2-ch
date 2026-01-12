using System;
using System.Collections.Generic;
using System.Text;

namespace TaschenrechnerCS
{
    class Token
    {
        public int Index { get; set; }
        public string Value { get; set; }
        public TokenTypes ToType { get; set; }

        public Token(int index, string value, TokenTypes type)
        {
            this.Index = index;
            this.Value = value;
            this.ToType = type;
        }

    }
}
