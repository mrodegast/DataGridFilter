using System;
using System.ComponentModel;

namespace DataGridFilterLibrary.Support {
    [Serializable]
    public class FilterData : INotifyPropertyChanged {
        private FilterOperator mOperator;

        private string mQueryString;
        private string mQueryStringTo;

        public FilterData(FilterOperator Operator, FilterType Type, string ValuePropertyBindingPath, Type ValuePropertyType, string QueryString, string QueryStringTo, bool IsTypeInitialized, bool IsCaseSensitiveSearch) {
            this.Operator = Operator;
            this.Type = Type;
            this.ValuePropertyBindingPath = ValuePropertyBindingPath;
            this.ValuePropertyType = ValuePropertyType;
            this.QueryString = QueryString;
            this.QueryStringTo = QueryStringTo;

            this.IsTypeInitialized = IsTypeInitialized;
            this.IsCaseSensitiveSearch = IsCaseSensitiveSearch;
        }

        public FilterOperator Operator {
            get => mOperator;
            set {
                if (mOperator != value) {
                    mOperator = value;
                    NotifyPropertyChanged("Operator");
                    OnFilterChangedEvent();
                }
            }
        }

        public string QueryString {
            get => mQueryString;
            set {
                if (mQueryString != value) {
                    mQueryString = value ?? string.Empty;

                    NotifyPropertyChanged("QueryString");
                    OnFilterChangedEvent();
                }
            }
        }

        public string QueryStringTo {
            get => mQueryStringTo;
            set {
                if (mQueryStringTo != value) {
                    mQueryStringTo = value ?? string.Empty;

                    NotifyPropertyChanged("QueryStringTo");
                    OnFilterChangedEvent();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void ClearData() {
            mIsClearData = true;

            Operator = FilterOperator.Undefined;
            if (QueryString != string.Empty) QueryString = null;
            if (QueryStringTo != string.Empty) QueryStringTo = null;

            mIsClearData = false;
        }

        public void NotifyPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Metadata

        public FilterType Type { get; set; }
        public string ValuePropertyBindingPath { get; set; }
        public Type ValuePropertyType { get; set; }
        public bool IsTypeInitialized { get; set; }
        public bool IsCaseSensitiveSearch { get; set; }

        //query optimization fileds
        public bool IsSearchPerformed { get; set; }

        public bool IsRefresh { get; set; }
        //query optimization fileds

        #endregion

        #region Filter Change Notification

        public event EventHandler<EventArgs> FilterChangedEvent;
        private bool mIsClearData;

        private void OnFilterChangedEvent() {
            var temp = FilterChangedEvent;

            if (temp != null) {
                bool filterChanged;

                switch (Type) {
                    case FilterType.Numeric:
                    case FilterType.DateTime:

                        filterChanged = Operator != FilterOperator.Undefined || QueryString != string.Empty;
                        break;

                    case FilterType.NumericBetween:
                    case FilterType.DateTimeBetween:

                        mOperator = FilterOperator.Between;
                        filterChanged = true;
                        break;

                    case FilterType.Text:

                        mOperator = FilterOperator.Like;
                        filterChanged = true;
                        break;

                    case FilterType.List:
                    case FilterType.Boolean:

                        mOperator = FilterOperator.Equals;
                        filterChanged = true;
                        break;

                    default:
                        filterChanged = false;
                        break;
                }

                if (filterChanged && !mIsClearData) temp(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}