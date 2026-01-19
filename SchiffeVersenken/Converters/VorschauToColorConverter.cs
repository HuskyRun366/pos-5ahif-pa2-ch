using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SchiffeVersenken.Converters
{
    /// <summary>
    /// Konvertiert Vorschau-Status zu Hintergrundfarbe.
    /// </summary>
    public class VorschauToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3) return Brushes.LightBlue;

            string anzeige = values[0]?.ToString() ?? "Wasser";
            bool istVorschau = values[1] is bool v && v;
            bool istGueltig = values[2] is bool g && g;

            // Vorschau hat Priorität
            if (istVorschau)
            {
                return istGueltig
                    ? new SolidColorBrush(Color.FromArgb(180, 144, 238, 144)) // LightGreen transparent
                    : new SolidColorBrush(Color.FromArgb(180, 240, 128, 128)); // LightCoral transparent
            }

            // Normaler Zustand
            return anzeige switch
            {
                "Treffer" => Brushes.OrangeRed,
                "Daneben" => Brushes.LightGray,
                "Schiff" => Brushes.DarkSlateGray,
                "Unbekannt" => Brushes.LightSteelBlue,
                _ => Brushes.LightBlue
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
