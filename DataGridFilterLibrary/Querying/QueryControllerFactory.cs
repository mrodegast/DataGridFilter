using System.Collections;
using System.Windows.Controls;
using System.Windows.Data;
using DataGridFilterLibrary.Support;

namespace DataGridFilterLibrary.Querying {
    public static class QueryControllerFactory {

        public static QueryController GetQueryController(DataGrid dataGrid, FilterData filterData, IEnumerable itemsSource) {
            var query = DataGridExtensions.GetDataGridFilterQueryController(dataGrid);

            if (query == null) {
                //clear the filter if exisits begin
                var view = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
                if (view != null) view.Filter = null;
                //clear the filter if exisits end

                query = new QueryController();
                DataGridExtensions.SetDataGridFilterQueryController(dataGrid, query);
            }

            query.ColumnFilterData = filterData;
            query.ItemsSource = itemsSource;
            query.CallingThreadDispatcher = dataGrid.Dispatcher;
            query.UseBackgroundWorker = DataGridExtensions.GetUseBackgroundWorkerForFiltering(dataGrid);
            return query;
        }
    }
}