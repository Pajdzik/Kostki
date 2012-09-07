using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

            loadSettings();
            initializeSkins();
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

        private void loadSettings()
        {
            AppMemory am = new AppMemory();

            nameBox.Text = am.GetValueOrDefault("playerName", "Player");
        }

        /// <summary>
        /// Funkcja dodaje do listy w opcjach skórki, które znajdzie w folderze
        /// </summary>
        private void initializeSkins()
        {
            List<string> skins = new List<string>();

            skins.Add("cards");

            this.skinList.ItemsSource = skins;
        }

        /// <summary>
        /// Metoda wywoływana przy zmianie stanu suwaka dźwięków na "On"
        /// </summary>
        private void SoundsChecked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("DZIAŁA");
        }

        /// <summary>
        /// Metoda wywoływana przy zmianie stanu suwaka dźwięków na "Off"
        /// </summary>
        private void SoundsUnchecked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("NIEDZIAŁA");
        }

        /// <summary>
        /// Metoda wywoływana przy zmianie stanu suwaka trybu gościa na "On"
        /// </summary>
        private void GuestModeChecked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("From now your score won't count in your statistics.");
        }

        /// <summary>
        /// Metoda wywoływana przy zmianie stanu suwaka trybu gościa na "Off"
        /// </summary>
        private void GuestModeUnchecked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("From now your score will count in your statistics.");
        }

        /// <summary>
        /// Metoda wywoływana przy wciskaniu dowolnego klawisza z klawiatury przy aktywnym kursorze w nameBox
        /// </summary>
        private void NameBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                AppMemory am = new AppMemory();
                am.SaveValue("playerName", nameBox.Text);

                MessageBox.Show("Player's name successfully changed to \"" + nameBox.Text + "\"");
            }
        }

        /// <summary>
        /// Metoda uruchamiająca się w momencia aktywowania kontrolki nameBox. Zaznacza tekst znajdujący się w text boksie.
        /// </summary>
        private void nameBoxMouseEnter(object sender, MouseEventArgs e)
        {
            if (String.IsNullOrEmpty(nameBox.Text) == false)
                nameBox.SelectAll();
        }
    }
}