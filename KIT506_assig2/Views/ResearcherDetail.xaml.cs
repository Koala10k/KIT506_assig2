using RAP.Models;
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

namespace RAP.Views
{
    /// <summary>
    /// Interaction logic for ResearcherDetail.xaml
    /// </summary>
    public partial class ResearcherDetail : UserControl
    {
        public ResearcherDetail()
        {
            InitializeComponent();
        }

        private void BtnShowNames_Click(object sender, RoutedEventArgs e)
        {
            Supervisions window = new Supervisions();
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        private void BtnCumCount_Click(object sender, RoutedEventArgs e)
        {
            var window = new CumulativeWindow();
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        private void Col_Click(object sender, RoutedEventArgs e)
        {
            var list = (ListView)FindName("PubList");
            var hName = (string)((GridViewColumnHeader)e.OriginalSource).Column.Header;
            if (hName.StartsWith("Year"))
            {
                IEnumerable<Publication> data = (IEnumerable<Publication>)list.ItemsSource;
                if (hName.EndsWith("↓"))
                {
                    list.ItemsSource = data.OrderBy(x => x.Year);
                    ((GridViewColumnHeader)e.OriginalSource).Column.Header = "Year↑";
                }
                else
                {
                    list.ItemsSource = data.OrderByDescending(x => x.Year);
                    ((GridViewColumnHeader)e.OriginalSource).Column.Header = "Year↓";
                }
            }
        }

        private void UCResearcherDetail_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var content = this.DataContext;
            var provider = (ObjectDataProvider)App.Current.Resources["StaffSupervisionsProvider"];
            provider.MethodParameters[0] = content;
            provider = (ObjectDataProvider)App.Current.Resources["CumulativeCountProvider"];
            provider.MethodParameters[0] = content;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0) return;
            var provider = (ObjectDataProvider)App.Current.Resources["SelectedPublicationProvider"];
            provider.MethodParameters[0] = e.AddedItems[0];
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (email.Content == null)
                MessageBox.Show("No address is available.");
            else
            {
                Clipboard.SetDataObject(email.Content);
                MessageBox.Show("The Email address has already been copied to your Clipboard.");
            }
        }

        private void PubList_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var w = (Researcher)e.NewValue;
            App.Current.Resources["FocusedResearcher"] = w;

            if (w == null || w.AllPublications.Count == 0) return;

            var start = w.AllPublications.Min(x => Int32.Parse(x.Year));
            var end = w.AllPublications.Max(x => Int32.Parse(x.Year));

            var items = new List<int>();
            for (int i = start; i <= end; i++)
            {
                items.Add(i);
            }

            var combStart = ((ComboBox)FindName("StartCombo"));
            var combEnd = ((ComboBox)FindName("EndCombo"));
            combStart.ItemsSource = items;
            combStart.SelectedIndex = 0;
            var items2 = items.Select(x => x).ToList();
            items2.Reverse();
            combEnd.ItemsSource = items2;
            combEnd.SelectedIndex = 0;

        }

        private void StartCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filterYear(0, e);
        }

        private void EndCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filterYear(0, e);
        }

        private void filterYear(int p, SelectionChangedEventArgs e)
        {
            var list = (ListView)FindName("PubList");

            try
            {
                var start = (int)((ComboBox)FindName("StartCombo")).SelectedItem;
                var end = (int)((ComboBox)FindName("EndCombo")).SelectedItem;

                var data = ((Researcher)App.Current.Resources["FocusedResearcher"]).AllPublications;

                list.ItemsSource = data.Where(x => Int32.Parse(x.Year) >= start && Int32.Parse(x.Year) <= end);
            }
            catch (Exception ex)
            {

            }
        }

    }
}
