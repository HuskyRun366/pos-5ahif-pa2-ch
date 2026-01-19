using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SchiffeVersenken.Converters
{
    /// <summary>
    /// Konvertiert Anzeige-String zu Visibility für Treffer-Symbol.
    /// </summary>
    public class TrefferVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == "Treffer" ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Konvertiert Anzeige-String zu Visibility für Daneben-Symbol.
    /// </summary>
    public class DanebenVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == "Daneben" ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
