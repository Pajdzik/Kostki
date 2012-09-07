using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using Kostki.Class;

namespace Kostki
{
    public class AppMemory
    {
        private const string FolderName = "GameState";
        private const string FileName = "GameBoard";

        public string PlayerName
        {
            get { return this.GetValueOrDefault<string>("playerName", "Player"); }
            set { this.SaveValue("playerName", value); }
        }

        public uint HighScore
        {
            get { return this.GetValueOrDefault<uint>("highScore", 0); }
            set { this.SaveValue("highScore", value); }
        }

        public Id[, ,] LoadGameState()
        {
            Id[, ,] gameBoard = new Id[4, 4, 4];

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string filePath = System.IO.Path.Combine(FolderName, FileName);

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

                                    gameBoard[(int)PlaceType.Grid, j, i] = this.Read(temp);
                                }
                            }

                            // rand
                            for (int i = 0; i < 4; i++)
                            {
                                string temp = reader.ReadLine();
                                gameBoard[(int)PlaceType.Rand, i, 0] = this.Read(temp);
                            }

                            // joker
                            gameBoard[(int)PlaceType.Joker, 0, 0] = this.Read(reader.ReadLine());
                            gameBoard[(int)PlaceType.Joker, 1, 0] = this.Read(reader.ReadLine());

                            reader.Close();
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }

            return gameBoard;
        }

        private Id Read(string s)
        {
            if (s.Length > 0)
            {
                string[] prop = s.Split(' ');

                Figures figure = (Figures)Enum.Parse(typeof(Figures), prop[0], true);
                CardColors color = (CardColors)Enum.Parse(typeof(CardColors), prop[1], true);

                Id id = new Id(figure, color);

                id.IsJoker = bool.Parse(prop[2]);
                id.Blocked = bool.Parse(prop[3]);
                return id;
            }

            return null;
        }

        public void SaveGameState(Id[, ,] gameBoard)
        {
            StringBuilder result = new StringBuilder();

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isf.DirectoryExists(FolderName))
                    isf.CreateDirectory(FolderName);

                string filePath = System.IO.Path.Combine(FolderName, FileName);
                isf.DeleteFile(filePath);       // usuinięcie/wyczyszczenie pliku przed zapisem

                using (IsolatedStorageFileStream rawStream = isf.CreateFile(filePath))
                {
                    StreamWriter sw = new StreamWriter(rawStream);

                    // zapisanie planszy
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            result.AppendLine(this.Write(gameBoard[(int)PlaceType.Grid, j, i]));
                        }
                    }


                    // zapisanie randa
                    for (int i = 0; i < 4; i++)
                    {
                        result.AppendLine(this.Write(gameBoard[(int)PlaceType.Rand, i, 0]));
                    }

                    // zapisanie jokerów
                    // sw.WriteLine(gameBoard[(int)PlaceType.Joker, 0, 0]);
                    // sw.WriteLine(gameBoard[(int)PlaceType.Joker, 1, 0]);
                    result.AppendLine(this.Write(gameBoard[(int)PlaceType.Joker, 0, 0]));
                    result.AppendLine(this.Write(gameBoard[(int)PlaceType.Joker, 0, 0]));

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
                return string.Empty;
        }

        public T GetValueOrDefault<T>(string key, T defaultValue)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
                return (T)IsolatedStorageSettings.ApplicationSettings[key];
            else
                return defaultValue;
        }

        public void SaveValue(string key, object value)
        {
            IsolatedStorageSettings.ApplicationSettings[key] = value;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }
    }
}
