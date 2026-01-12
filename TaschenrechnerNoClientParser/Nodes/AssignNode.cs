using System.Windows;

namespace TaschenrechnerCS.Nodes
{
    internal class AssignNode : IExpression
    {
        public String Name;
        public Dictionary<String, double> Dict;
        private IExpression Expr;
        public double Evaluate(Dictionary<string, object> variables)
        {
            double val = Expr.Evaluate(variables);
            Dict[Name] = val;
            return val;
        }

        public AssignNode(String name, Dictionary<string, double> dict, IExpression expr)
        {
            Name = name;
            Dict = dict;
            Expr = expr;
        }
    }
}
