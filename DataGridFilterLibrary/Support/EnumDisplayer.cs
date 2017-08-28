using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows.Data;
using System.Windows.Markup;

namespace DataGridFilterLibrary.Support {
    /// <summary>
    ///     Code from: http://www.ageektrapped.com/blog/the-missing-net-7-displaying-enums-in-wpf/
    /// </summary>
    [ContentProperty("OverriddenDisplayEntries")]
    public class EnumDisplayer : IValueConverter {
        private IDictionary displayValues;
        private List<EnumDisplayEntry> overriddenDisplayEntries;
        private IDictionary reverseValues;
        private Type type;

        public EnumDisplayer() { }

        public EnumDisplayer(Type type) {
            Type = type;
        }

        public Type Type {
            get => type;
            set {
                if (!value.IsEnum) throw new ArgumentException("parameter is not an Enumermated type", "value");
                type = value;
            }
        }

        public ReadOnlyCollection<string> DisplayNames {
            get {
                var displayValuesType = typeof(Dictionary<,>).GetGenericTypeDefinition().MakeGenericType(type, typeof(string));
                displayValues = (IDictionary) Activator.CreateInstance(displayValuesType);

                reverseValues = (IDictionary) Activator.CreateInstance(typeof(Dictionary<,>).GetGenericTypeDefinition().MakeGenericType(typeof(string), type));

                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
                foreach (var field in fields) {
                    var a = (DisplayStringAttribute[]) field.GetCustomAttributes(typeof(DisplayStringAttribute), false);

                    var displayString = GetDisplayStringValue(a);
                    var enumValue = field.GetValue(null);

                    if (displayString == null) displayString = GetBackupDisplayStringValue(enumValue);
                    if (displayString != null) {
                        displayValues.Add(enumValue, displayString);
                        reverseValues.Add(displayString, enumValue);
                    }
                }
                return new List<string>((IEnumerable<string>) displayValues.Values).AsReadOnly();
            }
        }

        public List<EnumDisplayEntry> OverriddenDisplayEntries {
            get {
                if (overriddenDisplayEntries == null) overriddenDisplayEntries = new List<EnumDisplayEntry>();
                return overriddenDisplayEntries;
            }
        }


        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return displayValues[value];
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return reverseValues[value];
        }

        private string GetDisplayStringValue(DisplayStringAttribute[] a) {
            if (a == null || a.Length == 0) return null;
            var dsa = a[0];
            if (!string.IsNullOrEmpty(dsa.ResourceKey)) {
                var rm = new ResourceManager(type);
                return rm.GetString(dsa.ResourceKey);
            }
            return dsa.Value;
        }

        private string GetBackupDisplayStringValue(object enumValue) {
            if (overriddenDisplayEntries != null && overriddenDisplayEntries.Count > 0) {
                var foundEntry = overriddenDisplayEntries.Find(delegate(EnumDisplayEntry entry) {
                    var e = Enum.Parse(type, entry.EnumValue);
                    return enumValue.Equals(e);
                });
                if (foundEntry != null) {
                    if (foundEntry.ExcludeFromDisplay) return null;
                    return foundEntry.DisplayString;
                }
            }
            return Enum.GetName(type, enumValue);
        }
    }

    public class EnumDisplayEntry {
        public string EnumValue { get; set; }
        public string DisplayString { get; set; }
        public bool ExcludeFromDisplay { get; set; }
    }
}