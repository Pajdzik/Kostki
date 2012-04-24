using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace Kostki.Class
{
    public enum CardColors
    {
        Blue,
        Green,
        Red,
        Yellow
    }

    public enum Figures
    {
        Club,
        Diamond,
        Heart,
        Spade
    }

    public class ControlPanel
    {
        public int cardSize = 70;
        public int borderSize = 5;
        private BitmapImage[,] cards;

        public ControlPanel()
        {
            cards = new BitmapImage[4, 4];
            this.LoadCards();
        }

        private void LoadCards()
        {
            cards[(int) Figures.Club, (int) CardColors.Blue] = new BitmapImage(new Uri("/img/clubs/blue.png", UriKind.Relative));
            cards[(int) Figures.Club, (int) CardColors.Green] = new BitmapImage(new Uri("/img/clubs/green.png", UriKind.Relative));
            cards[(int) Figures.Club, (int) CardColors.Red] = new BitmapImage(new Uri("/img/clubs/red.png", UriKind.Relative));
            cards[(int) Figures.Club, (int) CardColors.Yellow] = new BitmapImage(new Uri("/img/clubs/yellow.png", UriKind.Relative));

            cards[(int) Figures.Diamond, (int) CardColors.Blue] = new BitmapImage(new Uri("/img/diamond/blue.png", UriKind.Relative));
            cards[(int) Figures.Diamond, (int) CardColors.Green] = new BitmapImage(new Uri("/img/diamond/green.png", UriKind.Relative));
            cards[(int) Figures.Diamond, (int) CardColors.Red] = new BitmapImage(new Uri("/img/diamond/red.png", UriKind.Relative));
            cards[(int) Figures.Diamond, (int) CardColors.Yellow] = new BitmapImage(new Uri("/img/diamond/yellow.png", UriKind.Relative));

            cards[(int) Figures.Heart, (int) CardColors.Blue] = new BitmapImage(new Uri("/img/heart/blue.png", UriKind.Relative));
            cards[(int) Figures.Heart, (int) CardColors.Green] = new BitmapImage(new Uri("/img/heart/green.png", UriKind.Relative));
            cards[(int) Figures.Heart, (int) CardColors.Red] = new BitmapImage(new Uri("/img/heart/red.png", UriKind.Relative));
            cards[(int) Figures.Heart, (int) CardColors.Yellow] = new BitmapImage(new Uri("/img/heart/yellow.png", UriKind.Relative));

            cards[(int) Figures.Spade, (int) CardColors.Blue]= new BitmapImage(new Uri("/img/spade/blue.png", UriKind.Relative));
            cards[(int) Figures.Spade, (int) CardColors.Green ] = new BitmapImage(new Uri("/img/spade/green.png", UriKind.Relative));
            cards[(int) Figures.Spade, (int) CardColors.Red] = new BitmapImage(new Uri("/img/spade/red.png", UriKind.Relative));
            cards[(int) Figures.Spade, (int) CardColors.Yellow] = new BitmapImage(new Uri("/img/spade/yellow.png", UriKind.Relative));
        }

        public int GetFieldSize()
        {
            return this.cardSize + this.borderSize;
        }

        public Image GetImageByColorAndId(Figures figure, CardColors cardColor)
        {
            Image image = new Image();
            image.Source = this.cards[(int)figure, (int)cardColor];
            image.Height = this.cardSize;
            image.Width = this.cardSize;
            return image;
        }
    }
}
