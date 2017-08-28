using System;
using System.Globalization;
using System.Windows.Data;

namespace DataGridFilterLibrary.Support {
    public class BooleanToHeightConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value != null && (bool) value) return double.NaN;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value;
        }
    }
}