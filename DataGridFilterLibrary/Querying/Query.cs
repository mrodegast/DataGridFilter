using System.Collections.Generic;
using System.Linq;

namespace DataGridFilterLibrary.Querying {
    public class Query {

        public Query() {
            LastFilterString = string.Empty;
            LastQueryParameters = new List<object>();
        }

        public string FilterString { get; set; }
        public List<object> QueryParameters { get; set; }

        private string LastFilterString { get; set; }
        private List<object> LastQueryParameters { get; set; }

        public bool IsQueryChanged {
            get {
                var queryChanged = false;
                if (FilterString != LastFilterString) {
                    queryChanged = true;
                }
                else {
                    if (QueryParameters.Count != LastQueryParameters.Count) queryChanged = true;
                    else
                        if (QueryParameters.Where((t, i) => !t.Equals(LastQueryParameters[i])).Any()) {
                            queryChanged = true;
                        }
                }
                return queryChanged;
            }
        }

        public void StoreLastUsedValues() {
            LastFilterString = FilterString;
            LastQueryParameters = QueryParameters;
        }
    }
}