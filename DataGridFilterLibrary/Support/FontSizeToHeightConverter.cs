using System;
using System.Globalization;
using System.Windows.Data;

namespace DataGridFilterLibrary.Support {
    public class FontSizeToHeightConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            double height;

            if (value != null)
                if (double.TryParse(value.ToString(), out height)) return height * 2;
                else return double.NaN;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value;
        }
    }
}