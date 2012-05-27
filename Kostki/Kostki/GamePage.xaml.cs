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
using Kostki.Exceptions;
using System.Reflection.Emit;

namespace Kostki
{
    public partial class GamePage : PhoneApplicationPage
    {
        private ControlPanel controlPanel;
        private Point currentPosition = new Point();
        private Point startPosition = new Point();
        private Point startPoint = new Point(); // punkt startowy karty, przed wciśnięciem
        private Point startCoords = new Point();
        private Point endCoords = new Point();
        private PlaceType startPlaceType;
        private PlaceType endPlaceType;
        private Rectangle opacityRect = null;
        private Game game = null;
        private Checker checker;
        private Calculate calculate;
        private TextBlock textblock;
        private Image[] jokers;
        private Boolean isItJoker;

        public GamePage()
        {
            this.calculate = new Calculate();
            this.checker = new Checker();
            this.controlPanel = new ControlPanel();
            this.game = new Game(controlPanel);
            InitializeComponent();
            this.ShowRectangle();
            this.Loaded += new RoutedEventHandler(GamePageLoaded);
            CreateTextBlockForResult();
        }

        public void CreateTextBlockForResult()
        {
            textblock = new TextBlock();
            textblock.Text = "0";
            textblock.FontSize = 40;
            canvas.Children.Add(textblock);
            Canvas.SetLeft(textblock, (double)(this.controlPanel.GetTopJoker().X + 220));
            Canvas.SetTop(textblock, (double)(this.controlPanel.GetTopJoker().Y + 15));
        }

        private void GamePageLoaded(object sender, RoutedEventArgs e)
        {
            this.showCards();
            //AddJoker();
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

        public void AddJoker()
        {
            if (jokers != null)
            {
                canvas.Children.Remove(jokers[0]);
                canvas.Children.Remove(jokers[1]);
            }
            jokers = new Image[2];
            Point jokerPoint = new Point();

            for (int i = 0; i < 2; i++)
            {
                jokers[i] = controlPanel.GetJoker();
                canvas.Children.Add(jokers[i]);
                jokerPoint = controlPanel.GetJokerCoordsForMarkRectangle(i + 1, 1);
                Canvas.SetLeft(jokers[i], jokerPoint.X+3);
                Canvas.SetTop(jokers[i], jokerPoint.Y + 3);
                jokers[i].ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(ManipulationStarted);
                jokers[i].ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(ManipulationDelta);
                jokers[i].ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(ManipulationCompleted);
                game.AddJoker(jokers[i], i);
            }

        }

        public void showCards() //tymczasowa funkcja
        {
            Random r = new Random();
            Point point = new Point();

            CardImage[] cardImage = this.game.RandNewCards();

            for (int i = 0; i < cardImage.Count(); i++)
            {
                cardImage[i].image.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(ManipulationStarted);
                cardImage[i].image.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(ManipulationDelta);
                cardImage[i].image.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(ManipulationCompleted);
                this.canvas.Children.Add(cardImage[i].image);
                point = controlPanel.GetRandCoordsForMarkRectangle(i + 1, 1);
                Canvas.SetLeft(cardImage[i].image, point.X + 3);
                Canvas.SetTop(cardImage[i].image, point.Y + 3);
            }            
        }



        private void ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
          
            Image image = (Image)sender;
            this.startPoint = new Point(Canvas.GetLeft(image), Canvas.GetTop(image));

            Canvas.SetLeft(image, (double)((int)(Canvas.GetLeft(image) - 0.15 * image.Width)));
            Canvas.SetTop(image, (double)((int)(Canvas.GetTop(image) - 0.15 * image.Width)));

            //testowy fragment kodu
            PlaceType place = this.controlPanel.RecognizePlace(new Point(Canvas.GetLeft(image) + (controlPanel.cardSize * 1.3) / 2, Canvas.GetTop(image) + (controlPanel.cardSize * 1.3) / 2));
            this.startCoords = this.controlPanel.GetCoordsFromActualPoint(new Point(Canvas.GetLeft(image) + (controlPanel.cardSize * 1.3) / 2, Canvas.GetTop(image) + (controlPanel.cardSize * 1.3) / 2), place);
            this.startPlaceType = place;

            isItJoker = game.GetJokerOnCoords(place,(int)(startCoords.X - 1), (int)(startCoords.Y - 1));

            Debug.WriteLine("place type " + place);
            Debug.WriteLine("X: " + startCoords.X + "Y : " + startCoords.Y);

            Debug.WriteLine(this.startPlaceType);

            //koniec testowego fragmentu
            //if ((place == PlaceType.Rand) && (place == PlaceType.Grid && game.IsFieldBlocked((int)startCoords.X, (int)startCoords.Y) == false)) { }


            Canvas.SetZIndex((UIElement)sender, 1);        // ustawienie z-indeksu trzymanego obrazka na wierzch                          

            image.Opacity = controlPanel.opacityCoefficient;                    // ustawienie półprzezroczystości
            image.Height = image.Width = controlPanel.cardSize * controlPanel.resizeCoefficient;       // zwiększenie rozmiaru
            
        }

