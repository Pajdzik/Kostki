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
using Kostki.Class;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kostki.Class
{
    public class Checker
    {

        private Id[, ,] gameBoard;

        public Id[, ,] GameBoard
        {
            get { return gameBoard; }
            set { gameBoard = value; }
        }

        /// <summary>
        /// Funkcja zwracająca Listę list wszystkich czwórek na planszy.
        /// W kolejności . Wierwsze, kolumny, kwadraty, skosy (lewy, prawy), 
        /// skrajne
        /// </summary>
        /// <returns>Lista List elementów</returns>
        public List<List<Id>> GetCollection()
        {
            List<List<Id>> resultList = new List<List<Id>>();
            List<List<Id>> tempList = new List<List<Id>>();

            tempList = GetFourInRows();

            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList[i] != null)
                {
                    resultList.Add(tempList[i]);
                }
            }

            tempList = GetFourInColumns();

            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList[i] != null)
                {
                    resultList.Add(tempList[i]);
                }
            }

            tempList = GetFourRectangle();

            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList[i] != null)
                {
                    resultList.Add(tempList[i]);
                }
            }

            tempList = GetFourCross();

            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList[i] != null)
                {
                    resultList.Add(tempList[i]);
                }
            }

            if (GetCorners() != null)
            {
                resultList.Add(GetCorners());
            }

            return resultList;
        }

        public List<Id> GetCorners()
        {
            List<Id> result = new List<Id>();
            int x = 0, y = 0, howMuchBlocked = 0;
            for (int i = 0; i < 4; i++)
            {
                y = (i % 2 == 1) ? 3 : 0; // Linijki specjalnie dla Ciebie pajdziu :D
                x = (i >> 1 == 1) ? 3 : 0; // Siedziałem nad nimi dłużej niż nad normalnymi ifami bym siedział :D ale są kochane <3

                result.Add(this.gameBoard[(int)PlaceType.Grid, x, y]);
                if (this.gameBoard[(int)PlaceType.Grid, x, y] == null)
                {
                    return null;
                }
                else if (this.gameBoard[(int)PlaceType.Grid, x, y].Blocked == true && !this.gameBoard[(int)PlaceType.Grid, x, y].IsJokerBlocked)
                {
                    howMuchBlocked++;
                }
            }

            if (howMuchBlocked == 4)
            {
                return null;
            }

            return result;
        }

        public List<List<Id>> GetFourCross()
        {
            List<List<Id>> result = new List<List<Id>>();

            List<Id> leftCross = new List<Id>();
            List<Id> rightCross = new List<Id>();

            int howMuchBlocked = 0;

            for (int i = 0; i < 4; i++)
            {
                leftCross.Add(this.gameBoard[(int)PlaceType.Grid, i, i]);
                if (this.gameBoard[(int)PlaceType.Grid, i, i] == null)
                {
                    leftCross = null;
                    break;
                }
                else if (this.gameBoard[(int)PlaceType.Grid, i, i].Blocked == true && !this.gameBoard[(int)PlaceType.Grid, i,i].IsJokerBlocked)
                {
                    howMuchBlocked++;
                }
            }

            if (howMuchBlocked == 4)
            {
                leftCross = null;
            }

            howMuchBlocked = 0;

            for (int i = 0; i < 4; i++)
            {
                rightCross.Add(this.gameBoard[(int)PlaceType.Grid, 3-i, i]);
                if (this.gameBoard[(int)PlaceType.Grid, 3-i, i] == null)
                {
                    rightCross = null;
                    break;
                }
                else if (this.gameBoard[(int)PlaceType.Grid, 3 - i, i].Blocked == true && this.gameBoard[(int)PlaceType.Grid, 3 - i, i].IsJokerBlocked == false)
                {
                    howMuchBlocked++;
                }
            }

            if (howMuchBlocked == 4)
            {
                rightCross = null;
            }

            result.Add(leftCross);
            result.Add(rightCross);

            return result; 
        }

        /// <summary>
        /// Funkcja zwracająca elementy we wszystkich kwadratach 2x2.
        /// Elementy sa zwracane w postaci Listy< List <Id > >
        /// Elementy są zwracane w kolejnosci rosnących kolumn a potem wierszy dla współrzędnych początku kwadratu.
        /// Czyli 0,0 - 0,1 - 0,2 - 1,0 - 1,1 itd.
        /// </summary>
        /// <returns>Lista List list elementów</returns>
        public List<List<Id>> GetFourRectangle()
        {
            List<List<Id>> result = new List<List<Id>>();
            List<Id> inside = new List<Id>();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    inside = GetFourRectangle(i, j);
                    if (inside != null)
                    {
                        result.Add(inside);
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Funkcja zwracająca elementy w kwadracie 2x2 o określonych współrzędnych poczatku.
        /// Elementy sa zwracane w postaci Listy< Id >
        /// </summary>
        /// <returns>Lista elementów</returns>
        public List<Id> GetFourRectangle(int x, int y)
        {
            List<Id> result = new List<Id>();

            int howMuchBlocked = 0;

            for (int i = x; i < x + 2; i++)
            {
                for (int j = y; j < y + 2; j++)
                {
                    result.Add(this.gameBoard[(int)PlaceType.Grid, i,j]);
                    if (this.gameBoard[(int)PlaceType.Grid, i, j] == null)
                    {
                        return null;
                    }
                    else if (this.gameBoard[(int)PlaceType.Grid, i, j].Blocked == true && !this.gameBoard[(int)PlaceType.Grid, i,j].IsJokerBlocked)
                    {
                        howMuchBlocked++;
                    }
                }
            }

            if (howMuchBlocked == 4)
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Funkcja zwracająca elementy we wszystkich wierszach.
        /// Elementy sa zwracane w postaci Listy< List <Id > >
        /// Elementy są zwracane w kolejnosci rosnących wierszy.
        /// </summary>
        /// <returns>Lista List elementów</returns>
        public List<List<Id>> GetFourInRows()
        {

            List<List<Id>> result = new List<List<Id>>();

            for (int i = 0; i < 4; i++)
            {
                List<Id> insideList = new List<Id>();
                insideList = GetFourInRow(i);
                if (insideList != null)
                {
                    result.Add(insideList);
                }
            }

            return result;
        }

        /// <summary>
        /// Funkcja zwracająca elementy w wierszu o numerze Id.
        /// Elementy sa zwracane w postaci Listy< Id >
        /// </summary>
        /// <param name="id">Numer wierwsza do odczytu</param>
        /// <returns>Lista elementów</returns>
        public List<Id> GetFourInRow(int id)
        {
            List<Id> result = new List<Id>();

            int howMuchBlocked = 0;

            for (int i = 0; i < 4; i++)
            {
                result.Add(this.gameBoard[(int)PlaceType.Grid, i, id]);
                if (this.gameBoard[(int)PlaceType.Grid, i, id] == null)
                {
                    return null;
                }
                else if (this.gameBoard[(int)PlaceType.Grid, i, id].Blocked == true && !this.gameBoard[(int)PlaceType.Grid, i, id].IsJokerBlocked)
                {
                    howMuchBlocked++;
                }
            }

            if (howMuchBlocked == 4)
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Funkcja zwracająca elementy we wszystkich kolumnach.
        /// Elementy sa zwracane w postaci Listy< List <Id > >
        /// Elementy są zwracane w kolejnosci rosnących kolumn.
        /// </summary>
        /// <returns>Lista List elementów</returns>
        public List<List<Id>> GetFourInColumns()
        {
            List<List<Id>> result = new List<List<Id>>();

            for (int i = 0; i < 4; i++)
            {
                List<Id> insideList = new List<Id>();
                insideList = GetFourInColumn(i);
                if (insideList != null)
                {
                    result.Add(insideList);
                }
            }

            return result;
        }

        /// <summary>
        /// Funkcja zwracająca elementy w kolumn o numerze Id.
        /// Elementy sa zwracane w postaci Listy< Id >
        /// </summary>
        /// <param name="id">Numer kolumny do odczytu</param>
        /// <returns>Lista elementów</returns>
        public List<Id> GetFourInColumn(int id)
        {
            List<Id> result = new List<Id>();

            int howMuchBlocked = 0;

            for (int i = 0; i < 4; i++)
            {
                result.Add(this.gameBoard[(int)PlaceType.Grid, id, i]);
                if (this.gameBoard[(int)PlaceType.Grid, id, i] == null)
                {
                    return null;
                }
                else if (this.gameBoard[(int)PlaceType.Grid, id, i].Blocked == true && !this.gameBoard[(int)PlaceType.Grid, id, i].IsJokerBlocked)
                {
                    howMuchBlocked++;
                }
            }

            if (howMuchBlocked == 4)
            {
                return null;
            }

            return result;
        }


        /// Poniższe funkcje robią dokładnie to samo co te wyżej, jednak w wnynikiu zwracją 
        /// zmienną typu Checkertype, która informuje co to jest za zaznaczony fragment 
        /// i gdzie on się zacyzna. Wystarczy wyifować (:
        /// Czyli pobieramy GetCollection i getCollectionInfo i mamy (:


        public List<CheckerType> GetCollectionInfo()
        {
            List<CheckerType> resultList = new List<CheckerType>();
            List<CheckerType> tempList = new List<CheckerType>();

            tempList = GetFourInRowsInfo();

            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList[i] != null)
                {
                    resultList.Add(tempList[i]);
                }
            }

            tempList = GetFourInColumnsInfo();

            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList[i] != null)
                {
                    resultList.Add(tempList[i]);
                }
            }

            tempList = GetFourRectangleInfo();

            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList[i] != null)
                {
                    resultList.Add(tempList[i]);
                }
            }

            tempList = GetFourCrossInfo();

            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList[i] != null)
                {
                    resultList.Add(tempList[i]);
                }
            }

            if (GetCornersInfo() != null)
            {
                resultList.Add(GetCornersInfo());
            }

            return resultList;
        }

        public CheckerType GetCornersInfo()
        {
            int x = 0, y = 0, howMuchBlocked = 0;
            for (int i = 0; i < 4; i++)
            {
                y = (i % 2 == 1) ? 3 : 0; // Linijki specjalnie dla Ciebie pajdziu :D
                x = (i >> 1 == 1) ? 3 : 0; // Siedziałem nad nimi dłużej niż nad normalnymi ifami bym siedział :D ale są kochane <3

                if (this.gameBoard[(int)PlaceType.Grid, x, y] == null)
                {
                    return null;
                }
                else if (this.gameBoard[(int)PlaceType.Grid, x, y].Blocked == true && !this.gameBoard[(int)PlaceType.Grid, x, y].IsJokerBlocked)
                {
                    howMuchBlocked++;
                }
            }

            if (howMuchBlocked == 4)
            {
                return null;
            }

            return new CheckerType(0, 0, FourcardType.Corners);
        }

        public List<CheckerType> GetFourCrossInfo()
        {
            List<CheckerType> result = new List<CheckerType>();
            CheckerType leftCross = new CheckerType(0, 0, FourcardType.Cross) ;
            CheckerType rightCross = new CheckerType(3, 0, FourcardType.Cross);
            int howMuchBlocked = 0;

            for (int i = 0; i < 4; i++)
            {
                if (this.gameBoard[(int)PlaceType.Grid, i, i] == null)
                {
                    leftCross = null;
                    break;
                }
                else if (this.gameBoard[(int)PlaceType.Grid, i, i].Blocked == true && !this.gameBoard[(int)PlaceType.Grid, i, i].IsJokerBlocked)
                {
                    howMuchBlocked++;
                }
            }

            if (howMuchBlocked == 4)
            {
                leftCross = null;
            }

            howMuchBlocked = 0;

            for (int i = 0; i < 4; i++)
            {
                if (this.gameBoard[(int)PlaceType.Grid, 3 - i, i] == null)
                {
                    rightCross = null;
                    break;
                }
                else if (this.gameBoard[(int)PlaceType.Grid, 3 - i, i].Blocked == true && !this.gameBoard[(int)PlaceType.Grid, 3-i,i].IsJokerBlocked)
                {
                    howMuchBlocked++;
                }
            }

            if (howMuchBlocked == 4)
            {
                rightCross = null;
            }

            result.Add(leftCross);
            result.Add(rightCross);

            return result;
        }

        public List<CheckerType> GetFourRectangleInfo()
        {
            List<CheckerType> result = new List<CheckerType>();
            CheckerType inside;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    inside = GetFourRectangleInfo(i, j);
                    if (inside != null)
                    {
                        result.Add(inside);
                    }
                }
            }

            return result;
        }

        public CheckerType GetFourRectangleInfo(int x, int y)
        {

            int howMuchBlocked = 0;

            for (int i = x; i < x + 2; i++)
            {
                for (int j = y; j < y + 2; j++)
                {
                    if (this.gameBoard[(int)PlaceType.Grid, i, j] == null)
                    {
                        return null;
                    }
                    else if (this.gameBoard[(int)PlaceType.Grid, i, j].Blocked == true && !this.gameBoard[(int)PlaceType.Grid, i, j].IsJokerBlocked)
                    {
                        howMuchBlocked++;
                    }
                }
            }

            if (howMuchBlocked == 4)
            {
                return null;
            }

            return new CheckerType(x, y, FourcardType.Rectangle); ;
        }

        public List<CheckerType> GetFourInRowsInfo()
        {

            List<CheckerType> result = new List<CheckerType>();
            CheckerType insideList;

            for (int i = 0; i < 4; i++)
            {
                insideList = GetFourInRowInfo(i);
                if (insideList != null)
                {
                    result.Add(insideList);
                }
            }

            return result;
        }

        public CheckerType GetFourInRowInfo(int id)
        {

            int howMuchBlocked = 0;

            for (int i = 0; i < 4; i++)
            {
                if (this.gameBoard[(int)PlaceType.Grid, i, id] == null)
                {
                    return null;
                }
                else if (this.gameBoard[(int)PlaceType.Grid, i, id].Blocked == true && !this.gameBoard[(int)PlaceType.Grid, i, id].IsJokerBlocked)
                {
                    howMuchBlocked++;
                }
            }

            if (howMuchBlocked == 4)
            {
                return null;
            }

            return new CheckerType(0, id, FourcardType.Row);
        }

        public List<CheckerType> GetFourInColumnsInfo()
        {
            List<CheckerType> result = new List<CheckerType>();
            CheckerType insideList;

            for (int i = 0; i < 4; i++)
            {
                insideList = GetFourInColumnInfo(i);
                if (insideList != null)
                {
                    result.Add(insideList);
                }
            }

            return result;
        }

        public CheckerType GetFourInColumnInfo(int id)
        {
            int howMuchBlocked = 0;

            for (int i = 0; i < 4; i++)
            {
                if (this.gameBoard[(int)PlaceType.Grid, id, i] == null)
                {
                    return null;
                }
                else if (this.gameBoard[(int)PlaceType.Grid, id, i].Blocked == true && !this.gameBoard[(int)PlaceType.Grid, id, i].IsJokerBlocked)
                {
                    howMuchBlocked++;
                }
            }

            if (howMuchBlocked == 4)
            {
                return null;
            }

            return new CheckerType(id, 0, FourcardType.Column); ;
        }
    }
}
