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
using System.Collections.Generic;
using Kostki.Class;
using System.Diagnostics;

namespace Kostki.Class
{
    public class Calculate
    {
        private List<List<Id>> listOfCards;
        private Int64 globalResult;
        private Boolean[,] system;

        public List<List<Id>> ListOfCards 
        {
            get { return listOfCards; }
            set { listOfCards = value; }
        }

        public Int64 GlobalResult
        {
            get { return globalResult; }
            set { globalResult = value; }
        }
        /// pomyśleć czy konstruktor jest w ogóle potrzebny 
        public Calculate()
        {
            system = new Boolean[4, 4];
            globalResult = 0;
        }
        /// <summary>
        /// Zwraca ilość punktów za rozkład na całej planszy
        /// </summary>
        /// <returns>Ilość punktów za rozkład na całej planszy</returns>
        public Int64 GetActResult()
        {
            Int64 result = 0;

            for (int i = 0; i < listOfCards.Count; i++)
            {
                result += GetFourResult(listOfCards[i]);
            }

            this.globalResult += result;

            return result;
        }
        /// <summary>
        /// Zwraca ilośc punktów za daną konfiguracje kart
        /// </summary>
        /// <param name="list">lista kart w czwórce</param>
        /// <returns>Ilość punktów za konfiguracji</returns>
        public Int64 GetFourResult(List<Id> list)
        {
            ClearSystem();
            Int64 result = 0;
            for (int i = 0; i < list.Count; i++)
            {
                system[(int)list[i].Figure, (int)list[i].Color] = true;
            }

            for (int i = 0; i < 4; i++)
            {
                result += GetRowOrColumnCount(i, true);
            }

            //same figure, same color
            if (result == 1)
            {
                return 400;
            }

            //same figure, each color
            for (int i = 0; i < 4; i++)
            {
                if (GetRowOrColumnCount(i, false) == 4)
                {
                    return 300;
                }
            }

            //same color, each figure
            for (int i = 0; i < 4; i++)
            {
                if (GetRowOrColumnCount(i, true) == 4)
                {
                    return 200;
                }
            }

            Boolean notCross = false;

            for (int i = 0; i < 4; i++)
            {
                if (GetRowOrColumnCount(i, true) != 1) notCross = true;
            }

            for (int i = 0; i < 4; i++)
            {
                if (GetRowOrColumnCount(i, false) != 1) notCross = true;
            }

            //each color, each figure
            if (!notCross)
            {
                return 100;
            }

            Boolean []colors = new Boolean[4];
            int howMuchRowsNotClear = 0;

            for (int i = 0; i < 4; i++)
            {
                colors[i] = (GetRowOrColumnCount(i, true) > 0) ? true : false;
            }

            for (int i = 0; i < 4; i++) 
            {
                howMuchRowsNotClear += (colors[i]) ? 1 : 0;
            }

            //same color
            if (howMuchRowsNotClear == 1)
            {
                return 60;
            }

            Boolean[] figures = new Boolean[4];
            int howMuchColumnsNotClear = 0;

            for (int i = 0; i < 4; i++)
            {
                figures[i] = (GetRowOrColumnCount(i, false) > 0) ? true : false;
            }

            for (int i = 0; i < 4; i++)
            {
                howMuchColumnsNotClear += (figures[i]) ? 1 : 0;
            }

            //same figure
            if (howMuchColumnsNotClear == 1)
            {
                return 50;
            }

            figures = new Boolean[4];
            int howMuchFigures = 0;

            for (int i = 0; i < 4; i++)
            {
                figures[i] = (GetRowOrColumnCount(i, false) > 0) ? true : false;
            }

            for (int i = 0; i < 4; i++)
            {
                howMuchFigures += (figures[i]) ? 1 : 0;
            }

            //each figure
            if (howMuchFigures == 4)
            {
                return 40;
            }

            colors = new Boolean[4];
            int howMuchColors = 0;

            for (int i = 0; i < 4; i++)
            {
                colors[i] = (GetRowOrColumnCount(i, true) > 0) ? true : false;
            }

            for (int i = 0; i < 4; i++)
            {
                howMuchColors += (colors[i]) ? 1 : 0;
            }

            //each color
            if (howMuchColors == 4)
            {
                return 30;
            }

            return 0; // tymczasowo
        }

        public int GetRowOrColumnCount(int id, Boolean rowOrColumn)
        {
            int result = 0;

            for (int i = 0; i < 4; i++)
            {
                result += (system[i, id] && rowOrColumn) ? 1 : 0;
                result += (system[id, i] && !rowOrColumn) ? 1 : 0;
            }

            return result;
        }

        public void ClearSystem()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    system[i, j] = false;
                }
            }
        }

    }
}
