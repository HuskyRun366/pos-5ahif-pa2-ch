using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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

namespace TaschenrechnerCS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        Dictionary<string, double> variables = new Dictionary<string, double>();

        private string _findString = string.Empty;
        public string FindString
        {
            get => _findString;
            set
            {
                if (_findString != value)
                {
                    _findString = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            foreach (Button btn in btnGrid.Children) {
                //if(btn.Name != "auswertenBtn" || btn.Name != "clearBtn")
                if ((string) btn.Content != "AC" && (string) btn.Content != "Auswerten")
                {
                    btn.Click += Btn_Click;
                }
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            outputBox.Text += btn.Content;
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            outputBox.Text = string.Empty;
        }

        private void auswertenBtn_Click(object sender, RoutedEventArgs e)
        {
            Tokenizer toki = new Tokenizer(outputBox.Text);
            var temp = toki.getListOrderedByIndex();
            if (temp.Count == 0)
            {
                FindString = toki.unknownTK;
                return;
            }
            try
            {
                var pars = new Parser(temp, variables);
                var ast = pars.ParseExpression();
                double ergebnis = ast.Evaluate();
                outputBox.Text = ergebnis.ToString();
            }
            catch (Exception ex)
            {
                if(ex is WrongTokenException)
                {
                    MessageBox.Show(ex.Message);
                    FindString = ex.Message;
                }
                if(ex is MissingClosingBracketException)
                {
                    MessageBox.Show("Fehlende schließende Klammer");
                }
            }
            
        }
    }
}