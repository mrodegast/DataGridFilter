using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using DataGridFilterTest.TestData;

namespace DataGridFilterTest {
    /// <summary>
    ///     Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window {
        private readonly DispatcherTimer timer = new DispatcherTimer();

        public Window1() {
            InitializeComponent();

            if (MyObjectDataProvider != null) MyObjectDataProvider.ObjectInstance = TestDataGenerator.Instance;

            DataContext = MyObjectDataProvider;

            //this.DataContext = TestData.TestDataGenerator.Instance;

            TestDataGenerator.Instance.GenerateTestData(null);
        }

        private ObjectDataProvider MyObjectDataProvider => TryFindResource("EmployeeData") as ObjectDataProvider;

        private void Button_Click(object sender, RoutedEventArgs e) {
            TestDataGenerator.Instance.GenerateTestData(null);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            double newSize;

            if (double.TryParse(txtFontSize.Text, out newSize)) FontSize = newSize;
        }

        private void Button_Click_Insert_New_Position(object sender, RoutedEventArgs e) {
            var button = sender as Button;

            if (button?.Content.ToString() == "Start inserting Employees with new position") {
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Tick += delegate { TestDataGenerator.Instance.InsertNewEmployeeWithNewPosition(); };

                button.Content = "Stop inserting Employees";
                button.Background = Brushes.Red;

                timer.Start();
            }
            else {
                timer.Stop();
                if (button != null) {
                    button.Content = "Start inserting Employees with new position";
                    button.Background = Brushes.Transparent;
                }
            }
        }
    }
}