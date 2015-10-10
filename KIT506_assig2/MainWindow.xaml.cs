using RAP.Controllers;
using RAP.Models;
using RAP.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RAP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            var combo = (ComboBox)FindName("ComboLevel");

            var items = new List<object>();
            items.Add("All");
            foreach (EmploymentLevel i in Enum.GetValues(typeof(EmploymentLevel)))
            {
                items.Add(i.ToString());
            }
            combo.ItemsSource = items;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            MessageBox.Show("aaa");
        }

        private void Btn_ShowPerf_Click(object sender, RoutedEventArgs e)
        {
            var window = new StaffPerfWindow();
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var provider = (ObjectDataProvider)App.Current.Resources["ResearchersProvider"];
                provider.MethodParameters[1] = ((TextBox)FindName("SearchBox")).Text;
            }
        }

    }
}
