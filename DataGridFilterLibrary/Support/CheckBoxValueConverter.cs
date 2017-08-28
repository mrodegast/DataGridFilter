using System;
using System.Globalization;
using System.Windows.Data;

namespace DataGridFilterLibrary.Support {
    public class CheckBoxValueConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var result = false;

            if (value is string) bool.TryParse(value.ToString(), out result);
            else if (value != null) result = System.Convert.ToBoolean(value);

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value;
        }

        #endregion
    }
}