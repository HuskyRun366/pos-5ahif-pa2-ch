using System;
using System.Collections.Generic;
using System.Text;

namespace TaschenrechnerCS.Nodes
{
    internal class DivNode:IExpression
    {
        IExpression left;
        IExpression right;
        public double Evaluate(Dictionary<string, object> variables)
        {
            return left.Evaluate(variables) / right.Evaluate(variables);
        }

        public DivNode(IExpression left, IExpression right)
        {
            this.left = left;
            this.right = right;
        }
    }
}
