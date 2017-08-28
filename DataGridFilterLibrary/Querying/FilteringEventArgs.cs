using System;

namespace DataGridFilterLibrary.Querying {
    public class FilteringEventArgs : EventArgs {

        public FilteringEventArgs(Exception ex) {
            Error = ex;
        }

        public Exception Error { get; }
    }
}