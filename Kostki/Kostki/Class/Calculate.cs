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

            return result;
        }
        /// <summary>
        /// Zwraca ilośc punktów za daną konfiguracje kart
        /// </summary>
        /// <param name="list">lista kart w czwórce</param>
        /// <returns>Ilość punktów za konfiguracji</returns>
        public Int64 GetFourResult(List<Id> list)
        {
            Int64 result = 0;
            for (int i = 0; i < list.Count; i++)
            {
                system[(int)list[i].Figure, (int)list[i].Color] = true;
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
