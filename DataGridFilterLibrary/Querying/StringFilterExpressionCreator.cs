using System;
using System.Collections.Generic;
using System.Text;
using DataGridFilterLibrary.Support;

namespace DataGridFilterLibrary.Querying {
    internal class StringFilterExpressionCreator {

        private const string WildcardAnyString = "%";

        private readonly FilterData mFilterData;
        private readonly ParameterCounter mParamCounter;
        private readonly List<object> mParamseters;

        internal StringFilterExpressionCreator(ParameterCounter paramCounter, FilterData filterData, List<object> paramseters) {
            this.mParamCounter = paramCounter;
            this.mFilterData = filterData;
            this.mParamseters = paramseters;
        }

        internal int ParametarsCrated => mParamseters.Count;

        internal string Create() {
            var filter = new StringBuilder();
            var filterList = Parse(mFilterData.QueryString);

            for (var i = 0; i < filterList.Count; i++) {
                if (i > 0) filter.Append(" and ");

                filter.Append(filterList[i]);
            }
            return filter.ToString();
        }

        private List<string> Parse(string filterString) {
            string token;
            var i = 0;
            var expressionCompleted = false;
            var filter = new List<string>();
            var expressionValue = string.Empty;
            var function = StringExpressionFunction.Undefined;

            do {
                token = i < filterString.Length ? filterString[i].ToString() : null;

                if (token == WildcardAnyString || token == null)
                    if (expressionValue.StartsWith(WildcardAnyString) && token != null) {
                        function = StringExpressionFunction.IndexOf;
                        expressionCompleted = true;
                    }
                    else if (expressionValue.StartsWith(WildcardAnyString) && token == null) {
                        function = StringExpressionFunction.EndsWith;
                        expressionCompleted = false;
                    }
                    else {
                        function = StringExpressionFunction.StartsWith;
                        if (filterString.Length - 1 > i) expressionCompleted = true;
                    }

                if (token == null) expressionCompleted = true;

                expressionValue += token;

                if (expressionCompleted && function != StringExpressionFunction.Undefined && expressionValue != string.Empty) {
                    var expressionValueCopy = string.Copy(expressionValue);
                    expressionValueCopy = expressionValueCopy.Replace(WildcardAnyString, string.Empty);

                    if (expressionValueCopy != string.Empty) filter.Add(CreateFunction(function, expressionValueCopy));
                    function = StringExpressionFunction.Undefined;
                    expressionValue = expressionValue.EndsWith(WildcardAnyString) ? WildcardAnyString : string.Empty;
                    expressionCompleted = false;
                }
                i++;
            } while (token != null);

            return filter;
        }

        private string CreateFunction(StringExpressionFunction function, string value) {
            var filter = new StringBuilder();
            mParamseters.Add(value);

            filter.Append(mFilterData.ValuePropertyBindingPath);
            if (mFilterData.ValuePropertyType.IsGenericType && mFilterData.ValuePropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) filter.Append(".Value");

            mParamCounter.Increment();
            mParamCounter.Increment();

            filter.Append(".ToString()." + function + "(@" + (mParamCounter.ParameterNumber - 1) + ", @" + mParamCounter.ParameterNumber + ")");
            if (function == StringExpressionFunction.IndexOf) filter.Append(" != -1 ");
            mParamseters.Add(mFilterData.IsCaseSensitiveSearch ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);
            return filter.ToString();
        }

        private enum StringExpressionFunction {
            Undefined = 0,
            StartsWith = 1,
            IndexOf = 2,
            EndsWith = 3
        }
    }
}