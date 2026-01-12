using System;
using System.Collections.Generic;
using System.Text;

namespace TaschenrechnerCS.Nodes
{
    internal class MinusNode:IExpression
    {
        IExpression left;
        IExpression right;
        public double Evaluate(Dictionary<string, object> variables)
        {
            return left.Evaluate(variables) - right.Evaluate(variables);
        }

        public MinusNode(IExpression left, IExpression right)
        {
            this.left = left;
            this.right = right;
        }
    }
}
