using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Kostki.Class;
using Kostki.Exceptions;

namespace Kostki
{
    public partial class GamePage : PhoneApplicationPage
    {
        private ControlPanel controlPanel;
        private Point _currentPosition;
        private Point _startPosition;
        private Point _startPoint; // punkt startowy karty, przed wciśnięciem
        private Point _startCoords;
        private Point _endCoords;
        private PlaceType _startPlaceType;
        private PlaceType _endPlaceType;
        private Rectangle _opacityRect;
        private Game _game;
        private Checker checker;
        private Calculate _calculate;
        private TextBlock _textblock;
        private Image[] _jokers;
        private Boolean _isItJoker;

        public GamePage()
        {
            this._calculate = new Calculate();
            this.checker = new Checker();
            this.controlPanel = new ControlPanel();
            this._game = new Game(this.controlPanel);
            InitializeComponent();
            this.ShowRectangle();
            this.CreateTextBlockForResult();
        }

        /// <summary>
        /// Metoda dodająca manipulation eventy 
        /// </summary>
        /// <param name="element"> Element, do którego mają zostać dodane te eventy</param>
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
            _textblock = new TextBlock();
            _textblock.Text = "0";
            _textblock.FontSize = 40;
            canvas.Children.Add(_textblock);
            SettingCanvasOrigin(_textblock, new Point((double)(this.controlPanel.GetTopJoker().X + 220),
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
            _currentPosition = _startPosition = this.controlPanel.GetTopJoker();
            for (int j = 0; j < 2; j++)
            {
                Rectangle rect = this.controlPanel.GetRectangle();
                this.canvas.Children.Add(rect);
                this.SettingCanvasOrigin(rect, _currentPosition);
                _currentPosition.X += 100;
            }
        }

        /// <summary>
        /// Metoda pokazująca kafelki dla Grida
        /// </summary>
        public void ShowRectangleForGrid()
        {
            _currentPosition = _startPosition = this.controlPanel.GetTopGrid();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Rectangle rect = this.controlPanel.GetRectangle();
                    this.canvas.Children.Add(rect);
                    this.SettingCanvasOrigin(rect, _currentPosition);
                    _currentPosition.X += 100;
                }
                _currentPosition.X = _startPosition.X;
                _currentPosition.Y += 100;
            }
        }

        /// <summary>
        /// Metoda pokazująca kafelki dla Randa 
        /// </summary>
        public void ShowRectangleForRand()
        {
            _currentPosition = _startPosition = this.controlPanel.GetTopRand();
            for (int j = 0; j < 4; j++)
            {
                Rectangle rect = this.controlPanel.GetRectangle();
                this.canvas.Children.Add(rect);
                this.SettingCanvasOrigin(rect, _currentPosition);
                _currentPosition.X += 100;
            }
        }

        /// <summary>
        /// Programowalne dodawania Jokerów (Bonus dla gracza)
        /// </summary>
        public void AddJoker()
        {
            if (this._jokers != null)
            {
                this.canvas.Children.Remove(_jokers[0]);
                this.canvas.Children.Remove(_jokers[1]);
            }
            this._jokers = new Image[2];
            Point jokerPoint = new Point();

            for (int i = 0; i < 2; i++)
            {
                this._jokers[i] = this.controlPanel.GetJoker();
                this.canvas.Children.Add(_jokers[i]);
                jokerPoint = this.controlPanel.GetJokerCoordsForMarkRectangle(i + 1, 1);
                this.SettingCanvasTranslate(_jokers[i], jokerPoint);
                this.ManipulationSettings(_jokers[i]);
                this._game.AddJoker(_jokers[i], i);
            }
        }

        /// <summary>
        /// Metoda przeciązająca akcje po wciśnięciu na dany UIelement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Sender object</param>
        private new void ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            Image image = (Image)sender;
            this._startPoint = new Point(Canvas.GetLeft(image), Canvas.GetTop(image));

            SettingCanvasOrigin(image, new Point((double)((int)(Canvas.GetLeft(image) - 0.15 * image.Width)),
                                                (double)((int)(Canvas.GetTop(image) - 0.15 * image.Width))));

