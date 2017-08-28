using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DataGridFilterLibrary.Support {
    public class ClearFilterButtonVisibilityConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if ((bool) values[0] && (bool) values[1]) return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}