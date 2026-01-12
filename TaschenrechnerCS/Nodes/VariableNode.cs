using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace TaschenrechnerCS.Nodes
{
    internal class VariableNode : IExpression
    {
        public String Name;
        public Dictionary<String, double> Dict;
        public double Evaluate(Dictionary<string, object> variables)
        {
            if (!Dict.TryGetValue(Name, out double value))
            {
                MessageBox.Show("Variable nicht definiert: " + Name);
            }
            return value;
        }

        public VariableNode(String name, Dictionary<string, double> dict)
        {
            Name = name;
            Dict = dict;
        }
    }
}