            //Canvas.SetLeft(image, (double)((int)(Canvas.GetLeft(image) - 0.15 * image.Width)));
            //Canvas.SetTop(image, (double)((int)(Canvas.GetTop(image) - 0.15 * image.Width)));

            //testowy fragment kodu
            PlaceType place = this.controlPanel.RecognizePlace(new Point(Canvas.GetLeft(image) +
                (this.controlPanel.CardSize * 1.3) / 2, Canvas.GetTop(image) + (this.controlPanel.CardSize * 1.3) / 2));
            this._startCoords = this.controlPanel.GetCoordsFromActualPoint(new Point(Canvas.GetLeft(image) +
                (this.controlPanel.CardSize * 1.3) / 2, Canvas.GetTop(image) + (this.controlPanel.CardSize * 1.3) / 2), place);
            this._startPlaceType = place;

            _isItJoker = _game.GetJokerOnCoords(place, (int)(_startCoords.X - 1), (int)(_startCoords.Y - 1));

            Canvas.SetZIndex((UIElement)sender, 1);        // ustawienie z-indeksu trzymanego obrazka na wierzch                          

            image.Opacity = this.controlPanel.OpacityCoefficient;                    // ustawienie półprzezroczystości
            image.Height = image.Width = this.controlPanel.CardSize * this.controlPanel.ResizeCoefficient;       // zwiększenie rozmiaru

        }

        /// <summary>
        /// Metoda wywoływana podczas przeciągania danego UIelementu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Sender object</param>
        private new void ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            Image image = (Image)sender;
            SettingCanvasOrigin(image, new Point(Canvas.GetLeft(image) + e.DeltaManipulation.Translation.X,
                                                Canvas.GetTop(image) + e.DeltaManipulation.Translation.Y));
            PlaceType place;
            try
            {
                place = this.controlPanel.RecognizePlace(new Point(Canvas.GetLeft(image) + (this.controlPanel.CardSize * 1.3) / 2, Canvas.GetTop(image) + (this.controlPanel.CardSize * 1.3) / 2));
                this._endCoords = this.controlPanel.GetCoordsFromActualPoint(new Point(Canvas.GetLeft(image) + (this.controlPanel.CardSize * 1.3) / 2, Canvas.GetTop(image) + (this.controlPanel.CardSize * 1.3) / 2), place);
                this._endPlaceType = place;
            }
            catch (OutOfBoardException)
            {
                canvas.Children.Remove(this._opacityRect);
                this._opacityRect = null;
                return;
            }

            try
            {
                canvas.Children.Remove(this._opacityRect);
                Point point = controlPanel.GetViewportPointFromActualPoint(new Point(Canvas.GetLeft(image) + (controlPanel.CardSize * 1.3) / 2, Canvas.GetTop(image) + (controlPanel.CardSize * 1.3) / 2));
                if (_isItJoker == true && this._game.GetBoardField(this._endPlaceType, (int)(_endCoords.X - 1), (int)(_endCoords.Y - 1)) != null && 
                    this._game.GetBoardField(this._endPlaceType, (int)(_endCoords.X - 1), (int)(_endCoords.Y - 1)).IsJoker == true)
                {
                    this._opacityRect = null;
                    return;
                }
                else if (this._game.IsFieldFree((int)_endCoords.X, (int)_endCoords.Y, this._endPlaceType) == true || _isItJoker == true)            // wyłączenie podświetlenia kafelka gdy jest zajęty
                {
                    this._opacityRect = controlPanel.GetMarkRectangle();
                    canvas.Children.Add(_opacityRect);
                    SettingCanvasOrigin(_opacityRect, point);
                }
                else if (_endPlaceType == PlaceType.Joker && _isItJoker == true)
                {
                    _opacityRect = controlPanel.GetMarkRectangle();
                    canvas.Children.Add(_opacityRect);
                    Point jokerPoint = controlPanel.GetJokerViewportPointFromCoords((int)(_endCoords.X - 1), (int)(_endCoords.Y));
                    SettingCanvasTranslate(_opacityRect, jokerPoint);
                }
            }
            catch (NullReferenceException)
            {
                this._opacityRect = null;
            }
        }

        /// <summary>
        /// Metoda wywoływana, kiedy akcja na UIelemencie została zakończona.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Sender object</param>
        private new void ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Image image = (Image)sender;

            SettingCanvasOrigin(image, new Point((double)((int)(Canvas.GetLeft(image) + 0.15 * image.Width)),
                                                (double)((int)(Canvas.GetTop(image) + 0.15 * image.Width))));

            //Canvas.SetLeft(image, (double)((int)(Canvas.GetLeft(image) + 0.15 * image.Width)));
            //Canvas.SetTop(image, (double)((int)(Canvas.GetTop(image) + 0.15 * image.Width)));

            image.Opacity = 1.0;                    // ustawienie pełnej widoczności z powrotem
            image.Height = image.Width = controlPanel.CardSize;     // ustawienie rozmiarów z powrotem
            Canvas.SetZIndex((UIElement)sender, 0);         // przesunięcie kafelka wartstwę niżej

            if (this._opacityRect != null)
            {
                if (_isItJoker == true)
                {
                    if ((int)(_startCoords.X - 1) == (int)(_endCoords.X - 1) &&
                        (int)(_startCoords.Y - 1) == (int)(_endCoords.Y - 1) &&
                        _startPlaceType == _endPlaceType)
                    {
                        SettingCanvasOrigin(image, _startPoint);
                        canvas.Children.Remove(this._opacityRect);
                        this._opacityRect = null;
                        return;
                    }

                    Image forSwapFirst, forSwapSecond;
                    canvas.Children.Remove(image);

                    try
                    {
                        forSwapSecond = this._game.GetImageOnCoords(_endPlaceType, (int)(_endCoords.X - 1), (int)(_endCoords.Y - 1));
                        this.canvas.Children.Remove(forSwapSecond);
                    }
                    catch (NullReferenceException)
                    {
                    }

                    this._game.MoveJoker(_startPlaceType, (int)(_startCoords.X - 1), (int)(_startCoords.Y - 1), _endPlaceType, (int)(_endCoords.X - 1), (int)(_endCoords.Y - 1));

                    if (this._game.GetBoardField(_startPlaceType, (int)(_startCoords.X - 1), (int)(_startCoords.Y - 1)) != null)
                    {
                        if (this._game.GetJokerOnCoords(_startPlaceType, (int)(_startCoords.X - 1), (int)(_startCoords.Y - 1)))
                        {
                            forSwapFirst = this.controlPanel.GetJoker();
                            this.ManipulationSettings(forSwapFirst);
                            this._game.SetImageOnCoords(_startPlaceType, (int)(_startCoords.X - 1), (int)(_startCoords.Y - 1), forSwapFirst);
                            this.canvas.Children.Add(forSwapFirst);
                            this.SettingCanvasOrigin(forSwapFirst, _startPoint);
                        }
                        else
                        {
                            Id id = this._game.GetBoardField(_startPlaceType, (int)(_startCoords.X - 1), (int)(_startCoords.Y - 1));
                            forSwapFirst = this.controlPanel.GetImageByColorAndId(id.Figure, id.Color);
                            this.ManipulationSettings(forSwapFirst);
                            this._game.SetImageOnCoords(_startPlaceType, (int)(_startCoords.X - 1), (int)(_startCoords.Y - 1), forSwapFirst);
                            this.canvas.Children.Add(forSwapFirst);
                            this.SettingCanvasOrigin(forSwapFirst, _startPoint);
                        }
                    }

                    if (_game.GetJokerOnCoords(_endPlaceType, (int)(_endCoords.X - 1), (int)(_endCoords.Y - 1)))
                    {
                        forSwapSecond = controlPanel.GetJoker();
                        ManipulationSettings(forSwapSecond);
                        _game.SetImageOnCoords(_endPlaceType, (int)(_endCoords.X - 1), (int)(_endCoords.Y - 1), forSwapSecond);
                        canvas.Children.Add(forSwapSecond);
                        SettingCanvasTranslate(forSwapSecond, new Point(Canvas.GetLeft(this._opacityRect),
                                                                        Canvas.GetTop(this._opacityRect)));
                        //Canvas.SetLeft(forSwapSecond, Canvas.GetLeft(this.opacityRect) + 3);
                        //Canvas.SetTop(forSwapSecond, Canvas.GetTop(this.opacityRect) + 3);
                    }
                    else
                    {
                        Id id = _game.GetBoardField(_endPlaceType, (int)(_endCoords.X - 1), (int)(_endCoords.Y - 1));
                        forSwapSecond = controlPanel.GetImageByColorAndId(id.Figure, id.Color);
                        ManipulationSettings(forSwapSecond);
                        _game.SetImageOnCoords(_endPlaceType, (int)(_endCoords.X - 1), (int)(_endCoords.Y - 1), forSwapSecond);
                        canvas.Children.Add(forSwapSecond);
                        SettingCanvasTranslate(forSwapSecond, new Point(Canvas.GetLeft(this._opacityRect),
                                                                        Canvas.GetTop(this._opacityRect)));
                        //Canvas.SetLeft(forSwapSecond, Canvas.GetLeft(this.opacityRect) + 3);
                        //Canvas.SetTop(forSwapSecond, Canvas.GetTop(this.opacityRect) + 3);
                    }
                    canvas.Children.Remove(this._opacityRect);
                    this._opacityRect = null;
                }
                else
                {
                    try
                    {
                        this._game.MoveCards(this._startPlaceType, (int)this._startCoords.X - 1, (int)this._startCoords.Y - 1, this._endPlaceType, (int)this._endCoords.X - 1, (int)this._endCoords.Y - 1);
                        this._game.SetImageOnCoords(this._endPlaceType, (int)(this._endCoords.X - 1), (int)(this._endCoords.Y - 1), image);
                    }
                    catch (AlreadyTakenException)
                    {
                        SettingCanvasOrigin(image, _startPoint);
                        canvas.Children.Remove(this._opacityRect);
                        this._opacityRect = null;
                        return;
                    }
                    catch (NullReferenceException)
                    {
                        SettingCanvasOrigin(image, _startPoint);
                        canvas.Children.Remove(this._opacityRect);
                        this._opacityRect = null;
                        return;
                    }
                    //odejmowanie 
                    SettingCanvasTranslate(image, new Point(Canvas.GetLeft(this._opacityRect),
                                                            Canvas.GetTop(this._opacityRect)));
                    canvas.Children.Remove(this._opacityRect);
                    this._opacityRect = null;
                }
            }
            else
            {
                SettingCanvasOrigin(image, _startPoint);
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
            pop = this._game.IsRandBoardClear();

            this.checker.GameBoard = this._game.GetGameBoard();
            List<List<Id>> collection = this.checker.GetCollection();
            List<CheckerType> infoCollection = this.checker.GetCollectionInfo();
            List<int> Index = new List<int>();

            this._calculate.ListOfCards = collection;

            this._calculate.GetActResult();

            if (this._calculate.LastJokerPromotion > 1000)
            {
                this._calculate.LastJokerPromotion -= 1000;
                this.AddJoker();
            }

            for (int i = 0; i < collection.Count; i++)
            {
                if (this._calculate.CalculateFourResult(collection[i]) >= 100)
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
                    if (this._game.IsFieldFree(i + 1, j + 1, PlaceType.Grid) == false)         // jeśli na danym polu leży kafelek
                    {
                        this._game.BlockField(i, j);
                        Image imageTemp = this._game.GetImageOnCoords(i, j);
                        imageTemp.ManipulationStarted -= ManipulationStarted;
                        imageTemp.ManipulationDelta -= ManipulationDelta;
                        imageTemp.ManipulationCompleted -= ManipulationCompleted;
                    }

                    if (_game.GetBoardField(PlaceType.Grid, i, j) != null)
                    {
                        if (_game.GetBoardField(PlaceType.Grid, i, j).IsJoker == true ||
                            _game.GetBoardField(PlaceType.Grid, i, j).Figure == Figures.Joker)
                        {
                            _game.GetBoardField(PlaceType.Grid, i, j).IsJoker = true;
                        }
                        else
                        {
                            _game.GetBoardField(PlaceType.Grid, i, j).IsJoker = false;
                        }
                    }
                }
            }

            if (pop == true)                // sprawdzenie czy cała plansza jest zajęta (wg mnie niepotrzebnie)
            // Moim też, niedługo to wyjebiemy :)
            {
                CardImage[] cardImage = this._game.RandNewCards();
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

 
            UpdateScore(_calculate.GlobalResult);
            

            if (this._game.HowMuchFreeSpaceOnGameBoard() == 0 && _game.NoJokerOnBoard())
            {
                NavigationService.GoBack();
                this.ClearMemory();
            }
        }

        /// <summary>
        /// Metoda wywoływana podczas powrotu do widoku Panoramy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"> Sender object</param>
        private void BackToPanorama(object sender, EventArgs e)
        {
            NavigationService.GoBack();

            SaveGameState();
        }

        /// <summary>
        /// Metoda wywoływana podczas przyciśnięcia przycisku wstecz.
        /// </summary>
        /// <param name="e"> e </param>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            SaveGameState();

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
            Int64 score = 0;
            Id[,,] memBoard = appMemory.LoadGameState(ref score);

            UpdateScore(score);

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (memBoard[k, i, j] != null)
                        {
                            Image imageToLoad;

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
                                // TODO: niepotrzebne, ale to twój kod - Pajdziu
                                continue;
                            }
                        }
                    }
                }
            }

            _game.SetGameBoard(memBoard);
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
                image = this.controlPanel.GetImageByColorAndId(card.Figure, card.Color);
            }
            else 
            {
                image = this.controlPanel.GetJoker();
            }
            Point point = this.controlPanel.GetGridCoordsForMarkRectangle(x + 1, y + 1);

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
            _jokers = new Image[2];
            Image joker = controlPanel.GetJoker();
            _jokers[x] = joker;
            Point jokerPoint = controlPanel.GetJokerCoordsForMarkRectangle(x + 1, 1);
            this.ManipulationSettings(joker);
            canvas.Children.Add(joker);
            this.SettingCanvasTranslate(joker, jokerPoint);
            this._game.AddJoker(joker, x);
        }

        /// <summary>
        /// Czyszczenie Pamięci.
        /// </summary>
        private void ClearMemory()
        {
            ControlPanel controlClear = new ControlPanel();
            Game gameClear = new Game(controlClear);

            CardImage[] cardImage = gameClear.RandNewCards();

            AppMemory appMemory = new AppMemory();
            appMemory.SaveGameState(gameClear.GetGameBoard(), 0);
        }

        private void ResetGameState(object sender, EventArgs e)
        {
            this.controlPanel = new ControlPanel();
            this._game = new Game(controlPanel);
            this._calculate = new Calculate();

            this.canvas.Children.Clear();

            this.ShowRectangle();
            this.CreateTextBlockForResult();

            CardImage[] cardImage = this._game.RandNewCards();
            Point point = new Point();

            for (int i = 0; i < cardImage.Count(); i++)
            {
                if (cardImage[i] == null)
                {
                    break;
                }

                ManipulationSettings(cardImage[i].image);
                canvas.Children.Add(cardImage[i].image);
                point = controlPanel.GetRandCoordsForMarkRectangle(i + 1, 1);
                SettingCanvasTranslate(cardImage[i].image, point);
            }
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
                            Image image = this._game.DeleteImageOnCoords(3 - i, i);
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
                            Image image = this._game.DeleteImageOnCoords(i, i);
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
                        Image image = this._game.DeleteImageOnCoords(x, i);
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
                        Image image = this._game.DeleteImageOnCoords(i, y);
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
                            Image image = this._game.DeleteImageOnCoords(i, j);
                            canvas.Children.Remove(image);
                        }
                        catch (NullReferenceException) { }
                    }
                }
            }
        }

        /// <summary>
        /// Funkcja zapisująca stan gry i wynik
        /// </summary>
        private void SaveGameState()
        {
            AppMemory appMemory = new AppMemory();
            appMemory.SaveGameState(_game.GetGameBoard(), _calculate.GlobalResult);
        }

        /// <summary>
        /// Funkcja aktualizująca zmienną reprezentującą wynik i odświeżającą widok
        /// </summary>
        /// <param name="score">Aktualny wynik</param>
        private void UpdateScore(Int64 score)
        {
            _calculate.GlobalResult = score;
            _textblock.Text = Convert.ToString(score);
        }
    }
}