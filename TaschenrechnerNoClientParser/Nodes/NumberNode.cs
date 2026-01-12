using System;
using System.Collections.Generic;
using System.Text;

namespace TaschenrechnerCS.Nodes
{
    internal class NumberNode:IExpression
    {
        double Value;

        public double Evaluate(Dictionary<string, object> variables)
        {
            return Value;
        }

        public NumberNode(double value)
        {
            Value = value;
        }
    }
}
