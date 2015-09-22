using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Common.Converters
{
    public class NullableBooleanToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                bool result = false;
                bool.TryParse(value.ToString(), out result);
                return result;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            bool result = false;
            bool.TryParse(value.ToString(), out result);
            return result ? true : false;
        }
    }
}
