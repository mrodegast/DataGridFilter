using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic;
using System.Windows.Data;
using System.Windows.Threading;
using DataGridFilterLibrary.Support;

namespace DataGridFilterLibrary.Querying {
    public class QueryController {

        private readonly Dictionary<string, FilterData> mFiltersForColumns;
        private readonly object mLockObject;

        private Query mQuery;

        public QueryController() {
            mLockObject = new object();

            mFiltersForColumns = new Dictionary<string, FilterData>();
            mQuery = new Query();
        }

        public FilterData ColumnFilterData { get; set; }
        public IEnumerable ItemsSource { get; set; }

        public Dispatcher CallingThreadDispatcher { get; set; }
        public bool UseBackgroundWorker { get; set; }

        public bool IsCurentControlFirstControl => mFiltersForColumns.Count > 0 && mFiltersForColumns.ElementAt(0).Value.ValuePropertyBindingPath == ColumnFilterData.ValuePropertyBindingPath;

        public void DoQuery(bool force = false) {
            ColumnFilterData.IsSearchPerformed = false;

            if (!mFiltersForColumns.ContainsKey(ColumnFilterData.ValuePropertyBindingPath)) mFiltersForColumns.Add(ColumnFilterData.ValuePropertyBindingPath, ColumnFilterData);
            else mFiltersForColumns[ColumnFilterData.ValuePropertyBindingPath] = ColumnFilterData;

            if (IsRefresh) {
                if (mFiltersForColumns.ElementAt(mFiltersForColumns.Count - 1).Value.ValuePropertyBindingPath == ColumnFilterData.ValuePropertyBindingPath) RunFiltering(force);
            }
            else if (FilteringNeeded) {
                RunFiltering(force);
            }
            ColumnFilterData.IsSearchPerformed = true;
            ColumnFilterData.IsRefresh = false;
        }

        public void ClearFilter() {
            var count = mFiltersForColumns.Count;
            for (var i = 0; i < count; i++) {
                var data = mFiltersForColumns.ElementAt(i).Value;
                data.ClearData();
            }
            DoQuery();
        }

        #region Internal

        private bool IsRefresh => (from f in mFiltersForColumns where f.Value.IsRefresh select f).Any();

        private bool FilteringNeeded => (from f in mFiltersForColumns where f.Value.IsSearchPerformed == false select f).Count() == 1;

        private void RunFiltering(bool force) {
            CreateFilterExpressionsAndFilteredCollection(out var filterChanged, force);

            if (filterChanged || force) {
                OnFilteringStarted(this, EventArgs.Empty);
                ApplayFilter();
            }
        }

        private void CreateFilterExpressionsAndFilteredCollection(out bool filterChanged, bool force) {
            var queryCreator = new QueryCreator(mFiltersForColumns);
            queryCreator.CreateFilter(ref mQuery);

            filterChanged = mQuery.IsQueryChanged || mQuery.FilterString != string.Empty && IsRefresh;

            if (force && mQuery.FilterString != string.Empty || mQuery.FilterString != string.Empty && filterChanged) {
                var collection = ItemsSource;
                if (ItemsSource is ListCollectionView) collection = (ItemsSource as ListCollectionView).SourceCollection;

                if (ItemsSource is INotifyCollectionChanged observable) {
                    observable.CollectionChanged -= observable_CollectionChanged;
                    observable.CollectionChanged += observable_CollectionChanged;
                }

                #region Debug

#if DEBUG
                System.Diagnostics.Debug.WriteLine("QUERY STATEMENT: " + query.FilterString);

                string debugParameters = String.Empty;
                query.QueryParameters.ForEach(p =>
                {
                    if (debugParameters.Length > 0) debugParameters += ",";
                    debugParameters += p.ToString();
                });

                System.Diagnostics.Debug.WriteLine("QUERY PARAMETRS: " + debugParameters);
                #endif

                #endregion

                if (mQuery.FilterString != string.Empty) {
                    var result = collection.AsQueryable().Where(mQuery.FilterString, mQuery.QueryParameters.ToArray<object>());
                    mFilteredCollection = result.Cast<object>().ToList();
                }
            }
            else {
                mFilteredCollection = null;
            }
            mQuery.StoreLastUsedValues();
        }

        private void observable_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            DoQuery(true);
        }

        #region Internal Filtering

        private IList mFilteredCollection;
        private HashSet<object> mFilteredCollectionHashSet;

        private void ApplayFilter() {
            var view = CollectionViewSource.GetDefaultView(ItemsSource);

            if (mFilteredCollection != null)
                ExecuteFilterAction(() => {
                    mFilteredCollectionHashSet = initLookupDictionary(mFilteredCollection);

                    view.Filter = ItemPassesFilter;

                    OnFilteringFinished(this, EventArgs.Empty);
                });
            else
                ExecuteFilterAction(() => {
                    if (view.Filter != null) view.Filter = null;
                    OnFilteringFinished(this, EventArgs.Empty);
                });
        }

        private void ExecuteFilterAction(Action action) {
            if (UseBackgroundWorker) {
                var worker = new BackgroundWorker();

                worker.DoWork += delegate {
                    lock (mLockObject) {
                        ExecuteActionUsingDispatcher(action);
                    }
                };

                worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e) {
                    if (e.Error != null) OnFilteringError(this, new FilteringEventArgs(e.Error));
                };
                worker.RunWorkerAsync();
            }
            else {
                try {
                    ExecuteActionUsingDispatcher(action);
                }
                catch (Exception e) {
                    OnFilteringError(this, new FilteringEventArgs(e));
                }
            }
        }

        private void ExecuteActionUsingDispatcher(Action action) {
            if (CallingThreadDispatcher != null && !CallingThreadDispatcher.CheckAccess()) CallingThreadDispatcher.Invoke(new Action(() => { invoke(action); }));
            else invoke(action);
        }

        private static void invoke(Action action) {
            Trace.WriteLine("------------------ START APPLAY FILTER ------------------------------");
            var sw = Stopwatch.StartNew();

            action.Invoke();

            sw.Stop();
            Trace.WriteLine("TIME: " + sw.ElapsedMilliseconds);
            Trace.WriteLine("------------------ STOP APPLAY FILTER ------------------------------");
        }

        private bool ItemPassesFilter(object item) {
            return mFilteredCollectionHashSet.Contains(item);
        }

        #region Helpers

        private HashSet<object> initLookupDictionary(IEnumerable collection) {
            return collection != null ? new HashSet<object>(collection.Cast<object>() /*.ToList()*/) : new HashSet<object>();
        }

        #endregion

        #endregion

        #endregion

        #region Progress Notification

        public event EventHandler<EventArgs> FilteringStarted;
        public event EventHandler<EventArgs> FilteringFinished;
        public event EventHandler<FilteringEventArgs> FilteringError;

        private void OnFilteringStarted(object sender, EventArgs e) {
            var localEvent = FilteringStarted;

            localEvent?.Invoke(sender, e);
        }

        private void OnFilteringFinished(object sender, EventArgs e) {
            var localEvent = FilteringFinished;

            localEvent?.Invoke(sender, e);
        }

        private void OnFilteringError(object sender, FilteringEventArgs e) {
            var localEvent = FilteringError;

            localEvent?.Invoke(sender, e);
        }

        #endregion
    }
}