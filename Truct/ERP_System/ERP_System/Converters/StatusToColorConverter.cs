using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ERP_System.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status.ToLower() switch
                {
                    "draft" => new SolidColorBrush(Color.FromRgb(149, 165, 166)),
                    "approved" => new SolidColorBrush(Color.FromRgb(39, 174, 96)),
                    "completed" => new SolidColorBrush(Color.FromRgb(52, 152, 219)),
                    "cancelled" => new SolidColorBrush(Color.FromRgb(231, 76, 60)),
                    "low" => new SolidColorBrush(Color.FromRgb(231, 76, 60)),
                    "high" => new SolidColorBrush(Color.FromRgb(243, 156, 18)),
                    "normal" => new SolidColorBrush(Color.FromRgb(39, 174, 96)),
                    _ => new SolidColorBrush(Color.FromRgb(149, 165, 166))
                };
            }
            return new SolidColorBrush(Color.FromRgb(149, 165, 166));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}