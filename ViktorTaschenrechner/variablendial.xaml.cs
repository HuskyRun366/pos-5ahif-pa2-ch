using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Russiantechnerrr
{
    /// <summary>
    /// Interaktionslogik für variablendial.xaml
    /// </summary>
    public partial class variablendial : Window
    {
        public variablendial()
        {
            InitializeComponent();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            string var = tbvar.Text;
            string val = tbval.Text;
            lb.Items.Add(var + "=" + val);
        }

        private void rmvar_Click(object sender, RoutedEventArgs e)
        {
            if (lb.SelectedItem != null)
            {
                lb.Items.Remove(lb.SelectedItem);
            }
        }
    }
}
