using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace DataGridFilterTest.TestData {
    public class TestDataGenerator : INotifyPropertyChanged {
        private static TestDataGenerator instance;

        private ObservableCollection<Employee> employeeList;
        private ObservableCollection<Employee> employeeListCopy;

        private ObservableCollection<EmployeePosition> employeePositionList;
        private ObservableCollection<EmployeeStatus> employeeStatusList;
        private bool isTestDataGenerationInProgress;
        private int numberOfRecordsToGenerate;
        private double testDataGenerationPercent;

        static TestDataGenerator() {
            instance = null;
        }

        private TestDataGenerator() {
            employeeList = new ObservableCollection<Employee>();
            employeePositionList = new ObservableCollection<EmployeePosition>();
            NumberOfRecordsToGenerate = 1000;
        }

        public static TestDataGenerator Instance => instance ?? (instance = new TestDataGenerator());

        public int NumberOfRecordsToGenerate {
            get => numberOfRecordsToGenerate;
            set {
                numberOfRecordsToGenerate = value;
                NotifyPropertyChanged("NumberOfRecordsToGenerate");
            }
        }

        public ObservableCollection<Employee> EmployeeList {
            get => employeeList;
            set {
                employeeList = value;
                NotifyPropertyChanged("EmployeeList");
            }
        }

        public ObservableCollection<Employee> EmployeeListCopy {
            get => employeeListCopy;
            set {
                employeeListCopy = value;
                NotifyPropertyChanged("EmployeeListCopy");
            }
        }

        public ObservableCollection<EmployeePosition> EmployeePositionList {
            get => employeePositionList;
            set {
                employeePositionList = value;
                NotifyPropertyChanged("EmployeePositionList");
            }
        }

        public ObservableCollection<EmployeeStatus> EmployeeStatuses {
            get => employeeStatusList;
            set {
                employeeStatusList = value;
                NotifyPropertyChanged("EmployeeStatuses");
            }
        }

        public double TestDataGenerationPercent {
            get => testDataGenerationPercent;
            set {
                testDataGenerationPercent = value;
                NotifyPropertyChanged("TestDataGenerationPercent");
            }
        }

        public bool IsTestDataGenerationInProgress {
            get => isTestDataGenerationInProgress;
            set {
                isTestDataGenerationInProgress = value;
                NotifyPropertyChanged("IsTestDataGenerationInProgress");
            }
        }

        public void GenerateTestData(Action<EventArgs> callback) {
            if (NumberOfRecordsToGenerate > 0) {
                IsTestDataGenerationInProgress = true;
                var worker = new BackgroundWorker();
                var list = new List<Employee>();

                worker.DoWork += delegate {
                    for (var i = 0; i < NumberOfRecordsToGenerate; i++) {
                        InitRandomGenerator();
                        var emp = new Employee();

                        FillWithTheRandomData(emp, i);
                        list.Add(emp);
                        TestDataGenerationPercent = (double) i / NumberOfRecordsToGenerate * 100;
                    }
                };

                worker.RunWorkerCompleted += delegate {
                    EmployeeStatuses = new ObservableCollection<EmployeeStatus>(STATUS.ToList());
                    EmployeePositionList = new ObservableCollection<EmployeePosition>(POSITIONS.ToList());
                    EmployeeList = new ObservableCollection<Employee>(list);

                    var tmpArray = new Employee[EmployeeList.Count];
                    EmployeeList.ToList().CopyTo(tmpArray);
                    EmployeeListCopy = new ObservableCollection<Employee>(tmpArray.ToList());

                    IsTestDataGenerationInProgress = false;
                    callback?.Invoke(EventArgs.Empty);
                };
                worker.RunWorkerAsync();
            }
        }

        public void InsertNewEmployeeWithNewPosition() {
            var newPosition = new EmployeePosition {Id = EmployeePositionList.Max(p => p.Id) + 1, Name = "Position " + (EmployeePositionList.Max(p => p.Id) + 1)};

            EmployeePositionList.Add(newPosition);
            NotifyPropertyChanged("EmployeePositionList");

            var emp = new Employee();
            var employeeId = EmployeeList.Max(e => e.Id) + 1;

            FillWithTheRandomData(emp, employeeId);

            emp.Position = newPosition;

            EmployeeList.Add(emp);
            NotifyPropertyChanged("EmployeeList");
        }

        #region Internal - random data generation

        private Random random;

        private void InitRandomGenerator() {
            random = new Random((int) DateTime.Now.Ticks);
            random = new Random((int) new DateTime(random.Next(DateTime.MinValue.Year, DateTime.MaxValue.Year), random.Next(1, 12), random.Next(1, 28)).Ticks);

            Thread.Sleep(2);
        }

        private void FillWithTheRandomData(Employee e, int i) {
            e.Id = i;

            e.Name = GetRandomName();
            e.Email = GetRandomEmail(e.Name);
            e.Address = GetRandomAddress();
            e.EmployeeGuid = getEmployeeGuid(i);
            e.WorkExperience = random.Next(0, 40);
            e.Position = i % 13 == 0 ? null : GetRadndomPosition();
            e.EmployeeStatusId = GetRandomStatusId();
            e.IsInterviewed = GetRandomIsInterviewed();
            e.DateOfBirth = GetRandomDateOfBirth();
        }

        private string GetRandomName() {
            var number = random.Next(NAMES.Length - 1);
            return NAMES[number];
        }

        private string GetRandomEmail(string email) {
            return email + "-" + getRandomString(3) + "@" + getRandomDomain();
        }

        private string GetRandomAddress() {
            return getRandomString(random.Next(5, 10)) + ", " + getRandomString(random.Next(5, 15)) + " " + random.Next(0, 200);
        }

        private Guid? getEmployeeGuid(int i) {
            Guid? value;

            if (i % 10 == 0) value = null;
            else value = Guid.NewGuid();

            return value;
        }

        private EmployeePosition GetRadndomPosition() {
            var number = random.Next(POSITIONS.Length - 1);

            return POSITIONS[number];
        }

        private int GetRandomStatusId() {
            return random.Next(1, STATUS.Length);
        }

        private bool GetRandomIsInterviewed() {
            return !(random.NextDouble() < 0.5);
        }

        private DateTime GetRandomDateOfBirth() {
            return new DateTime(random.Next(1950, 1990), random.Next(1, 12), random.Next(1, 28));
        }

        private readonly string[] NAMES = {
            "Mark", "Tom", "Harry", "Sally", "Sandra", "Paul", "Anastasia", "David", "Alex", "Michael", "Tina", "Zachary", "Bob", "Elise", "Jime", "Anderry", "Rustin", "Ivadon", "Nichardo", "Jasey", "Rent", "Millack", "Alenn", "Serrett", "Tanifer", "Syllica", "Allickie", "Jacey",
            "Janther", "Racey", "Alicherry", "Clary", "Kather", "Bonna"
        };

        private readonly string[] DOMAINS = {"xxx.com", "aa.com", "min.com", "erp.com", "holidays.com", "mon.com", "san.com", "sun.com", "ibm.com", "hp.com", "google.com", "yahoo.com", "bing.com", "ask.com"};

        private readonly string ASCII = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        private readonly EmployeeStatus[] STATUS = {new EmployeeStatus {Id = 1, Name = "Available"}, new EmployeeStatus {Id = 2, Name = "Not Available"}, new EmployeeStatus {Id = 3, Name = "Undefined"}};

        private readonly EmployeePosition[] POSITIONS = {
            new EmployeePosition {Id = 1, Name = "EAP Specialist"}, new EmployeePosition {Id = 2, Name = "Instructor"}, new EmployeePosition {Id = 3, Name = "Full professor"}, new EmployeePosition {Id = 4, Name = "ERP Specialist"},
            new EmployeePosition {Id = 5, Name = "SQL Programmer"}, new EmployeePosition {Id = 6, Name = "QA Tester"}, new EmployeePosition {Id = 7, Name = "Senior Software Engineer "}, new EmployeePosition {Id = 8, Name = "Technical Analyst"},
            new EmployeePosition {Id = 9, Name = "Web Master"}, new EmployeePosition {Id = 10, Name = "Programmer Analyst "}
        };

        private string getRandomDomain() {
            var number = random.Next(DOMAINS.Length - 1);
            return NAMES[number];
        }

        private char getRandomChar() {
            var number = random.Next(ASCII.Length - 1);
            return ASCII[number];
        }

        private string getRandomString(int length) {
            var randomString = new StringBuilder();
            for (var i = 0; i < length; i++) randomString.Append(getRandomChar());
            return randomString.ToString();
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}