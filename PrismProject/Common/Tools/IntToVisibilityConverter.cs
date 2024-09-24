using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PrismProject.Common.Tools
{
    public class IntToVisibilityConverter : IValueConverter  // Int转换为Visibility
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value != null && int.TryParse(value.ToString(), out int intValue))
            {
                if(intValue == 0) return Visibility.Visible;
                else return Visibility.Hidden;

            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
