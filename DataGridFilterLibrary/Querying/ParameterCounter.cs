namespace DataGridFilterLibrary.Querying {
    public class ParameterCounter {
        public ParameterCounter() { }

        public ParameterCounter(int count) {
            this.Count = count;
        }

        public int ParameterNumber => Count - 1;

        private int Count { get; set; }

        public void Increment() {
            Count++;
        }

        public void Decrement() {
            Count--;
        }

        public override string ToString() {
            return ParameterNumber.ToString();
        }
    }
}