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
using System.Diagnostics;

namespace Kostki
{
    public partial class GamePage : PhoneApplicationPage
    {

        private List<Image> listOfImages = new List<Image>();
        private Image clubs, diamond, heart, spade;

        public GamePage()
        {

            InitializeComponent();
            this.Init();
            this.Loaded += new RoutedEventHandler(GamePageLoaded);
        }

        public void Init()
        {
            this.clubs = new Image();
            this.heart = new Image();
            this.spade = new Image();
            this.diamond = new Image();
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
            this.clubs.Height = 70;
            this.clubs.Width = 70;
            this.diamond.Source = new BitmapImage(new Uri("/img/diamond.png", UriKind.Relative));
            this.diamond.Height = 70;
            this.diamond.Width = 70;
            this.heart.Source = new BitmapImage(new Uri("/img/heart.png", UriKind.Relative));
            this.heart.Height = 70;
            this.heart.Width = 70;
            this.spade.Source = new BitmapImage(new Uri("/img/spade.png", UriKind.Relative));
            this.spade.Height = 70;
            this.spade.Width = 70;
        }

        public void showCards()
        {

            this.heart.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(ManipulationStarted);
            this.heart.ManipulationDelta +=new EventHandler<ManipulationDeltaEventArgs>(ManipulationDelta);
            this.heart.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(ManipulationCompleted);

            this.spade.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(ManipulationStarted);
            this.spade.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(ManipulationDelta);
            this.spade.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(ManipulationCompleted);

            this.diamond.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(ManipulationStarted);
            this.diamond.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(ManipulationDelta);
            this.diamond.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(ManipulationCompleted);

            this.clubs.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(ManipulationStarted);
            this.clubs.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(ManipulationDelta);
            this.clubs.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(ManipulationCompleted);

             this.canvas.Children.Add(heart);
             this.canvas.Children.Add(spade);
             this.canvas.Children.Add(diamond);
             this.canvas.Children.Add(clubs);

             Canvas.SetLeft(spade, 71);
             Canvas.SetLeft(diamond, 142);
             Canvas.SetLeft(clubs, 213);
        }



        private void ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            Debug.WriteLine("start");
        }

        private void ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            Image image = (Image)sender;

            Debug.WriteLine(e.DeltaManipulation.Translation.X + " " + e.DeltaManipulation.Translation.Y);

            double x = Canvas.GetLeft(image);
            double y = Canvas.GetTop(image);

            Canvas.SetLeft(image, x + e.DeltaManipulation.Translation.X);
            Canvas.SetTop(image, y + e.DeltaManipulation.Translation.Y); 


            Debug.WriteLine("w trakcie");

        }

        private void ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Image image = (Image)sender;
            Debug.WriteLine("end");

        }
    }
}