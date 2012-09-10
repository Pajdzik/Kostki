using System;
using System.Collections.Generic;

namespace Kostki.Class
{
    public class Calculate
    {
        private List<List<Id>> listOfCards;
        private Int64 globalResult;             // wynik
        private Boolean[,] system;
        private Int64 lastJokerPromotion;

        public Calculate()
        {
            // TODO: pomyśleć czy konstruktor jest w ogóle potrzebny 
            this.system = new Boolean[5, 5];
            this.globalResult = 0;
        }

        public Int64 LastJokerPromotion
        {
            get { return this.lastJokerPromotion; }
            set { this.lastJokerPromotion = value; }
        }

        public List<List<Id>> ListOfCards
        {
            get { return this.listOfCards; }
            set { this.listOfCards = value; }
        }

        public Int64 GlobalResult
        {
            get { return this.globalResult; }
            set { this.globalResult = value; }
        }

        /// <summary>
        /// Zwraca ilość punktów za rozkład na całej planszy
        /// </summary>
        /// <returns>Ilość punktów za rozkład na całej planszy</returns>
        public Int64 GetActResult()
        {
            Int64 result = 0;

            foreach (List<Id> t in this.listOfCards)
            {
                Boolean isJoker = false;

                foreach (Id t1 in t)
                {
                    if (t1.IsJoker == true)
                        isJoker = true;
                }

                if (isJoker)
                {
                    result += this.GetEveryFourResult(t);
                }
                else
                {
                    result += this.GetFourResult(t);
                }
            }
            
            return result;
        }

        public void ActualizeScore(Int64 reward)
        {
            this.lastJokerPromotion += reward;
            this.globalResult += reward;
        }

        public Int64 CalculateFourResult(List<Id> list)
        {
            Int64 result = 0;
            Boolean isJoker = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsJoker == true)
                {
                    isJoker = true;
                }
            }
            if (isJoker)
            {
                result += this.GetEveryFourResult(list);
            }
            else
            {
                result += this.GetFourResult(list);
            }

            return result;
        }

        public Int64 GetEveryFourResult(List<Id> list)
        {
            Int64 result = 0;
            List<Id> newList = new List<Id>();
            Id tempId;
            Id tempIdSecond;
            Int64 tempResult = 0;

            newList.Clear();
            
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsJoker != true)
                {
                    newList.Add(list[i]);
                }
            }


            if (newList.Count == 3) // 3
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        tempId = new Id((Figures)i, (CardColors)j);
                        newList.Add(tempId);
                        tempResult = this.GetFourResult(newList);
                        if (tempResult > result)
                        {
                            result = tempResult;
                        }
                        newList.Remove(tempId);
                    }
                }
            } 
            else 
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        tempId = new Id((Figures)i, (CardColors)j);
                        newList.Add(tempId);
                        for (int k = 0; k < 4; k++)
                        {
                            for (int l = 0; l < 4; l++)
                            {
                                tempIdSecond = new Id((Figures)k, (CardColors)l);
                                newList.Add(tempIdSecond);
                                tempResult = this.GetFourResult(newList);
                                if (tempResult > result)
                                {
                                    result = tempResult;
                                }
                                newList.Remove(tempIdSecond);
                            }
                        }
                        newList.Remove(tempId);
                    }
                }
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
            this.ClearSystem();
            Int64 result = 0;
            for (int i = 0; i < list.Count; i++)
            {
                this.system[(int)list[i].Figure, (int)list[i].Color] = true;
            }

            for (int i = 0; i < 4; i++)
            {
                result += this.GetRowOrColumnCount(i, true);
            }

            // same figure, same color
            if (result == 1)
            {
                return 400;
            }

            // same figure, each color
            for (int i = 0; i < 4; i++)
            {
                if (this.GetRowOrColumnCount(i, false) == 4)
                {
                    return 300;
                }
            }

            // same color, each figure
            for (int i = 0; i < 4; i++)
            {
                if (this.GetRowOrColumnCount(i, true) == 4)
                {
                    return 200;
                }
            }

            Boolean notCross = false;

            for (int i = 0; i < 4; i++)
            {
                if (this.GetRowOrColumnCount(i, true) != 1) notCross = true;
            }

            for (int i = 0; i < 4; i++)
            {
                if (this.GetRowOrColumnCount(i, false) != 1) notCross = true;
            }

            // each color, each figure
            if (!notCross)
            {
                return 100;
            }

            Boolean[] colors = new Boolean[4];
            int howMuchRowsNotClear = 0;

            for (int i = 0; i < 4; i++)
            {
                colors[i] = (this.GetRowOrColumnCount(i, true) > 0) ? true : false;
            }

            for (int i = 0; i < 4; i++)
            {
                howMuchRowsNotClear += (colors[i]) ? 1 : 0;
            }

            // same color
            if (howMuchRowsNotClear == 1)
            {
                return 60;
            }

            Boolean[] figures = new Boolean[4];
            int howMuchColumnsNotClear = 0;

            for (int i = 0; i < 4; i++)
            {
                figures[i] = (this.GetRowOrColumnCount(i, false) > 0) ? true : false;
            }

            for (int i = 0; i < 4; i++)
            {
                howMuchColumnsNotClear += (figures[i]) ? 1 : 0;
            }

            // same figure
            if (howMuchColumnsNotClear == 1)
            {
                return 50;
            }

            figures = new Boolean[4];
            int howMuchFigures = 0;

            for (int i = 0; i < 4; i++)
            {
                figures[i] = (this.GetRowOrColumnCount(i, false) > 0) ? true : false;
            }

            for (int i = 0; i < 4; i++)
            {
                howMuchFigures += (figures[i]) ? 1 : 0;
            }

            // each figure
            if (howMuchFigures == 4)
            {
                return 40;
            }

            colors = new Boolean[4];
            int howMuchColors = 0;

            for (int i = 0; i < 4; i++)
            {
                colors[i] = (this.GetRowOrColumnCount(i, true) > 0) ? true : false;
            }

            for (int i = 0; i < 4; i++)
            {
                howMuchColors += (colors[i]) ? 1 : 0;
            }

            // each color
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
                result += (this.system[i, id] && rowOrColumn) ? 1 : 0;
                result += (this.system[id, i] && !rowOrColumn) ? 1 : 0;
            }

            return result;
        }

        public void ClearSystem()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.system[i, j] = false;
                }
            }
        }

    }
}
