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

            if (result == 1)
            {
                return 400;
            }

            for (int i = 0; i < 4; i++)
            {
                if (GetRowOrColumnCount(i, false) == 4)
                {
                    return 300;
                }
            }

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

            if (!notCross)
            {
                return 100;
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