        private void ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            Image image = (Image)sender;
            Canvas.SetLeft(image, Canvas.GetLeft(image) + e.DeltaManipulation.Translation.X);
            Canvas.SetTop(image, Canvas.GetTop(image) + e.DeltaManipulation.Translation.Y);
            PlaceType place;
            try
            {
                place = this.controlPanel.RecognizePlace(new Point(Canvas.GetLeft(image) + (controlPanel.cardSize * 1.3) / 2, Canvas.GetTop(image) + (controlPanel.cardSize * 1.3) / 2));
                this.endCoords = this.controlPanel.GetCoordsFromActualPoint(new Point(Canvas.GetLeft(image) + (controlPanel.cardSize * 1.3) / 2, Canvas.GetTop(image) + (controlPanel.cardSize * 1.3) / 2), place);
                this.endPlaceType = place;
            }
            catch (OutOfBoardException)
            {
                canvas.Children.Remove(this.opacityRect);
                this.opacityRect = null;
                return;
            }
            Debug.WriteLine(place + " " + endCoords.X + " " + endCoords.Y);

            try
            {
                canvas.Children.Remove(this.opacityRect);
                Point point = controlPanel.GetViewportPointFromActualPoint(new Point(Canvas.GetLeft(image)+(controlPanel.cardSize*1.3)/2, Canvas.GetTop(image)+(controlPanel.cardSize*1.3)/2));
                if (this.game.IsFieldFree((int)endCoords.X, (int)endCoords.Y, this.endPlaceType) == true || isItJoker == true)            // wyłączenie podświetlenia kafelka gdy jest zajęty
                {
                    this.opacityRect = controlPanel.GetMarkRectangle();
                    canvas.Children.Add(opacityRect);
                    Canvas.SetLeft(opacityRect, point.X);
                    Canvas.SetTop(opacityRect, point.Y);
                }
                else if (endPlaceType == PlaceType.Joker && isItJoker == true)
                {
                    opacityRect = controlPanel.GetMarkRectangle();
                    canvas.Children.Add(opacityRect);
                    Point jokerPoint = controlPanel.GetJokerViewportPointFromCoords((int)(endCoords.X - 1), (int)(endCoords.Y));
                    Canvas.SetLeft(opacityRect, jokerPoint.X + 3);
                    Canvas.SetTop(opacityRect, jokerPoint.Y + 3);
                }
            }
            catch (NullReferenceException)
            {
                this.opacityRect = null;
            }
       }

        private void ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Image image = (Image) sender;

            Canvas.SetLeft(image, (double)((int)(Canvas.GetLeft(image) + 0.15 * image.Width)));
            Canvas.SetTop(image, (double)((int)(Canvas.GetTop(image) + 0.15 * image.Width)));

