using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using Kostki.Class;

namespace Kostki
{
    public class AppMemory
    {
        private const string folderName = "GameState";
        private const string fileName = "GameBoard";

        public string PlayerName
        {
            get { return GetValueOrDefault<string>("playerName", "Player"); }
            set { SaveValue("playerName", value); }
        }

        public uint HighScore
        {
            get { return GetValueOrDefault<uint>("highScore", 0); }
            set { SaveValue("highScore", value); }
        }

        public Id[,,] LoadGameState()
        {
            Debug.WriteLine("WCZYTUJE:");
            Id[,,] gameBoard = new Id[4, 4, 4];

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string filePath = System.IO.Path.Combine(folderName, fileName);

                if (isf.FileExists(filePath))
                {
                    try
                    {
                        using (IsolatedStorageFileStream fileStream = isf.OpenFile(filePath, System.IO.FileMode.Open))
                        {
                            StreamReader reader = new StreamReader(fileStream);
                            
                            // wczytanie grida
                            for (int i = 0; i < 4; i++)
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    string temp = reader.ReadLine();

                                    gameBoard[(int) PlaceType.Grid, j, i] = Read(temp);
                                }
                            }

                            // rand
                            for (int i = 0; i < 4; i++)
                            {
                                string temp = reader.ReadLine();
                                gameBoard[(int) PlaceType.Rand, i, 0] = Read(temp);
                            }

                            // joker
                            gameBoard[(int)PlaceType.Joker, 0, 0] = Read(reader.ReadLine());
                            gameBoard[(int)PlaceType.Joker, 1, 0] = Read(reader.ReadLine());
                            
                            reader.Close();
                        }
                    }
                    catch (Exception)
                    {
                        
                        throw;
                    }
                }
            }

            return null;
        }

        private Id Read(string s)
        {
            if (s.Length > 0)
            {
                string[] prop = s.Split(' ');

                Figures figure = (Figures) Enum.Parse(typeof (Figures), prop[0], true);
                CardColors color = (CardColors) Enum.Parse(typeof (CardColors), prop[1], true);

                Id id = new Id(figure, color);

                id.IsJoker = bool.Parse(prop[2]);
                id.Blocked = bool.Parse(prop[3]);
            }

            
            return null;
        }

        public void SaveGameState(Id[,,] gameBoard)
        {
            StringBuilder result = new StringBuilder();

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isf.DirectoryExists(folderName))
                    isf.CreateDirectory(folderName);

                string filePath = System.IO.Path.Combine(folderName, fileName);
                isf.DeleteFile(filePath);       // usuinięcie/wyczyszczenie pliku przed zapisem

                using (IsolatedStorageFileStream rawStream = isf.CreateFile(filePath))
                {
                    StreamWriter sw = new StreamWriter(rawStream);

                    // zapisanie planszy
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            //sw.WriteLine(Write(gameBoard[(int) PlaceType.Grid, j, i]));
                            result.AppendLine(Write(gameBoard[(int) PlaceType.Grid, j, i]));
                        }
                    }

                   
                    // zapisanie randa
                    for (int i = 0; i < 4; i++)
                    {
                        //sw.WriteLine(Write(gameBoard[(int) PlaceType.Rand, i, 0]));
                        result.AppendLine(Write(gameBoard[(int) PlaceType.Rand, i, 0]));
                    }

                    // zapisanie jokerów
                    //sw.WriteLine(gameBoard[(int)PlaceType.Joker, 0, 0]);
                    //sw.WriteLine(gameBoard[(int)PlaceType.Joker, 1, 0]);
                    result.AppendLine(Write(gameBoard[(int) PlaceType.Joker, 0, 0]));
                    result.AppendLine(Write(gameBoard[(int) PlaceType.Joker, 0, 0]));

                    sw.Write(result.ToString());
                    sw.Close();
                }
            }
        }

        private string Write(Id id)
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
