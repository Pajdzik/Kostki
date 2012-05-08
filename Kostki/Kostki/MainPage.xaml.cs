using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Kostki;
using System.Diagnostics;

namespace Kostki
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.MainMenuItemModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(App.MainMenuItemModel.IsDataLoaded.ToString());
            if (App.MainMenuItemModel.IsDataLoaded == false)
            {
             //   Debug.WriteLine("IsDataLoDed = false");
                App.MainMenuItemModel.LoadData();
            }

           // NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem listBoxItem = ((sender as ListBox).SelectedItem as ListBoxItem);

            NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));

        }
    }
}