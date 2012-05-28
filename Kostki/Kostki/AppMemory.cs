using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using Kostki.Class;

namespace Kostki
{
    public class AppMemory
    {
        private const string folderName = "GameState";
        private const string fileName = "GameBoard";

        public string playerName
        {
            get { return GetValueOrDefault<string>("playerName", "Player"); }
            set { SaveValue("playerName", value); }
        }

        public uint highScore
        {
            get { return GetValueOrDefault<uint>("highScore", 0); }
            set { SaveValue("highScore", value); }
        }

        public void SaveGameState(Id[,,] gameBoard)
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isf.DirectoryExists(folderName))
                    isf.CreateDirectory(folderName);

                string filePath = System.IO.Path.Combine(folderName, fileName);

                using (IsolatedStorageFileStream rawStream = isf.CreateFile(filePath))
                {
                    StreamWriter sw = new StreamWriter(rawStream);


                    Debug.WriteLine("PLANSZA: ");

                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            Debug.WriteLine(write(gameBoard[(int) PlaceType.Grid, i, j]));
                            // Debug.WriteLine(gameBoard[(int) PlaceType.Grid, i, j].ToString());
                        }
                    }

                    Debug.WriteLine("RAND: ");

                    for (int i = 0; i < 4; i++)
                    {
                        Debug.WriteLine(write(gameBoard[(int) PlaceType.Rand, i, 0]));
                        // Debug.WriteLine(gameBoard[(int) PlaceType.Rand, i, 0]);
                    }

                    Debug.WriteLine("JOKER: ");
                  //  Debug.WriteLine();
                 // Debug.WriteLine(gameBoard[(int) PlaceType.Joker, i, 0]);
                    

                }
            }
        }

        private string write(Id id)
        {
            if (id != null)
                return id.ToString();
            else
                return "";
        }

        public T GetValueOrDefault<T>(string key, T defaultValue)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
                return (T) IsolatedStorageSettings.ApplicationSettings[key];
            else
                return defaultValue;
        }

        private void SaveValue(string key, object value)
        {
            IsolatedStorageSettings.ApplicationSettings[key] = value;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }
    }
}
