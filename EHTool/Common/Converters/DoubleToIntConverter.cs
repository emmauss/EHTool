using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Common.Converters
{
    public sealed class DoubleToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!value.GetType().Equals(typeof(double)))
            {
                throw new ArgumentException("Only Double is supported");
            }
            if (targetType.Equals(typeof(int)))
            {
                return (int)((double)value);
            }
            else if (targetType.Equals(typeof(uint)))
            {
                return (uint)((double)value);
            }
            else
            {
                throw new ArgumentException("Unsuported type {0}", targetType.FullName);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (double)((int)value);
        }
    }

    public sealed class IntToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (double)((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (int)((double)value);
        }
    }
}
