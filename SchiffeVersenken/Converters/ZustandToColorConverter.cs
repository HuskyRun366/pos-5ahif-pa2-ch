using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SchiffeVersenken.Converters
{
    /// <summary>
    /// Konvertiert Zellenanzeige zu Hintergrundfarbe.
    /// </summary>
    public class ZustandToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 1) return Brushes.LightBlue;

            string anzeige = values[0]?.ToString() ?? "Wasser";
            bool istVorschau = values.Length > 1 && values[1] is bool v && v;
            bool istGueltig = values.Length > 2 && values[2] is bool g && g;

            if (istVorschau)
            {
                return istGueltig ? Brushes.LightGreen : Brushes.LightCoral;
            }

            return anzeige switch
            {
                "Treffer" => Brushes.OrangeRed,
                "Daneben" => Brushes.LightGray,
                "Schiff" => Brushes.DarkGray,
                "Unbekannt" => Brushes.LightSteelBlue,
                _ => Brushes.LightBlue // Wasser
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Einfacher Konverter für Einzelwert.
    /// </summary>
    public class ZustandToSimpleColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string anzeige = value?.ToString() ?? "Wasser";

            return anzeige switch
            {
                "Treffer" => Brushes.OrangeRed,
                "Daneben" => Brushes.LightGray,
                "Schiff" => Brushes.DarkGray,
                "Unbekannt" => Brushes.LightSteelBlue,
                _ => Brushes.LightBlue
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
