using HighlightTextBlock;
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
    /// Interaktionslogik für Fehlerdial.xaml
    /// </summary>
    public partial class Fehlerdial : Window
    {
        public string x = "";

        public List<Token> tokens;

        public static readonly DependencyProperty FindStringProperty =
            DependencyProperty.Register(nameof(FindString), typeof(string), typeof(Fehlerdial),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty FullTextProperty =
            DependencyProperty.Register(nameof(FullText), typeof(string), typeof(Fehlerdial),
                new PropertyMetadata(string.Empty));
        public Fehlerdial(List<Token> tokens)
        {
            this.tokens = tokens;
            InitializeComponent();
            this.DataContext = this;
            showerr();
        }

        public String FindString
        {
            get
            {
                return (string)GetValue(FindStringProperty);
            }
            set
            {
                SetValue(FindStringProperty, value);
            }
        }
        public String FullText
        {
            get
            {
                return (string)GetValue(FullTextProperty);
            }
            set
            {
                SetValue(FullTextProperty, value);
            }
        }

        public void showerr()
        {
            while (tokens.Count > 0) 
            {
                x += tokens[0].value;
                if (tokens[0].type == Token.TokenType.fehler)
                {
                    FindString = tokens[0].value;
                    tokens.RemoveAt(0);
                    FullText = x;
                    return;
                }
                tokens.RemoveAt(0);

            }
            FullText = x;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            showerr();
        }
    }
}
