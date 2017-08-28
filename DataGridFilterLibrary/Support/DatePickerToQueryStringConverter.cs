using System;
using System.Globalization;
using System.Windows.Data;

namespace DataGridFilterLibrary.Support {
    public class DatePickerToQueryStringConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            object convertedValue;

            if (value != null && value.ToString() == string.Empty) {
                convertedValue = null;
            }
            else {
                DateTime dateTime;

                if (DateTime.TryParse(value?.ToString(), culture.DateTimeFormat, DateTimeStyles.None, out dateTime)) convertedValue = dateTime;
                else convertedValue = null;
            }

            return convertedValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value;
        }

        #endregion
    }
}