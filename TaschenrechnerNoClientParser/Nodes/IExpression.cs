using System;
using System.Collections.Generic;
using System.Text;

namespace TaschenrechnerCS.Nodes
{
    internal interface IExpression
    {
        double Evaluate(Dictionary<string, object> variables=null);
    }
}
