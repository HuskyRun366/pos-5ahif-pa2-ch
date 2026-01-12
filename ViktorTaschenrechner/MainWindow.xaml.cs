using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Russiantechnerrr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<char, float> variables = new Dictionary<char, float>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void auswertbutton_Click(object sender, RoutedEventArgs e)
        {
            bool err = false;
            Regex Variabler = new Regex(@"[A-Za-z]");
            Regex Hoch = new Regex(@"\^");
            Regex MalDividiert = new Regex(@"\*|\/");
            Regex PlusMinus = new Regex(@"\+|\-");
            Regex Nummer = new Regex(@"(\d+(,\d+)?)");
            Regex lKlammer = new Regex(@"\(");
            Regex rKlammer = new Regex(@"\)");
            Regex Globalregex = new Regex(@"[A-Z]|\^|\*|\/|\+|\-|\d+(,\d+)?|\)|\(|\s|.");
            string x  = inptb.Text.ToString().Replace(" " , "");
            MatchCollection  mc = Globalregex.Matches(x);
            List <Token> Tokenlist = new List<Token>();
            foreach (Match m in mc)
            {
                Token t = new Token();
                if (m.Success)
                {
                    if (Variabler.Match(m.Value).Success)
                    {
                        t.type = Token.TokenType.variable;
                        t.value = m.Value;
                    }
                    if (Hoch.Match(m.Value).Success)
                    {
                        t.type = Token.TokenType.hoch;
                        t.value = m.Value;
                    }
                    if (MalDividiert.Match(m.Value).Success)
                    {
                        t.type = Token.TokenType.maldividiert;
                        t.value = m.Value;
                    }
                    if (PlusMinus.Match(m.Value).Success)
                    {
                        t.type = Token.TokenType.plusminus;
                        Console.WriteLine(m.Value);
                        t.value = m.Value;
                    }
                    if (Nummer.Match(m.Value).Success)
                    {
                        t.type = Token.TokenType.zahl;
                        t.value = m.Value;
                    }
                    if (lKlammer.Match(m.Value).Success)
                    {
                        t.type = Token.TokenType.lkl;
                        t.value = m.Value;
                    }
                    if (rKlammer.Match(m.Value).Success)
                    {
                        t.type = Token.TokenType.rkl;
                        t.value = m.Value;
                    }
                    if (t.type == Token.TokenType.fehler)
                    {
                        err = true;
                        t.value = m.Value;
                    }
                    Tokenlist.Add(t);
                }

            }
            if (err)
            {
                Fehlerdial fehlerr = new Fehlerdial(Tokenlist);
                fehlerr.ShowDialog();
            }
            else
            {
                PlusMinus expr = new PlusMinus();
                Anweisung.variables = variables;
                expr.Parse(ref Tokenlist);
                float erg = expr.Run(ref Tokenlist);
                inptb.Text = erg.ToString();
            }
        }

        private void del_Click(object sender, RoutedEventArgs e)
        {
            if (inptb.Text != "")
            {
                string x = inptb.Text;
                x = x.Remove(x.Length - 1);
                inptb.Text = x;
            }
       
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            inptb.Text = inptb.Text + 1;
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            inptb.Text = inptb.Text + 2;

        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            inptb.Text = inptb.Text + 3;

        }

        private void plusbtn_Click(object sender, RoutedEventArgs e)
        {
            inptb.Text = inptb.Text + " " + "+";

        }

        private void minusbtn_Click(object sender, RoutedEventArgs e)
        {
            inptb.Text = inptb.Text + " " + "- ";

        }

        private void btn4_Click(object sender, RoutedEventArgs e)
        {
            inptb.Text = inptb.Text +  4;

        }

        private void btn5_Click(object sender, RoutedEventArgs e)
        {
            inptb.Text = inptb.Text +  5;

        }

        private void btn6_Click(object sender, RoutedEventArgs e)
        {
            inptb.Text = inptb.Text + 6;

        }

        private void malbtn_Click(object sender, RoutedEventArgs e)
        {
            inptb.Text = inptb.Text + " " + "*" + " ";

        }

        private void divbtn_Click(object sender, RoutedEventArgs e)
        {
            inptb.Text = inptb.Text + " " + "/ ";


        }

        private void kommabtn_Click(object sender, RoutedEventArgs e)
        {
            inptb.Text = inptb.Text + ",";

        }

        private void btn7_Click(object sender, RoutedEventArgs e)
        {
            inptb.Text = inptb.Text +  7;

        }

        private void btn8_Click(object sender, RoutedEventArgs e)
        {
            inptb.Text = inptb.Text + 8;

        }

        private void btn9_Click(object sender, RoutedEventArgs e)
        {
            inptb.Text = inptb.Text + 9;

        }

        private void hochtbn_Click(object sender, RoutedEventArgs e)
        {
            inptb.Text = inptb.Text + " ^ ";


        }

        private void klzubtn_Click(object sender, RoutedEventArgs e)
        {
            inptb.Text = inptb.Text + ") ";

        }

        private void klaufbtn_Click(object sender, RoutedEventArgs e)
        {
            inptb.Text = inptb.Text + " (";

        }

        private void zbtn_Click(object sender, RoutedEventArgs e)
        {
            inptb.Text = inptb.Text + "z";

        }

        private void ybtn_Click(object sender, RoutedEventArgs e)
        {
            inptb.Text = inptb.Text + "y";

        }

        private void xtbn_Click(object sender, RoutedEventArgs e)
        {
            inptb.Text = inptb.Text + "x";

        }

        private void editvar_Click(object sender, RoutedEventArgs e)
        {
            variablendial variablendial = new variablendial();
            variablendial.close.Click += (sender, e) => {
                   foreach(var item in variablendial.lb.Items)
                    {
                        string x = item.ToString();
                    string[] y = x.Split("=");
                        variables.Add(y[0][0], Convert.ToSingle(y[1]));
                    }
                variablendial.Close();
                
            };
            variablendial.Show();
                    
            
        }


    }
}