using System;
using System.Collections.Generic;
using System.Text;

namespace TaschenrechnerCS.Nodes
{
    internal class PowerNode:IExpression
    {
        IExpression left;
        IExpression right;
        public double Evaluate(Dictionary<string, object> variables)
        {
            return Math.Pow(left.Evaluate(variables), right.Evaluate(variables));
        }

        public PowerNode(IExpression left, IExpression right)
        {
            this.left = left;
            this.right = right;
            
        }
    }
}
