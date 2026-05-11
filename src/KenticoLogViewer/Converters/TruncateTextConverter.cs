using System.Globalization;
using System.Windows.Data;

namespace KenticoLogViewer.Converters;

public class TruncateTextConverter : IValueConverter
{
    public int MaxLength { get; set; } = 200;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string text) return string.Empty;
        if (text.Length <= MaxLength) return text;
        return string.Concat(text.AsSpan(0, MaxLength), "…");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
