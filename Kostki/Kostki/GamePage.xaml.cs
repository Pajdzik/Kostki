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
using Kostki.Class;

namespace Kostki
{
    public partial class GamePage : PhoneApplicationPage
    {
        private Image clubs, diamond, heart, spade;

        private ControlPanel controlPanel;
        private Point currentPosition = new Point();
        private Point startPosition = new Point();

        public GamePage()
        {
            this.controlPanel = new ControlPanel();
            InitializeComponent();
            this.Init();
            this.ShowRectangle();
            this.Loaded += new RoutedEventHandler(GamePageLoaded);
        }

        public void Init()
        {
            this.clubs = this.controlPanel.GetImageByColorAndId(Figures.Diamond, CardColors.Blue);
            this.heart = this.controlPanel.GetImageByColorAndId(Figures.Diamond, CardColors.Blue);
            this.spade = this.controlPanel.GetImageByColorAndId(Figures.Diamond, CardColors.Blue);
            this.diamond = this.controlPanel.GetImageByColorAndId(Figures.Diamond, CardColors.Blue);
        }

        private void GamePageLoaded(object sender, RoutedEventArgs e)
        {
            this.showCards();
        }

        public void ShowRectangle() 
        {
            this.ShowRectangleForJoker();
            this.ShowRectangleForGrid();
            this.ShowRectangleForRand();
        }

        public void ShowRectangleForJoker()
        {
            currentPosition = startPosition = controlPanel.GetTopJoker();
            for (int j = 0; j < 2; j++)
            {
                Rectangle rect = controlPanel.GetRectangle();
                this.canvas.Children.Add(rect);
                Canvas.SetLeft(rect, currentPosition.X);
                Canvas.SetTop(rect, currentPosition.Y);
                currentPosition.X += 100;
            }
        }

        public void ShowRectangleForGrid()
        {
            currentPosition = startPosition = controlPanel.GetTopGrid();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Rectangle rect = controlPanel.GetRectangle();
                    this.canvas.Children.Add(rect);
                    Canvas.SetLeft(rect, currentPosition.X);
                    Canvas.SetTop(rect, currentPosition.Y);
                    currentPosition.X += 100;
                }
                currentPosition.X = startPosition.X;
                currentPosition.Y += 100;
            }
        }

        public void ShowRectangleForRand()
        {
            currentPosition = startPosition = controlPanel.GetTopRand();
            for (int j = 0; j < 4; j++)
            {
                Rectangle rect = controlPanel.GetRectangle();
                this.canvas.Children.Add(rect);
                Canvas.SetLeft(rect, currentPosition.X);
                Canvas.SetTop(rect, currentPosition.Y);
                currentPosition.X += 100;
            }
        }

        public void showCards() //tymczasowa funkcja
        {
            Random r = new Random();           

            for (int i = 0; i < 4; i++) 
            {
                Image image = this.controlPanel.GetImageByColorAndId(r.Next(4), r.Next(4));
                image.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(ManipulationStarted);
                image.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(ManipulationDelta);
                image.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(ManipulationCompleted);
                this.canvas.Children.Add(image);

                Canvas.SetLeft(image, controlPanel.newCardGrid.X + (i * 100));
                Canvas.SetTop(image, controlPanel.newCardGrid.Y);
            }
        }



        private void ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            Image image = (Image)sender;

            Canvas.SetLeft(image, (double)((int)(Canvas.GetLeft(image) - 0.15 * image.Width)));
            Canvas.SetTop(image, (double)((int)(Canvas.GetTop(image) - 0.15 * image.Width)));

            image.Opacity = controlPanel.opacityCoefficient;                    // ustawienie półprzezroczystości
            image.Height = image.Width = controlPanel.cardSize * controlPanel.resizeCoefficient;       // zwiększenie rozmiaru
        }

        private void ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            Image image = (Image)sender;
            Canvas.SetLeft(image, Canvas.GetLeft(image) + e.DeltaManipulation.Translation.X);
            Canvas.SetTop(image, Canvas.GetTop(image) + e.DeltaManipulation.Translation.Y); 
        }

        private void ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Image image = (Image) sender;

            Canvas.SetLeft(image, (double)((int)(Canvas.GetLeft(image) + 0.15 * image.Width)));
            Canvas.SetTop(image, (double)((int)(Canvas.GetTop(image) + 0.15 * image.Width)));

            image.Opacity = 1.0;                    // ustawienie pełnej widoczności z powrotem
            image.Height = image.Width = controlPanel.cardSize;     // ustawienie rozmiarów z powrotem

            Canvas.SetLeft(image, (double)((int)(Canvas.GetLeft(image) / this.controlPanel.GetFieldSize()) * this.controlPanel.GetFieldSize()));
            Canvas.SetTop(image, (double)((int)(Canvas.GetTop(image) / this.controlPanel.GetFieldSize()) * this.controlPanel.GetFieldSize()));
        }
    }
}