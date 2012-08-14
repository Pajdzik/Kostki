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
using System.Diagnostics;
using Kostki.Exceptions;

namespace Kostki.Class
{
    public enum FourcardType 
    {
        Row,
        Column,
        Cross,
        Rectangle,
        Corners
    }

    public enum CardColors
    {
        Blue,
        Green,
        Red,
        Yellow,
        Joker
    }

    public enum PlaceType
    {
        Grid,
        Rand,
        Joker,
        Null
    }

    public enum Figures
    {
        Club,
        Diamond,
        Heart,
        Spade,
        Joker
    }

    public class ControlPanel
    {
        private BitmapImage[,] cards;
        private BitmapImage Joker;

        public readonly int addPanel = 60;
        public readonly int leftAndRight = 42;
        public readonly int cardSize = 86;
        public readonly int borderSize = 3;
        public readonly int spaceSize = 4;
        public readonly double opacityCoefficient = 0.5;
        public readonly double resizeCoefficient = 1.3;
        public readonly Point newCardGrid;
        public readonly Point grid4x4;

        public ControlPanel()
        {
            cards = new BitmapImage[4, 4];
            newCardGrid = new Point(45, 565);
            grid4x4 = new Point(45, 153);

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

            Joker = new BitmapImage(new Uri("/img/jokers/joker_violet.png", UriKind.Relative));
        }

        public int GetFieldSize()
        {
            return this.cardSize + this.borderSize * 2 ;
        }

        public Image GetJoker()
        {
            Image image = new Image();
            image.Source = this.Joker;
            image.Height = this.cardSize;
            image.Width = this.cardSize;
            return image;
        }

        public Image GetImageByColorAndId(Figures figure, CardColors cardColor)
        {
            Image image = new Image();
            image.Source = this.cards[(int)figure, (int)cardColor];
            image.Height = this.cardSize;
            image.Width = this.cardSize;
            return image;
        }

        public Image GetImageByColorAndId(int figure, int cardColor)
        {
            Image image = new Image();
            image.Source = this.cards[figure, cardColor];
            image.Height = this.cardSize;
            image.Width = this.cardSize;
            return image;
        }

        public Point GetTopJoker()
        {
            return new Point((double)this.leftAndRight,(double)this.addPanel + 4);
        }

        public Point GetTopGrid()
        {
            Point point = this.GetTopJoker();
            point.Y += 112;
            return point;
        }

        public Point GetTopRand()
        {
            Point point = this.GetTopGrid();
            point.Y += 412;
            return point;
        }

        public Rectangle GetRectangle()
        {
            Rectangle rect = new Rectangle();
            rect.Fill = new SolidColorBrush(Colors.Gray);
            rect.Height = 96;
            rect.Width = 96;

            return rect;
        }

        public Rectangle GetMarkRectangle()
        {
            Rectangle rect = new Rectangle();
            rect.Fill = new SolidColorBrush(Colors.White);
            rect.Opacity = 0.35;
            rect.Height = this.GetFieldSize();
            rect.Width = this.GetFieldSize();
            return rect;
        }

        public Point GetCoordsFromActualPoint(Point point, PlaceType place)
        {
            if (place == PlaceType.Grid)
            {
                return this.GetRowAndColumnFromViewportPoint(point, this.GetTopGrid().Y);
            }
            else if (place == PlaceType.Rand)
            {
                return this.GetRowAndColumnFromViewportPoint(point, this.GetTopRand().Y);
            }
            else
            {
                return this.GetRowAndColumnFromViewportPoint(point, this.GetTopJoker().Y);
            }
        }

        public Point GetViewportPointFromActualPoint(Point point)
        {
            PlaceType place = this.RecognizePlace(point);
            Point p = new Point();
            if (place == PlaceType.Grid)
            {
                p = this.GetRowAndColumnFromViewportPoint(point, this.GetTopGrid().Y);
                return this.GetGridCoordsForMarkRectangle((int)p.X, (int)p.Y);
            }
            else if (place == PlaceType.Rand)
            {
                p = this.GetRowAndColumnFromViewportPoint(point, this.GetTopRand().Y);
                return this.GetRandCoordsForMarkRectangle((int)p.X, (int)p.Y);
            }
            else
            {
                p = this.GetRowAndColumnFromViewportPoint(point, this.GetTopJoker().Y);
                return this.GetJokerCoordsForMarkRectangle((int)p.X, (int)p.Y);
            }
        }

        public Point GetJokerViewportPointFromCoords(int x, int y)
        {
            if (x == 0)
            {
                return new Point(GetTopJoker().X, GetTopJoker().Y);
            }
            else
            {
                return new Point(GetTopJoker().X + 100, GetTopJoker().Y);
            }
        }

        public Point GetJokerCoordsForMarkRectangle(int x, int y)
        {
            Point resultPoint = this.GetTopJoker();
            resultPoint.X += (x - 1) * 100 + 2;
            resultPoint.Y += (y - 1) * 100 + 2;

            return resultPoint;
        }

        public Point GetGridCoordsForMarkRectangle(int x, int y)
        {
            Point resultPoint = this.GetTopGrid();
            resultPoint.X += (x - 1) * 100 + 3;
            resultPoint.Y += (y - 1) * 100 + 3;

            return resultPoint;
        }

        public Point GetRandCoordsForMarkRectangle(int x, int y)
        {
            Point resultPoint = this.GetTopRand();
            resultPoint.X += (x - 1) * 100 + 2;
            resultPoint.Y += (y - 1) * 100 + 2;

            return resultPoint;
        }

        public PlaceType RecognizePlace(Point current)
        {
            double top, bottom;
            top = this.GetTopGrid().Y;
            bottom = top + 396;
            if (this.InsideRange(top, bottom, current.Y) && this.InsideRange(this.leftAndRight,
                   this.leftAndRight + 396, current.X))
            {
                return PlaceType.Grid;
            }
            top = this.GetTopRand().Y;
            bottom = top + 96;
            if (this.InsideRange(top, bottom, current.Y) && this.InsideRange(this.leftAndRight,
                this.leftAndRight + 396, current.X))
            {
                return PlaceType.Rand;
            }
            top = this.GetTopJoker().Y;
            bottom = top + 96;
            if (this.InsideRange(top, bottom, current.Y) && this.InsideRange(this.leftAndRight,
                this.leftAndRight + 188, current.X))
            {
                return PlaceType.Joker;
            }

            throw new OutOfBoardException();
                
        }

        public Point GetRowAndColumnFromViewportPoint(Point current, double top)
        {
            Point point = new Point();
            point.Y = this.CalculateGridRowAndColumn((int)(current.Y - top));
            point.X = this.CalculateGridRowAndColumn((int)(current.X - this.leftAndRight));
            return point;
        }

        public Boolean InsideRange(double first, double second, double value)
        {
            if (value > first && value < second)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int CalculateGridRowAndColumn(int y)
        {
            int result = (int)(y / 100);
            return result + 1;
        }
    }
}
