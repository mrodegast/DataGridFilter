using System;

namespace DataGridFilterLibrary.Support {
    /// <summary>
    ///     Code from: http://www.ageektrapped.com/blog/the-missing-net-7-displaying-enums-in-wpf/
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DisplayStringAttribute : Attribute {
        public DisplayStringAttribute(string v) {
            Value = v;
        }

        public DisplayStringAttribute() { }

        public string Value { get; }

        public string ResourceKey { get; set; }
    }
}