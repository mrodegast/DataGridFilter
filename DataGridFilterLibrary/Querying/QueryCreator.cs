using System;
using System.Collections.Generic;
using System.Text;
using DataGridFilterLibrary.Support;

namespace DataGridFilterLibrary.Querying {
    public class QueryCreator {

        private readonly Dictionary<string, FilterData> mFiltersForColumns;
        private readonly ParameterCounter mParamCounter;

        public QueryCreator(Dictionary<string, FilterData> filtersForColumns) {
            this.mFiltersForColumns = filtersForColumns;

            mParamCounter = new ParameterCounter(0);
            Parameters = new List<object>();
        }

        private List<object> Parameters { get; }

        public void CreateFilter(ref Query query) {
            var filter = new StringBuilder();

            foreach (var kvp in mFiltersForColumns) {
                var partialFilter = CreateSingleFilter(kvp.Value);

                if (filter.Length > 0 && partialFilter.Length > 0) filter.Append(" AND ");
                if (partialFilter.Length > 0) {
                    var valuePropertyBindingPath = string.Empty;
                    var paths = kvp.Value.ValuePropertyBindingPath.Split('.');

                    foreach (var p in paths) {
                        if (valuePropertyBindingPath != string.Empty) valuePropertyBindingPath += ".";
                        valuePropertyBindingPath += p;

                        filter.Append(valuePropertyBindingPath + " != null AND "); //eliminate: Nullable object must have a value and object fererence not set to an object                        
                    }
                }
                filter.Append(partialFilter);
            }

            //init query
            query.FilterString = filter.ToString();
            query.QueryParameters = Parameters;
        }

        private StringBuilder CreateSingleFilter(FilterData filterData) {
            var filter = new StringBuilder();

            if ((filterData.Type == FilterType.NumericBetween || filterData.Type == FilterType.DateTimeBetween) && (filterData.QueryString != string.Empty || filterData.QueryStringTo != string.Empty)) {
                if (filterData.QueryString != string.Empty) CreateFilterExpression(filterData, filterData.QueryString, filter, getOperatorString(FilterOperator.GreaterThanOrEqual));
                if (filterData.QueryStringTo != string.Empty) {
                    if (filter.Length > 0) filter.Append(" AND ");
                    CreateFilterExpression(filterData, filterData.QueryStringTo, filter, getOperatorString(FilterOperator.LessThanOrEqual));
                }
            }
            else if (filterData.QueryString != string.Empty && filterData.Operator != FilterOperator.Undefined) {
                if (filterData.Type == FilterType.Text) CreateStringFilterExpression(filterData, filter);
                else CreateFilterExpression(filterData, filterData.QueryString, filter, getOperatorString(filterData.Operator));
            }
            return filter;
        }

        private void CreateFilterExpression(FilterData filterData, string queryString, StringBuilder filter, string operatorString) {
            filter.Append(filterData.ValuePropertyBindingPath);

            if (TrySetParameterValue(out var parameterValue, queryString, filterData.ValuePropertyType)) {
                Parameters.Add(parameterValue);
                mParamCounter.Increment();

                filter.Append(" " + operatorString + " @" + mParamCounter.ParameterNumber);
            }
            else {
                filter = new StringBuilder(); //do not use filter
            }
        }

        private bool TrySetParameterValue(out object parameterValue, string stringValue, Type type) {
            parameterValue = null;
            bool valueIsSet;

            try {
                if (type == typeof(DateTime?) || type == typeof(DateTime)) parameterValue = DateTime.Parse(stringValue);
                else if (type == typeof(Enum) || type.BaseType == typeof(Enum)) Parameters.Add(Enum.Parse(type, stringValue, true));
                else if (type == typeof(bool) || type.BaseType == typeof(bool)) parameterValue = Convert.ChangeType(stringValue, typeof(bool));
                else parameterValue = Convert.ChangeType(stringValue, typeof(double)); //TODO use "real" number type

                valueIsSet = true;
            }
            catch (Exception) {
                valueIsSet = false;
            }

            return valueIsSet;
        }

        private void CreateStringFilterExpression(FilterData filterData, StringBuilder filter) {
            var creator = new StringFilterExpressionCreator(mParamCounter, filterData, Parameters);
            var filterExpression = creator.Create();

            filter.Append(filterExpression);
        }

        private string getOperatorString(FilterOperator filterOperator) {
            string op;

            switch (filterOperator) {
                case FilterOperator.Undefined:
                    op = string.Empty;
                    break;
                case FilterOperator.LessThan:
                    op = "<";
                    break;
                case FilterOperator.LessThanOrEqual:
                    op = "<=";
                    break;
                case FilterOperator.GreaterThan:
                    op = ">";
                    break;
                case FilterOperator.GreaterThanOrEqual:
                    op = ">=";
                    break;
                case FilterOperator.Equals:
                    op = "=";
                    break;
                case FilterOperator.Like:
                    op = string.Empty;
                    break;
                default:
                    op = string.Empty;
                    break;
            }

            return op;
        }
    }
}