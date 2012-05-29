using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
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

            return null;
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

                    // zapisanie planszy
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            sw.WriteLine(Write(gameBoard[(int) PlaceType.Grid, j, i]));
                        }
                    }

                   
                    // zapisanie randa
                    for (int i = 0; i < 4; i++)
                    {
                        sw.WriteLine(Write(gameBoard[(int) PlaceType.Rand, i, 0]));
                    }

                    // zapisanie jokerów
                    sw.WriteLine(gameBoard[(int)PlaceType.Joker, 0, 0]);
                    sw.WriteLine(gameBoard[(int)PlaceType.Joker, 1, 0]);

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
