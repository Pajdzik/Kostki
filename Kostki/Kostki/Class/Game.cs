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
        private Id[,] GameBoard;
        private Id[] RandBoard;
        private ControlPanel controlPanel;

        public Game(ControlPanel controlPanel)
        {
            this.controlPanel = controlPanel;
            this.GameBoard = new Id[4, 4];
            this.RandBoard = new Id[4];
        }

        private void ClearBoards()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.GameBoard[i, j] = null;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                this.RandBoard[i] = null;
            }
        }

        private Boolean IsRandBoardClear()
        {
            for (int i = 0; i < 4; i++)
            {
                if (this.RandBoard[i] != null)
                {
                    return false;
                }
            }
            return true;
        }

        private int HowMuchFreeSpaceOnGameBoard()
        {
            int result = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (this.GameBoard[i, j] == null)
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
                }
                return randImages;
            }
        }

        public Boolean IsFieldFree(int x, int y, PlaceType placeType)
        {
            if (placeType == PlaceType.Grid && !(x > 4 || x < 1 || y > 4 || y < 1))
            {
                if (this.GameBoard[x, y] != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (placeType == PlaceType.Rand && !(x > 4 || x < 1 || y != 1))
            {
                if (this.RandBoard[x] != null)
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
    }
}
