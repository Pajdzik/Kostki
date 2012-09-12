using System;
using System.Windows.Controls;
using Kostki.Exceptions;

namespace Kostki.Class
{
    public class Game
    {
        private Id[,,] gameBoard;
        private ControlPanel controlPanel;

        public Game(ControlPanel controlPanel)
        {
            this.controlPanel = controlPanel;
            this.gameBoard = new Id[4, 4, 4];
        }

        public Id GetBoardField(PlaceType place, int x, int y)
        {
            return this.gameBoard[(int) place, x, y];
        }

        public Id[,,] GetGameBoard() 
        {
            return this.gameBoard;
        }

        public void SetGameBoard(Id[,,] newGameBoard)
        {
            this.gameBoard = newGameBoard;
        }

        private void ClearBoards()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.gameBoard[(int)PlaceType.Grid, i, j] = null;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                this.gameBoard[(int)PlaceType.Rand, i, 0] = null;
            }
        }

        public Boolean IsRandBoardClear() // tymczasowo public
        {
            for (int i = 0; i < 4; i++)
            {
                if (this.gameBoard[(int)PlaceType.Rand, i, 0] != null)
                {
                    return false;
                }
            }
            return true;
        }

        public int HowMuchFreeSpaceOnGameBoard()
        {
            int result = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (this.gameBoard[(int)PlaceType.Grid, i, j] == null)
                    {
                        result++;
                    }
                }
            }
            return result;
        }

        public CardImage[] RandNewCards()
        {
            CardImage[] randImages = new CardImage[4];
            int HowMuch = this.HowMuchFreeSpaceOnGameBoard();
            Random random = new Random();
            int figureType, cardColor;

            if (!this.IsRandBoardClear())
            {
                throw new AlreadyDoneException();
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    randImages[i] = null;
                }

                for (int i = 0; i < Math.Min(HowMuch, 4); i++)
                {
                    figureType = random.Next(4);
                    cardColor = random.Next(4);
                    CardImage cardImage = new CardImage(figureType, cardColor);
                    cardImage.SetImage(this.controlPanel.GetImageByColorAndId(figureType, cardColor));
                    randImages[i] = cardImage;
                    this.gameBoard[(int)PlaceType.Rand, i, 0] = new Id((Figures)figureType, (CardColors)cardColor);
                    this.gameBoard[(int)PlaceType.Rand, i, 0].Image = cardImage.image;
                }
                return randImages;
            }
        }

        public Boolean IsFieldFree(int x, int y, PlaceType placeType)
        {
            x--;
            y--;

            if (placeType == PlaceType.Grid && !(x > 3 || x < 0 || y > 3 || y < 0))
            {
                if (this.gameBoard[(int)PlaceType.Grid, x, y] != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (placeType == PlaceType.Rand && !(x > 3 || x < 0 || y != 0))
            {
                if (this.gameBoard[(int)PlaceType.Rand, x, y] != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public void MoveCards(PlaceType start, int startX, int startY, PlaceType end, int endX, int endY)
        {
            try
            {
                if (this.gameBoard[(int)end, endX, endY] != null)
                {
                    throw new AlreadyTakenException();
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new AlreadyTakenException();
            }
            catch (AlreadyTakenException)
            {
                throw new AlreadyTakenException();
            }

            Id id = this.gameBoard[(int)start, startX, startY];
            if (start == PlaceType.Rand)
            {
                this.gameBoard[(int)start, startX, startY] = null;
            }
            else
            {
                this.gameBoard[(int)start, startX, startY] = null;
            }
            this.gameBoard[(int)end, endX, endY] = id;
            return;
        }

        /// <summary>
        /// Funkcja blokująca podany kafelek. Wykorzystywana po wciśnięciu przyciski next [apply] na appbarze.
        /// </summary>
        /// <param name="x">Współrzędna X</param>
        /// <param name="y">Współrzędna Y</param>

        public void BlockField(int x, int y)
        {
            if (x < 4 && y < 4)
            {
                this.gameBoard[(int) PlaceType.Grid, x, y].Blocked = true;
            }
        }

        /// <summary>
        /// Funkcja zwracająca stan zablokowania danego kafelka. 
        /// </summary>
        /// <param name="x">Współrzędna X</param>
        /// <param name="y">Współrzędna Y</param>
        /// <returns>False, gdy kafelkiem możemy poruszać. True w przeciwnym wypadku</returns>

        public Boolean IsFieldBlocked(int x, int y)
        {
            return this.gameBoard[(int)PlaceType.Grid, x, y].Blocked;
        }

        public void SetImageOnCoords(PlaceType place, int x, int y, Image image)
        {
            this.gameBoard[(int)place, x, y].Image = image;
        }

        public Image GetImageOnCoords(int x, int y)
        {
            return this.gameBoard[(int)PlaceType.Grid, x, y].Image;
        }

        public Image GetImageOnCoords(PlaceType place, int x, int y)
        {
            return this.gameBoard[(int)place, x, y].Image;
        }

        public Image DeleteImageOnCoords(int x, int y)
        {
            Image image = this.gameBoard[(int)PlaceType.Grid, x, y].Image;
            this.gameBoard[(int)PlaceType.Grid, x, y] = null;
            return image;
        }

        public void SetJokerOnCoords(int x, int y)
        {
            try
            {
                this.gameBoard[(int)PlaceType.Grid, x, y].IsJoker = true;
            }
            catch (NullReferenceException)
            {
                this.gameBoard[(int)PlaceType.Grid, x, y] = new Id(Figures.Joker, CardColors.Joker);
                this.gameBoard[(int)PlaceType.Grid, x, y].IsJoker = true;
            }
        }

        public void SetJokerOnCoords(PlaceType place, int x, int y)
        {
            try
            {
                this.gameBoard[(int)place, x, y].IsJoker = true; 
            }
            catch (NullReferenceException)
            {
                this.gameBoard[(int)place, x, y] = new Id(Figures.Joker, CardColors.Joker);
                this.gameBoard[(int)place, x, y].IsJoker = true;
            }
        }

        public Boolean GetJokerOnCoords(PlaceType place, int x, int y)
        {
            return this.gameBoard[(int)place, x, y].IsJoker;
        }

        /// <summary>
        /// Metoda pozwalająca panować nad przesuwaniem jokerów. Jest kilka sytuacji,
        /// jakie mogą wystapić, wszystkie one muszą zostać sprawdzone, by aplikacja
        /// działała poprawnie. Jednym z przypadków jest taki, kiedy przekładamy jokera
        /// na miejsce, które jest puste. Reszta przypadków okomentowana jest w kodzie.
        /// </summary>
        /// <param name="start"> Typ miejsca początkowego</param>
        /// <param name="startX"> Wpółrzędna x startu</param>
        /// <param name="startY"> Współrzędna y startu</param>
        /// <param name="end"> Typ miejsca końcowego</param>
        /// <param name="endX"> Współrzędna x końca</param>
        /// <param name="endY"> Współrzędna y końca</param>
        public void MoveJoker(PlaceType start, int startX, int startY, PlaceType end, int endX, int endY)
        {
            // Jeśli miejsce, na które przeciągamy jokera jest puste
            if (this.gameBoard[(int)end, endX, endY] == null)
            {
                // Jeśli przeciągamy bezpośrednio jokera
                if (this.gameBoard[(int)start, startX, startY].Figure == Figures.Joker)
                {
                    this.gameBoard[(int)end, endX, endY] = this.gameBoard[(int)start, startX, startY];
                    this.gameBoard[(int)start, startX, startY] = null;
                }
                // Jeśli joker leży (przykrywa) inną kartę.
                else
                {
                    this.gameBoard[(int)start, startX, startY].IsJoker = false;
                    this.SetJokerOnCoords(end, endX, endY);
                }
            }
            // Jeśli miejsce, na które przeciągamy jokera na miejsce z jakąś kartą
            else
            {
                // Jeśli przeciągamy bezpośrednio jokera
                if (this.gameBoard[(int)start, startX, startY].Figure == Figures.Joker)
                {
                    this.gameBoard[(int)end, endX, endY].IsJoker = true;
                    this.gameBoard[(int)start, startX, startY] = null;
                }
                // Jeśli joker leży (przykrywa) inną kartę.
                else
                {
                    this.gameBoard[(int)start, startX, startY].IsJoker = false;
                    this.gameBoard[(int)end, endX, endY].IsJoker = true;
                }
            }
        }

        /// <summary>
        /// Dodanie jokera na pozycję x w miejscu dla Jokerów.
        /// </summary>
        /// <param name="image"> Obrazek, który należy wyświetlić</param>
        /// <param name="x"> Miejsce (index), w którym ma zostać wyświetlony obrazek</param>
        public void AddJoker(Image image, int x)
        {
            this.gameBoard[(int)PlaceType.Joker, x, 0] = new Id(Figures.Joker, CardColors.Joker);
            this.gameBoard[(int)PlaceType.Joker, x, 0].Image = image;
            this.gameBoard[(int)PlaceType.Joker, x, 0].IsJoker = true;
        }

        /// <summary>
        /// Sprawdzenie czy jest jakiś Joker na planszy
        /// </summary>
        /// <returns> True jeśli nie ma żadnych Jokerów, false, jeśli
        /// jest inaczej</returns>
        public Boolean NoJokerOnBoard()
        {
            for (int i = 0; i < 2; i++)
            {
                if (this.gameBoard[(int)PlaceType.Joker, i, 0] != null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
