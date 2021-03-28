using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace DCGServiceDesk.Converters
{
    public class DataConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(targetType.FullName == "System.Double")
                return (double)value - (double)parameter;

            return (bool)value;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == "false")
                value = "false";

            return bool.Parse(value.ToString());
        }
    }
    public class StringToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString().Equals("y");
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                return bool.Parse(value.ToString()) ? "y" : "n";
            }
            return null;
        }
    }
}
