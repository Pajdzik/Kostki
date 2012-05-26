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
using Kostki.Exceptions;
using Kostki.Class;
using System.Diagnostics;

namespace Kostki.Class
{
    public class Game
    {
        private Id[,,] GameBoard;
        private Id[] RandBoard;
        private ControlPanel controlPanel;

        public Game(ControlPanel controlPanel)
        {
            this.controlPanel = controlPanel;
            this.GameBoard = new Id[4, 4, 4];
            this.RandBoard = new Id[4];
        }

        public Id[,,] GetGameBoard() 
        {
            return this.GameBoard;
        }

        private void ClearBoards()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.GameBoard[(int)PlaceType.Grid,i, j] = null;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                this.GameBoard[(int)PlaceType.Rand, i, 0] = null;
            }
        }

        public Boolean IsRandBoardClear() //tymczasowo public
        {
            for (int i = 0; i < 4; i++)
            {
                if (this.GameBoard[(int)PlaceType.Rand,i, 0] != null)
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
                    if (this.GameBoard[(int)PlaceType.Grid, i, j] == null)
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
            int FigureType, CardColor;

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

                for (int i = 0; i < Math.Min(HowMuch,4); i++)
                {
                    FigureType = random.Next(4);
                    CardColor = random.Next(4);
                    CardImage cardImage = new CardImage(FigureType, CardColor);
                    cardImage.SetImage(this.controlPanel.GetImageByColorAndId(FigureType, CardColor));
                    randImages[i] = cardImage;
                    this.GameBoard[(int)PlaceType.Rand, i, 0] = new Id((Figures)FigureType, (CardColors)CardColor);
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
                if (this.GameBoard[(int)PlaceType.Grid,x, y] != null)
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
                if (this.GameBoard[(int)PlaceType.Rand,x,y] != null)
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
                if (this.GameBoard[(int)end, endX, endY] != null)
                {
                    throw new AlreadyTakenException();
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new AlreadyTakenException();
            }

            Id id = this.GameBoard[(int)start, startX, startY];
            if (start == PlaceType.Rand)
            {
                this.GameBoard[(int)start, startX, startY] = null;
            }
            else
            {
                this.GameBoard[(int)start, startX, startY] = null;
            }
            this.GameBoard[(int)end, endX, endY] = id;
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
                this.GameBoard[(int) PlaceType.Grid, x, y].Blocked = true;
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
            return this.GameBoard[(int) PlaceType.Grid, x, y].Blocked;
        }

        public void SetImageOnCoords(PlaceType place, int x, int y, Image image)
        {
            this.GameBoard[(int)place, x, y].Image = image;
        }

        public Image GetImageOnCoords(int x, int y)
        {
            return this.GameBoard[(int)PlaceType.Grid, x, y].Image;
        }

        public Image DeleteImageOnCoords(int x, int y)
        {
            Debug.WriteLine("Usuwam " + x + "  " + y);
            Image image = this.GameBoard[(int)PlaceType.Grid, x, y].Image;
            this.GameBoard[(int)PlaceType.Grid, x, y] = null;
            return image;

        }
    }
}
