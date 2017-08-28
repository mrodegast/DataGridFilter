using System;
using System.Globalization;
using System.Windows.Data;

namespace DataGridFilterLibrary.Support {
    public class ComboBoxToQueryStringConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value != null && value.ToString() == string.Empty ? null : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value;
        }

        #endregion
    }
}