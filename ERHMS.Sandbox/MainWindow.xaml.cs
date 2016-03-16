using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;

namespace ERHMS.Sandbox
{
    public partial class MainWindow : Window
    {
        private class Surveillance : ViewEntity
        {
            public string FirstName
            {
                get { return GetProperty<string>("FirstName"); }
                set { SetProperty("FirstName", value); }
            }

            public string LastName
            {
                get { return GetProperty<string>("LastName"); }
                set { SetProperty("LastName", value); }
            }

            public DateTime? BirthDate
            {
                get { return GetProperty<DateTime?>("BirthDate"); }
                set { SetProperty("BirthDate", value); }
            }

            public double? Year
            {
                get { return GetProperty<double?>("Year"); }
                set { SetProperty("Year", value); }
            }

            public double? Week
            {
                get { return GetProperty<double?>("Week"); }
                set { SetProperty("Week", value); }
            }

            public bool? Pregnant
            {
                get { return GetProperty<bool?>("Pregnant"); }
                set { SetProperty("Pregnant", value); }
            }
        }

        private IDataDriver driver;
        private ViewRepository<Surveillance> surveillances;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Epi Info 7 Sample Database (Sample.mdb)|Sample.mdb";
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                driver = AccessDriver.Create(dialog.FileName);
                surveillances = new ViewRepository<Surveillance>(driver, "Surveillance");
            }
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            Data.ItemsSource = surveillances.Select();
        }

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            Surveillance surveillance = surveillances.Create();
            surveillance.FirstName = "Steven";
            surveillance.LastName = "Williams";
            surveillance.BirthDate = new DateTime(1984, 10, 7);
            surveillance.Year = 2007;
            surveillance.Week = new Random().Next(1, 52);
            surveillance.Pregnant = false;
            surveillances.Save(surveillance);
            MessageBox.Show(surveillance.UniqueKey.ToString());
        }

        private Surveillance GetSurveillanceByName(string firstName, string lastName)
        {
            DataParameterCollection parameters = new DataParameterCollection(driver);
            parameters.AddByValue(firstName);
            parameters.AddByValue(lastName);
            string sql = parameters.Format("FirstName = {0} AND LastName = {1}");
            return surveillances.Select(new DataPredicate(sql, parameters)).Single();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            Surveillance surveillance = GetSurveillanceByName("John", "Smith");
            surveillance.Week = new Random().Next(1, 52);
            surveillances.Save(surveillance);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Surveillance surveillance = GetSurveillanceByName("John", "Smith");
            surveillances.Delete(surveillance);
        }

        private void Undelete_Click(object sender, RoutedEventArgs e)
        {
            Surveillance surveillance = GetSurveillanceByName("John", "Smith");
            surveillances.Undelete(surveillance);
        }
    }
}
