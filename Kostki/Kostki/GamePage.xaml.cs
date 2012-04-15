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
using System.Windows.Media.Imaging;

namespace Kostki
{
    public partial class GamePage : PhoneApplicationPage
    {

        private List<Image> listOfImages = new List<Image>();
        private Image clubs, diamond, heart, spade;

        public GamePage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(GamePageLoaded);
        }

        private void GamePageLoaded(object sender, RoutedEventArgs e)
        {
            this.LoadCardsImage();
            this.showCards();
        }

        ///
        /// Loaded cards images
        /// 
        public void LoadCardsImage()
        {
            this.clubs.Source = new BitmapImage(new Uri("/img/clubs.png", UriKind.Relative));
           // this.diamond.Source = new BitmapImage(new Uri("/img/diamond.png", UriKind.Relative));
            //this.heart.Source = new BitmapImage(new Uri("/img/heart.png", UriKind.Relative));
           // this.spade.Source = new BitmapImage(new Uri("/img/spade.png", UriKind.Relative));
        }

        public void showCards()
        {
            this.canvas.Children.Add(heart);
            this.canvas.Children.Add(spade);
            this.canvas.Children.Add(diamond);
            this.canvas.Children.Add(clubs);

            Canvas.SetLeft(spade, 50);
            Canvas.SetLeft(diamond, 100);
            Canvas.SetLeft(clubs, 150);
        }

        /*
            Image image = new Image();
            image.Source = new BitmapImage(new Uri("/heart.png", UriKind.Relative)) ;
            this.canvas.Children.Add(image);
    */}
}