            image.Opacity = 1.0;                    // ustawienie pełnej widoczności z powrotem
            image.Height = image.Width = controlPanel.cardSize;     // ustawienie rozmiarów z powrotem
            Canvas.SetZIndex((UIElement)sender, 0);         // przesunięcie kafelka wartstwę niżej

            if (this.opacityRect != null)
            {
                if (isItJoker == true)
                {

                    if ((int)(startCoords.X - 1) == (int)(endCoords.X - 1) && (int)(startCoords.Y - 1) == (int)(endCoords.Y - 1) && startPlaceType == endPlaceType)
                    {
                        Canvas.SetLeft(image, this.startPoint.X);
                        Canvas.SetTop(image, this.startPoint.Y);
                        canvas.Children.Remove(this.opacityRect);
                        this.opacityRect = null;
                        return;
                    }

                    Image forSwapFirst = game.GetImageOnCoords(startPlaceType, (int)(startCoords.X - 1), (int)(startCoords.Y - 1));
                    Image forSwapSecond;
                    canvas.Children.Remove(forSwapFirst);
                    try
                    {
                        forSwapSecond = game.GetImageOnCoords(endPlaceType, (int)(endCoords.X - 1), (int)(endCoords.Y - 1));
                        canvas.Children.Remove(forSwapSecond);
                    }
                    catch (NullReferenceException)
                    {
                    }
                    
                    game.MoveJoker(startPlaceType, (int)(startCoords.X - 1), (int)(startCoords.Y - 1), endPlaceType, (int)(endCoords.X - 1), (int)(endCoords.Y - 1));

                    if (game.GetBoardField(startPlaceType, (int)(startCoords.X - 1), (int)(startCoords.Y - 1)) == null)
                    {
                        //do nothing
                    }
                    else if (game.GetJokerOnCoords(startPlaceType, (int)(startCoords.X - 1), (int)(startCoords.Y - 1)))
                    {
                        forSwapFirst = controlPanel.GetJoker();
                        forSwapFirst.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(ManipulationStarted);
                        forSwapFirst.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(ManipulationDelta);
                        forSwapFirst.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(ManipulationCompleted);
                        game.SetImageOnCoords(startPlaceType, (int)(startCoords.X - 1), (int)(startCoords.Y - 1), forSwapFirst);
                        canvas.Children.Add(forSwapFirst);
                        Canvas.SetLeft(forSwapFirst, this.startPoint.X);
                        Canvas.SetTop(forSwapFirst, this.startPoint.Y);
                    }
                    else
                    {
                        Id id = game.GetBoardField(startPlaceType, (int)(startCoords.X - 1), (int)(startCoords.Y - 1));
                        forSwapFirst = controlPanel.GetImageByColorAndId(id.Figure, id.Color);
                        forSwapFirst.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(ManipulationStarted);
                        forSwapFirst.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(ManipulationDelta);
                        forSwapFirst.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(ManipulationCompleted);
                        game.SetImageOnCoords(startPlaceType, (int)(startCoords.X - 1), (int)(startCoords.Y - 1), forSwapFirst);
                        canvas.Children.Add(forSwapFirst);
                        Canvas.SetLeft(forSwapFirst, this.startPoint.X);
                        Canvas.SetTop(forSwapFirst, this.startPoint.Y);
                    }

                    if (game.GetJokerOnCoords(endPlaceType, (int)(endCoords.X - 1), (int)(endCoords.Y - 1)))
                    {
                        Debug.WriteLine("czy na penwo?");
                        forSwapSecond = controlPanel.GetJoker();
                        forSwapSecond.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(ManipulationStarted);
                        forSwapSecond.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(ManipulationDelta);
                        forSwapSecond.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(ManipulationCompleted);
                        game.SetImageOnCoords(endPlaceType, (int)(endCoords.X - 1), (int)(endCoords.Y - 1), forSwapSecond);
                        canvas.Children.Add(forSwapSecond);
                        Canvas.SetLeft(forSwapSecond, Canvas.GetLeft(this.opacityRect) + 3);
                        Canvas.SetTop(forSwapSecond, Canvas.GetTop(this.opacityRect) + 3);
                    }
                    else
                    {
                        Id id = game.GetBoardField(endPlaceType, (int)(endCoords.X - 1), (int)(endCoords.Y - 1));
                        forSwapSecond = controlPanel.GetImageByColorAndId(id.Figure, id.Color);
                        forSwapSecond.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(ManipulationStarted);
                        forSwapSecond.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(ManipulationDelta);
                        forSwapSecond.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(ManipulationCompleted);
                        game.SetImageOnCoords(endPlaceType, (int)(endCoords.X - 1), (int)(endCoords.Y - 1), forSwapSecond);
                        canvas.Children.Add(forSwapSecond);
                        Canvas.SetLeft(forSwapSecond, Canvas.GetLeft(this.opacityRect) + 3);
                        Canvas.SetTop(forSwapSecond, Canvas.GetTop(this.opacityRect) + 3);
                    }
                    canvas.Children.Remove(this.opacityRect);
                    this.opacityRect = null;
                }
                else
                {
                    try
                    {
                        this.game.MoveCards(this.startPlaceType, (int)this.startCoords.X - 1, (int)this.startCoords.Y - 1, this.endPlaceType, (int)this.endCoords.X - 1, (int)this.endCoords.Y - 1);
                        this.game.SetImageOnCoords(this.endPlaceType, (int)(this.endCoords.X - 1), (int)(this.endCoords.Y - 1), image);
                    }
                    catch (AlreadyTakenException)
                    {
                        Debug.WriteLine("AlreadyTaken");
                        Canvas.SetLeft(image, this.startPoint.X);
                        Canvas.SetTop(image, this.startPoint.Y);
                        canvas.Children.Remove(this.opacityRect);
                        this.opacityRect = null;
                        return;
                    }
                    catch (NullReferenceException)
                    {
                        Debug.WriteLine("Nullreference");
                        Canvas.SetLeft(image, this.startPoint.X);
                        Canvas.SetTop(image, this.startPoint.Y);
                        canvas.Children.Remove(this.opacityRect);
                        this.opacityRect = null;
                        return;
                    }
                    //odejmowanie 
                    Canvas.SetLeft(image, Canvas.GetLeft(this.opacityRect) + 3);
                    Canvas.SetTop(image, Canvas.GetTop(this.opacityRect) + 3);
                    canvas.Children.Remove(this.opacityRect);
                    this.opacityRect = null;
                }
            }
            else
            {
                Canvas.SetLeft(image, this.startPoint.X);
                Canvas.SetTop(image, this.startPoint.Y);
            }
        }

        private void NextAndAccept(object sender, EventArgs e)
        {

            for (int i = 0; i < 4; i++)             // zablokowanie wszystkich kafelków po położeniu i wciśnięciu przycisku
            {
                for (int j = 0; j < 4; j++)
                {
                    if (game.GetBoardField(PlaceType.Grid, i, j) != null)
                    {
                        Debug.WriteLine("i = " + i + " j = " + j + " " + game.GetBoardField(PlaceType.Grid, i, j).Blocked);
                    }

                }
            }

            Boolean pop = false;
            pop = this.game.IsRandBoardClear();

            this.checker.GameBoard = this.game.GetGameBoard();
            List<List<Id>> collection = this.checker.GetCollection();
            List<CheckerType> infoCollection = this.checker.GetCollectionInfo();
            List<int> Index = new List<int>();

            calculate.ListOfCards = collection;

            calculate.GetActResult();

            if (calculate.GlobalResult > 300)
            {
                AddJoker();
            }

            for (int i = 0; i < collection.Count; i++)
            {
                Debug.WriteLine("coll");
                Debug.WriteLine(collection[i][0].Figure + " " + collection[i][0].Color + " " + collection[i][1].Figure + " " + collection[i][1].Color + " " + collection[i][2].Figure + " " + collection[i][2].Color + " " + collection[i][3].Figure + " " + collection[i][3].Color);
                if (calculate.CalculateFourResult(collection[i]) >= 100)
                {
                    Index.Add(i);
                }
            }

            for (int i = 0; i < Index.Count; i++)
            {
                ClearFour(infoCollection[Index[i]].x, infoCollection[Index[i]].y, infoCollection[Index[i]].fourcardtype);  
            }

            for (int i = 0; i < 4; i++)             // zablokowanie wszystkich kafelków po położeniu i wciśnięciu przycisku
            {
                for (int j = 0; j < 4; j++)
                {
                    // TODO: zmiana na i i j
                    if (game.IsFieldFree(i+1, j+1, PlaceType.Grid) == false)         // jeśli na danym polu leży kafelek
                    {
                        game.BlockField(i, j);
                        Image imageTemp = this.game.GetImageOnCoords(i, j);
                        imageTemp.ManipulationStarted -= ManipulationStarted;
                        imageTemp.ManipulationDelta -= ManipulationDelta;
                        imageTemp.ManipulationCompleted -= ManipulationCompleted;
                    }

                }
            }

            if (pop == true)                // sprawdzenie czy cała plansza jest zajęta (wg mnie niepotrzebnie)
            {
                CardImage[] cardImage = this.game.RandNewCards();
                Point point = new Point();

                for (int i = 0; i < cardImage.Count(); i++)
                {
                    if (cardImage[i] == null)
                    {
                        break;
                    }

                    // dodanie eventów do nowych kafelków na belce rand
                    cardImage[i].image.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(ManipulationStarted);
                    cardImage[i].image.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(ManipulationDelta);
                    cardImage[i].image.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(ManipulationCompleted);

                    // wyświetlenie kafelków na belce
                    this.canvas.Children.Add(cardImage[i].image);
                    point = controlPanel.GetRandCoordsForMarkRectangle(i + 1, 1);
                    Canvas.SetLeft(cardImage[i].image, point.X + 3);
                    Canvas.SetTop(cardImage[i].image, point.Y + 3);
                }
            }

            Int64 tempResult = Convert.ToInt64(this.textblock.Text);
            tempResult = calculate.GlobalResult;
            this.textblock.Text = Convert.ToString(tempResult);

            if (this.game.HowMuchFreeSpaceOnGameBoard() == 0 && game.NoJokerOnBoard())
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }

        }

        private void BackToPanorama(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ClearFour(int x, int y, FourcardType fourcardType)
        {
            // cross

            if (fourcardType == FourcardType.Cross)
            {
                if (x == 3)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        try
                        {
                            Image image = this.game.DeleteImageOnCoords(3 - i, i);
                            canvas.Children.Remove(image);
                        }
                        catch (NullReferenceException)
                        {
                        }
                    }
                } else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        try
                        {
                            Image image = this.game.DeleteImageOnCoords(i, i);
                            canvas.Children.Remove(image);
                        }
                        catch (NullReferenceException)
                        {
                        }
                    }
                }

            }

            // column

            else if (fourcardType == FourcardType.Column)
            {
                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                        Image image = this.game.DeleteImageOnCoords(x, i);
                        canvas.Children.Remove(image);
                    }
                    catch (NullReferenceException)
                    {
                    }
                }
            }

            // row

            else if (fourcardType == FourcardType.Row)
            {
                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                        Image image = this.game.DeleteImageOnCoords(i, y);
                        canvas.Children.Remove(image);
                    }
                    catch (NullReferenceException)
                    {
                    }
                }
            }

            // rectangle

            else if (fourcardType == FourcardType.Rectangle)
            {
                for (int i = x; i < x+2; i++)
                {
                    for (int j = y; j < y + 2; j++)
                    {
                        try
                        {
                            Image image = this.game.DeleteImageOnCoords(i, j);
                            canvas.Children.Remove(image);
                        }
                        catch (NullReferenceException)
                        {

                        }
                    }
                }
            }
        }

    }
}