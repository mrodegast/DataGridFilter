﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DataGridFilterLibrary.Support {
    public class VisibilityToWidthConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var visibility = (Visibility) value;

            return visibility == Visibility.Visible ? double.NaN : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value;
        }
    }
}