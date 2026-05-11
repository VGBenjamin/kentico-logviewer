using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace KenticoLogViewer.Converters;

public class EventTypeToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "E" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEBEE")),
            "W" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF8E1")),
            "I" => Brushes.White,
            _   => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5")),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
