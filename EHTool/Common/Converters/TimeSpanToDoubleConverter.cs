using System;
using Windows.UI.Xaml.Data;

namespace Common.Converters
{
    public class TimeSpanToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var time = TimeSpan.Parse(value.ToString());
            return time.TotalSeconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            double timevalue = double.Parse(value.ToString());
            var time = TimeSpan.FromSeconds(timevalue);
            return time;
        }
    }
}
