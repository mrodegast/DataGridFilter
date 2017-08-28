﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DataGridFilterLibrary.Support {
    public class MyOppositeBooleanToVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (!(bool) value) return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            var visibility = (Visibility) value;

            return visibility == Visibility.Visible ? true : false;
        }
    }
}