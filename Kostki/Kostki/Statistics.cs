using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kostki
{
    public class Statistics
    {
        public List<KeyValuePair<string, Int64>> TopTen;
        private string _playerName;
        private readonly uint _amountOfPlaces = 10;

        public Statistics()
        {
            LoadPlayerName();
            LoadTopTen();
        }

        /// <summary>
        /// Funkcja wczytująca z pamięci telefonu dziesięć najlepszych wyników
        /// </summary>
        private void LoadTopTen()
        {
            AppMemory am = new AppMemory();

            TopTen = new List<KeyValuePair<string, Int64>>();


            for (int i = 1; i <= _amountOfPlaces; i++)
            {
                string playerName = am.GetValueOrDefault<string>("playerName" + Convert.ToString(i), "Player");
                Int64 score = am.GetValueOrDefault<Int64>("score" + Convert.ToString(i), 0);

                TopTen.Add(new KeyValuePair<string, long>(playerName, score));
            }
        }

        private void LoadPlayerName()
        {
            AppMemory am = new AppMemory();

            _playerName = am.GetValueOrDefault("playerName", "Player");
        }


        /// <summary>
        /// Funkcja zapisuje wynik w TOP 10
        /// </summary>
        /// <param name="score">Wynik skończonej gry</param>
        /// <returns>Czy wynik jest najwyższym wynikiem do tej pory (TOP 1)</returns>
        public Boolean SaveScore(Int64 score)
        {
            Boolean isHighscore = false;
            AppMemory am = new AppMemory();

            for (int i = 0; i < _amountOfPlaces; i++)
            {
                if (score > TopTen[i].Value)
                {
                    TopTen.Insert(i, new KeyValuePair<string, Int64>(_playerName, score));

                    if (i == 0)
                    {
                        //TODO: highscore 
                        isHighscore = true;
                    }

                    break;
                }
            }

            for (int i = 0; i < _amountOfPlaces; i++)
            {
                am.SaveValue("score" + Convert.ToString(i + 1), TopTen[i].Value);
                am.SaveValue("playerName" + Convert.ToString(i + 1), TopTen[i].Key);
            }

            return isHighscore;
        }
    }


}
