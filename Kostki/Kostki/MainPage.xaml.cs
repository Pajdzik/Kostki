using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

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
            if (App.MainMenuItemModel.IsDataLoaded == false)
            {
                App.MainMenuItemModel.LoadData();
            } 
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem listBoxItem = ((sender as ListBox).SelectedItem as ListBoxItem);

            NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));

        }
    }
}