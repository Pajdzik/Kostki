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
            this.game = new Game(this.controlPanel);
            InitializeComponent();
            this.ShowRectangle();
            this.CreateTextBlockForResult();
        }

        /// <summary>
        /// Metoda dodająca manipulation eventy 
        /// </summary>
        /// <param name="element"> Element, do którego mają zostać 
        /// dodane te eventy</param>
        public void ManipulationSettings(UIElement element)
        {
            element.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(ManipulationStarted);
            element.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(ManipulationDelta);
            element.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(ManipulationCompleted);
        }

        /// <summary>
        /// Metoda odpowiedzialna za ustawianie obiektów na planszy.
        /// </summary>
        /// <param name="element"> Referencja na obiekt</param>
        /// <param name="point"> Punkt, w którym ma zostać wyświetlony</param>
        public void SettingCanvasOrigin(UIElement element, Point point)
        {
            Canvas.SetLeft(element, point.X);
            Canvas.SetTop(element, point.Y);
        }

        /// <summary>
        ///  Metoda odpowiedzialna za ustawienia obiektów na planszy
        ///  z przesunięciem 3 pikseli, aby było ładnie :)
        /// </summary>
        /// <param name="element"> Referencja na obiekt</param>
        /// <param name="point"> Punkt, w którym ma zostać wyświetlony</param>
        public void SettingCanvasTranslate(UIElement element, Point point)
        {
            Canvas.SetLeft(element, point.X + 3);
            Canvas.SetTop(element, point.Y + 3);
        }

        /// <summary>
        /// Stworzenie Blocku, w którym będzie pokazywany wynik.
        /// </summary>
        public void CreateTextBlockForResult()
        {
            textblock = new TextBlock();
            textblock.Text = "0";
            textblock.FontSize = 40;
            canvas.Children.Add(textblock);
            SettingCanvasOrigin(textblock, new Point((double)(this.controlPanel.GetTopJoker().X + 220),
                                                    (double)(this.controlPanel.GetTopJoker().Y + 15)));
        }

        /// <summary>
        /// Główna metoda generująca wygląd aplikacji
        /// </summary>
        public void ShowRectangle()
        {
            this.ShowRectangleForJoker();
            this.ShowRectangleForGrid();
            this.ShowRectangleForRand();
        }

        /// <summary>
        /// Metoda pokazująca kafelki dla Jokerów.
        /// </summary>
        public void ShowRectangleForJoker()
        {
            currentPosition = startPosition = this.controlPanel.GetTopJoker();
            for (int j = 0; j < 2; j++)
            {
                Rectangle rect = this.controlPanel.GetRectangle();
                this.canvas.Children.Add(rect);
                this.SettingCanvasOrigin(rect, currentPosition);
                currentPosition.X += 100;
            }
        }

        /// <summary>
        /// Metoda pokazująca kafelki dla Grida
        /// </summary>
        public void ShowRectangleForGrid()
        {
            currentPosition = startPosition = this.controlPanel.GetTopGrid();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Rectangle rect = this.controlPanel.GetRectangle();
                    this.canvas.Children.Add(rect);
                    this.SettingCanvasOrigin(rect, currentPosition);
                    currentPosition.X += 100;
                }
                currentPosition.X = startPosition.X;
                currentPosition.Y += 100;
            }
        }

        /// <summary>
        /// Metoda pokazująca kafelki dla Randa 
        /// </summary>
        public void ShowRectangleForRand()
        {
            currentPosition = startPosition = this.controlPanel.GetTopRand();
            for (int j = 0; j < 4; j++)
            {
                Rectangle rect = this.controlPanel.GetRectangle();
                this.canvas.Children.Add(rect);
                this.SettingCanvasOrigin(rect, currentPosition);
                currentPosition.X += 100;
            }
        }

        /// <summary>
        /// Programowalne dodawania Jokerów (Bonus dla gracza)
        /// </summary>
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
                SettingCanvasTranslate(jokers[i], jokerPoint);
                ManipulationSettings(jokers[i]);
                game.AddJoker(jokers[i], i);
            }
        }

        /// <summary>
        /// Metoda przeciązająca akcje po wciśnięciu na dany UIelement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Sender object</param>
        private void ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            Image image = (Image)sender;
            this.startPoint = new Point(Canvas.GetLeft(image), Canvas.GetTop(image));

            SettingCanvasOrigin(image, new Point((double)((int)(Canvas.GetLeft(image) - 0.15 * image.Width)),
                                                (double)((int)(Canvas.GetTop(image) - 0.15 * image.Width))));

            //Canvas.SetLeft(image, (double)((int)(Canvas.GetLeft(image) - 0.15 * image.Width)));
            //Canvas.SetTop(image, (double)((int)(Canvas.GetTop(image) - 0.15 * image.Width)));

            //testowy fragment kodu
            PlaceType place = this.controlPanel.RecognizePlace(new Point(Canvas.GetLeft(image) +
                (controlPanel.CardSize * 1.3) / 2, Canvas.GetTop(image) + (controlPanel.CardSize * 1.3) / 2));
            this.startCoords = this.controlPanel.GetCoordsFromActualPoint(new Point(Canvas.GetLeft(image) +
                (controlPanel.CardSize * 1.3) / 2, Canvas.GetTop(image) + (controlPanel.CardSize * 1.3) / 2), place);
            this.startPlaceType = place;

            isItJoker = game.GetJokerOnCoords(place, (int)(startCoords.X - 1), (int)(startCoords.Y - 1));

            Canvas.SetZIndex((UIElement)sender, 1);        // ustawienie z-indeksu trzymanego obrazka na wierzch                          

            image.Opacity = controlPanel.OpacityCoefficient;                    // ustawienie półprzezroczystości
            image.Height = image.Width = controlPanel.CardSize * controlPanel.ResizeCoefficient;       // zwiększenie rozmiaru

        }

        /// <summary>
        /// Metoda wywoływana podczas przeciągania danego UIelementu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Sender object</param>
        private void ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            Image image = (Image)sender;
            SettingCanvasOrigin(image, new Point(Canvas.GetLeft(image) + e.DeltaManipulation.Translation.X,
                                                Canvas.GetTop(image) + e.DeltaManipulation.Translation.Y));
            //Canvas.SetLeft(image, Canvas.GetLeft(image) + e.DeltaManipulation.Translation.X);
            //Canvas.SetTop(image, Canvas.GetTop(image) + e.DeltaManipulation.Translation.Y);
            PlaceType place;
            try
            {
                place = this.controlPanel.RecognizePlace(new Point(Canvas.GetLeft(image) + (controlPanel.CardSize * 1.3) / 2, Canvas.GetTop(image) + (controlPanel.CardSize * 1.3) / 2));
                this.endCoords = this.controlPanel.GetCoordsFromActualPoint(new Point(Canvas.GetLeft(image) + (controlPanel.CardSize * 1.3) / 2, Canvas.GetTop(image) + (controlPanel.CardSize * 1.3) / 2), place);
                this.endPlaceType = place;
            }
            catch (OutOfBoardException)
            {
                canvas.Children.Remove(this.opacityRect);
                this.opacityRect = null;
                return;
            }

            try
            {
                canvas.Children.Remove(this.opacityRect);
                Point point = controlPanel.GetViewportPointFromActualPoint(new Point(Canvas.GetLeft(image) + (controlPanel.CardSize * 1.3) / 2, Canvas.GetTop(image) + (controlPanel.CardSize * 1.3) / 2));
                if (this.game.IsFieldFree((int)endCoords.X, (int)endCoords.Y, this.endPlaceType) == true || isItJoker == true)            // wyłączenie podświetlenia kafelka gdy jest zajęty
                {
                    this.opacityRect = controlPanel.GetMarkRectangle();
                    canvas.Children.Add(opacityRect);
                    SettingCanvasOrigin(opacityRect, point);
                }
                else if (endPlaceType == PlaceType.Joker && isItJoker == true)
                {
                    opacityRect = controlPanel.GetMarkRectangle();
                    canvas.Children.Add(opacityRect);
                    Point jokerPoint = controlPanel.GetJokerViewportPointFromCoords((int)(endCoords.X - 1), (int)(endCoords.Y));
                    SettingCanvasTranslate(opacityRect, jokerPoint);
                }
            }
            catch (NullReferenceException)
            {
                this.opacityRect = null;
            }
        }

        /// <summary>
        /// Metoda wywoływana, kiedy akcja na UIelemencie została zakończona.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Sender object</param>
        private void ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Image image = (Image)sender;

            SettingCanvasOrigin(image, new Point((double)((int)(Canvas.GetLeft(image) + 0.15 * image.Width)),
                                                (double)((int)(Canvas.GetTop(image) + 0.15 * image.Width))));

            //Canvas.SetLeft(image, (double)((int)(Canvas.GetLeft(image) + 0.15 * image.Width)));
            //Canvas.SetTop(image, (double)((int)(Canvas.GetTop(image) + 0.15 * image.Width)));

            image.Opacity = 1.0;                    // ustawienie pełnej widoczności z powrotem
            image.Height = image.Width = controlPanel.CardSize;     // ustawienie rozmiarów z powrotem
            Canvas.SetZIndex((UIElement)sender, 0);         // przesunięcie kafelka wartstwę niżej

            if (this.opacityRect != null)
            {
                if (isItJoker == true)
                {
                    if ((int)(startCoords.X - 1) == (int)(endCoords.X - 1) &&
                        (int)(startCoords.Y - 1) == (int)(endCoords.Y - 1) &&
                        startPlaceType == endPlaceType)
                    {
                        SettingCanvasOrigin(image, startPoint);
                        canvas.Children.Remove(this.opacityRect);
                        this.opacityRect = null;
                        return;
                    }

                    Image forSwapFirst, forSwapSecond;
                    canvas.Children.Remove(image);

                    try
                    {
                        forSwapSecond = game.GetImageOnCoords(endPlaceType, (int)(endCoords.X - 1), (int)(endCoords.Y - 1));
                        canvas.Children.Remove(forSwapSecond);
                    }
                    catch (NullReferenceException)
                    {
                    }

                    game.MoveJoker(startPlaceType, (int)(startCoords.X - 1), (int)(startCoords.Y - 1), endPlaceType, (int)(endCoords.X - 1), (int)(endCoords.Y - 1));

                    if (game.GetBoardField(startPlaceType, (int)(startCoords.X - 1), (int)(startCoords.Y - 1)) != null)
                    {
                        if (game.GetJokerOnCoords(startPlaceType, (int)(startCoords.X - 1), (int)(startCoords.Y - 1)))
                        {
                            forSwapFirst = controlPanel.GetJoker();
                            ManipulationSettings(forSwapFirst);
                            game.SetImageOnCoords(startPlaceType, (int)(startCoords.X - 1), (int)(startCoords.Y - 1), forSwapFirst);
                            canvas.Children.Add(forSwapFirst);
                            SettingCanvasOrigin(forSwapFirst, startPoint);
                        }
                        else
                        {
                            Id id = game.GetBoardField(startPlaceType, (int)(startCoords.X - 1), (int)(startCoords.Y - 1));
                            forSwapFirst = controlPanel.GetImageByColorAndId(id.Figure, id.Color);
                            ManipulationSettings(forSwapFirst);
                            game.SetImageOnCoords(startPlaceType, (int)(startCoords.X - 1), (int)(startCoords.Y - 1), forSwapFirst);
                            canvas.Children.Add(forSwapFirst);
                            SettingCanvasOrigin(forSwapFirst, startPoint);
                        }
                    }

                    if (game.GetJokerOnCoords(endPlaceType, (int)(endCoords.X - 1), (int)(endCoords.Y - 1)))
                    {
                        forSwapSecond = controlPanel.GetJoker();
                        ManipulationSettings(forSwapSecond);
                        game.SetImageOnCoords(endPlaceType, (int)(endCoords.X - 1), (int)(endCoords.Y - 1), forSwapSecond);
                        canvas.Children.Add(forSwapSecond);
                        SettingCanvasTranslate(forSwapSecond, new Point(Canvas.GetLeft(this.opacityRect),
                                                                        Canvas.GetTop(this.opacityRect)));
                        //Canvas.SetLeft(forSwapSecond, Canvas.GetLeft(this.opacityRect) + 3);
                        //Canvas.SetTop(forSwapSecond, Canvas.GetTop(this.opacityRect) + 3);
                    }
                    else
                    {
                        Id id = game.GetBoardField(endPlaceType, (int)(endCoords.X - 1), (int)(endCoords.Y - 1));
                        forSwapSecond = controlPanel.GetImageByColorAndId(id.Figure, id.Color);
                        ManipulationSettings(forSwapSecond);
                        game.SetImageOnCoords(endPlaceType, (int)(endCoords.X - 1), (int)(endCoords.Y - 1), forSwapSecond);
                        canvas.Children.Add(forSwapSecond);
                        SettingCanvasTranslate(forSwapSecond, new Point(Canvas.GetLeft(this.opacityRect),
                                                                        Canvas.GetTop(this.opacityRect)));
                        //Canvas.SetLeft(forSwapSecond, Canvas.GetLeft(this.opacityRect) + 3);
                        //Canvas.SetTop(forSwapSecond, Canvas.GetTop(this.opacityRect) + 3);
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
                        SettingCanvasOrigin(image, startPoint);
                        canvas.Children.Remove(this.opacityRect);
                        this.opacityRect = null;
                        return;
                    }
                    catch (NullReferenceException)
                    {
                        SettingCanvasOrigin(image, startPoint);
                        canvas.Children.Remove(this.opacityRect);
                        this.opacityRect = null;
                        return;
                    }
                    //odejmowanie 
                    SettingCanvasTranslate(image, new Point(Canvas.GetLeft(this.opacityRect),
                                                            Canvas.GetTop(this.opacityRect)));
                    canvas.Children.Remove(this.opacityRect);
                    this.opacityRect = null;
                }
            }
            else
            {
                SettingCanvasOrigin(image, startPoint);
            }
        }

        /// <summary>
        /// Metoda wywoływana po wciśnięciu przez użytkownika
        /// przycisku akceptacji, który mówi o tym, że:
        ///     * przechodzimy do kolejnej partii
        ///     * blokujemy wszystkie karty
        ///     * dodajemy punktu
        ///     * randomujemy nowe karty, kiedy jest potrzeba 
        ///         (kiedy nie ma już żadnej karty w randzie
        ///         i są puste miejsca w gridzie)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Sender object</param>
        private void NextAndAccept(object sender, EventArgs e)
        {
            Boolean pop = false;
            pop = this.game.IsRandBoardClear();

            this.checker.GameBoard = this.game.GetGameBoard();
            List<List<Id>> collection = this.checker.GetCollection();
            List<CheckerType> infoCollection = this.checker.GetCollectionInfo();
            List<int> Index = new List<int>();

            this.calculate.ListOfCards = collection;

            this.calculate.GetActResult();

            // this.AddJoker();

            for (int i = 0; i < collection.Count; i++)
            {
                if (this.calculate.CalculateFourResult(collection[i]) >= 100)
                {
                    Index.Add(i);
                }
            }

            for (int i = 0; i < Index.Count; i++)
            {
                this.ClearFour(infoCollection[Index[i]].x, infoCollection[Index[i]].y, infoCollection[Index[i]].fourcardtype);
            }

            for (int i = 0; i < 4; i++)             // zablokowanie wszystkich kafelków po położeniu i wciśnięciu przycisku
            {
                for (int j = 0; j < 4; j++)
                {
                    // TODO: zmiana na i i j
                    if (this.game.IsFieldFree(i + 1, j + 1, PlaceType.Grid) == false)         // jeśli na danym polu leży kafelek
                    {
                        this.game.BlockField(i, j);
                        Image imageTemp = this.game.GetImageOnCoords(i, j);
                        imageTemp.ManipulationStarted -= ManipulationStarted;
                        imageTemp.ManipulationDelta -= ManipulationDelta;
                        imageTemp.ManipulationCompleted -= ManipulationCompleted;
                    }
                    if (game.GetBoardField(PlaceType.Grid, i, j) != null)
                    {
                        if (game.GetBoardField(PlaceType.Grid, i, j).IsJoker == true ||
                            game.GetBoardField(PlaceType.Grid, i, j).Figure == Figures.Joker)
                        {
                            game.GetBoardField(PlaceType.Grid, i, j).IsJoker = true;
                        }
                        else
                        {
                            game.GetBoardField(PlaceType.Grid, i, j).IsJoker = false;
                        }
                    }
                }
            }
            Debug.WriteLine(pop);

            if (pop == true)                // sprawdzenie czy cała plansza jest zajęta (wg mnie niepotrzebnie)
            // Moim też, niedługo to wyjebiemy :)
            {
                CardImage[] cardImage = this.game.RandNewCards();
                Point point = new Point();

                for (int i = 0; i < cardImage.Count(); i++)
                {
                    if (cardImage[i] == null)
                    {
                        break;
                    }
                    ManipulationSettings(cardImage[i].image);
                    this.canvas.Children.Add(cardImage[i].image);
                    point = controlPanel.GetRandCoordsForMarkRectangle(i + 1, 1);
                    SettingCanvasTranslate(cardImage[i].image, point);
                }
            }

            Int64 tempResult = Convert.ToInt64(this.textblock.Text);
            tempResult = this.calculate.GlobalResult;
            this.textblock.Text = Convert.ToString(tempResult);

            if (this.game.HowMuchFreeSpaceOnGameBoard() == 0 && game.NoJokerOnBoard())
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }

        /// <summary>
        /// Metoda wywoływana podczas powrotu do widoku Panoramy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"> Sender object</param>
        private void BackToPanorama(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            AppMemory appMemory = new AppMemory();
            appMemory.SaveGameState(game.GetGameBoard());
        }

        /// <summary>
        /// Metoda wywoływana podczas przyciśnięcia przycisku wstecz.
        /// </summary>
        /// <param name="e"> e </param>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            AppMemory appMemory = new AppMemory();
            appMemory.SaveGameState(game.GetGameBoard());

            base.OnBackKeyPress(e);
        }

        /// <summary>
        /// Metoda wywoływana w momencie ładowania strony (zaraz po konstruktorze)
        /// Tutaj ładujemy wszystkie dane z pamięci i wyświetlamy stary stan planszy.
        /// </summary>
        /// <param name="sender">Obiekt, który wywołuje metodę.</param>
        /// <param name="e"> Paramery routingu.</param>
        private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
        {
            AppMemory appMemory = new AppMemory();
            Id[,,] memBoard = appMemory.LoadGameState();
            Image imageToLoad;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (memBoard[k, i, j] != null)
                        {
                            if (k == (int)PlaceType.Grid)
                            {
                                imageToLoad = this.LoadCardsFromMemory(memBoard[k, i, j], i, j);
                                memBoard[k, i, j].Image = imageToLoad;
                            }
                            else if (k == (int)PlaceType.Joker)
                            {
                                LoadJokerFromMemory(i, j);
                            }
                            else if (k == (int)PlaceType.Rand)
                            {
                                imageToLoad = this.LoadRandFromMemory(memBoard[k, i, j], i, j);
                                memBoard[k, i, j].Image = imageToLoad;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            game.SetGameBoard(memBoard);
        }

        /// <summary>
        /// Ładowanie kart z randomu z pamięci
        /// </summary>
        /// <param name="card"> Karta, którą należy załadować</param>
        /// <param name="x"> Pozycja x tej karty</param>
        /// <param name="y"> Pozycja y tej karty</param>
        /// <returns> Image that's load from Memory</returns>
        private Image LoadRandFromMemory(Id card, int x, int y)
        {
            if (card == null)
                return null;
            Image image = controlPanel.GetImageByColorAndId(card.Figure, card.Color);
            Point point = controlPanel.GetRandCoordsForMarkRectangle(x + 1, 1);
            this.ManipulationSettings(image);
            this.canvas.Children.Add(image);
            this.SettingCanvasTranslate(image, point);

            return image;

        }

        /// <summary>
        /// Ładowanie kart z grida z panmięci
        /// </summary>
        /// <param name="card"> Karta, którą nalezy załadować</param>
        /// <param name="x"> Pozycja x karty</param>
        /// <param name="y"> Pozycja y karty</param>
        /// <returns> Image that's load from Memory</returns>
        private Image LoadCardsFromMemory(Id card, int x, int y)
        {
            Image image;
            if (card == null)
                return null;

            if (card.Figure != Figures.Joker)
            {
                image = controlPanel.GetImageByColorAndId(card.Figure, card.Color);
            }
            else 
            {
                Debug.WriteLine("Catch");
                image = controlPanel.GetJoker();
            }
            Point point = controlPanel.GetGridCoordsForMarkRectangle(x + 1, y + 1);

            if (card.Blocked == false)
            {
                this.ManipulationSettings(image);
            }
            this.canvas.Children.Add(image);
            this.SettingCanvasTranslate(image, point);

            return image;
        }


        /// <summary>
        /// Ładowanie Jokerów z pamięci
        /// </summary>
        /// <param name="x"> Pozycja x Jokera</param>
        /// <param name="y"> Pozycja y Jokera</param>
        private void LoadJokerFromMemory(int x, int y)
        {
            jokers = new Image[2];
            Image joker = controlPanel.GetJoker();
            jokers[x] = joker;
            Point jokerPoint = controlPanel.GetJokerCoordsForMarkRectangle(x + 1, 1);
            this.ManipulationSettings(joker);
            canvas.Children.Add(joker);
            this.SettingCanvasTranslate(joker, jokerPoint);
            this.game.AddJoker(joker, x);
        }

        /// <summary>
        ///  Czyszczenie planszy po czyszczącej kombinacji
        /// </summary>
        /// <param name="x"> Pozycja x, od której zaczyna się kombinacja</param>
        /// <param name="y"> Pozycja y, od której zaczyna się kombinacja</param>
        /// <param name="fourcardType"> Typ kombinacji do wyczyszczenia</param>
        private void ClearFour(int x, int y, FourcardType fourcardType)
        {
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
                        catch (NullReferenceException) { }
                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        try
                        {
                            Image image = this.game.DeleteImageOnCoords(i, i);
                            canvas.Children.Remove(image);
                        }
                        catch (NullReferenceException) { }
                    }
                }
            }
            else if (fourcardType == FourcardType.Column)
            {
                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                        Image image = this.game.DeleteImageOnCoords(x, i);
                        canvas.Children.Remove(image);
                    }
                    catch (NullReferenceException) { }
                }
            }
            else if (fourcardType == FourcardType.Row)
            {
                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                        Image image = this.game.DeleteImageOnCoords(i, y);
                        canvas.Children.Remove(image);
                    }
                    catch (NullReferenceException) { }
                }
            }
            else if (fourcardType == FourcardType.Rectangle)
            {
                for (int i = x; i < x + 2; i++)
                {
                    for (int j = y; j < y + 2; j++)
                    {
                        try
                        {
                            Image image = this.game.DeleteImageOnCoords(i, j);
                            canvas.Children.Remove(image);
                        }
                        catch (NullReferenceException) { }
                    }
                }
            }
        }
    }
}