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
    enum Colors
    {
        Blue,
        Green,
        Red,
        Yellow
    }

    enum Figures
    {
        Club,
        Diamond,
        Heart,
        Spade
    }



    public partial class GamePage : PhoneApplicationPage
    {

        private List<Image> listOfImages = new List<Image>();
        private Image clubs, diamond, heart, spade;

        private Image[] currentCards;
        private Image[,] cards;

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

            currentCards = new Image[4];
            cards = new Image[4, 4];
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
            //cards[(int) Figures.Club, (int) Colors.Blue].Source = new BitmapImage(new Uri("/img/clubs/blue.png", UriKind.Relative));
            //cards[(int) Figures.Club, (int) Colors.Green].Source = new BitmapImage(new Uri("/img/clubs/green.png", UriKind.Relative));
            //cards[(int) Figures.Club, (int) Colors.Red].Source = new BitmapImage(new Uri("/img/clubs/red.png", UriKind.Relative));
            //cards[(int) Figures.Club, (int) Colors.Yellow].Source = new BitmapImage(new Uri("/img/clubs/yellow.png", UriKind.Relative));

            //cards[(int) Figures.Diamond, (int) Colors.Blue].Source = new BitmapImage(new Uri("/img/diamond/blue.png", UriKind.Relative));
            //cards[(int) Figures.Diamond, (int) Colors.Green].Source = new BitmapImage(new Uri("/img/diamond/green.png", UriKind.Relative));
            //cards[(int) Figures.Diamond, (int) Colors.Red].Source = new BitmapImage(new Uri("/img/diamond/red.png", UriKind.Relative));
            //cards[(int) Figures.Diamond, (int) Colors.Yellow].Source = new BitmapImage(new Uri("/img/diamond/yellow.png", UriKind.Relative));

            //cards[(int) Figures.Heart, (int) Colors.Blue].Source = new BitmapImage(new Uri("/img/heart/blue.png", UriKind.Relative));
            //cards[(int) Figures.Heart, (int) Colors.Green].Source = new BitmapImage(new Uri("/img/heart/green.png", UriKind.Relative));
            //cards[(int) Figures.Heart, (int) Colors.Red].Source = new BitmapImage(new Uri("/img/heart/red.png", UriKind.Relative));
            //cards[(int) Figures.Heart, (int) Colors.Yellow].Source = new BitmapImage(new Uri("/img/heart/yellow.png", UriKind.Relative));

            //cards[(int) Figures.Spade, (int) Colors.Blue].Source = new BitmapImage(new Uri("/img/spade/blue.png", UriKind.Relative));
            //cards[(int) Figures.Spade, (int) Colors.Green].Source = new BitmapImage(new Uri("/img/spade/green.png", UriKind.Relative));
            //cards[(int) Figures.Spade, (int) Colors.Red].Source = new BitmapImage(new Uri("/img/spade/red.png", UriKind.Relative));
            //cards[(int) Figures.Spade, (int) Colors.Yellow].Source = new BitmapImage(new Uri("/img/spade/yellow.png", UriKind.Relative));

            this.clubs.Source = new BitmapImage(new Uri("/img/clubs/red.png", UriKind.Relative));
            this.clubs.Height = 70;
            this.clubs.Width = 70;
            this.diamond.Source = new BitmapImage(new Uri("/img/diamond/blue.png", UriKind.Relative));
            this.diamond.Height = 70;
            this.diamond.Width = 70;
            this.heart.Source = new BitmapImage(new Uri("/img/heart/green.png", UriKind.Relative));
            this.heart.Height = 70;
            this.heart.Width = 70;
            this.spade.Source = new BitmapImage(new Uri("/img/spade/yellow.png", UriKind.Relative));
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
            Image image = (Image) sender;
            double x = Canvas.GetLeft(image);
            double y = Canvas.GetTop(image);

            x = 50 * ((int)x / 50);             // pseudozaokrąglanie do "siatki"
            y = 50 * ((int)y / 50);

            Canvas.SetLeft(image, x);
            Canvas.SetTop(image, y); 

            Debug.WriteLine("end");

        }
    }
}