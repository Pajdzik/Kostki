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
using System.IO.IsolatedStorage;

namespace Kostki
{
    public class AppMemory
    {
        private const string FolderName = "Settings";

        public string PlayerName
        {
            get { return GetValueOrDefault<string>("PlayerName", "Player"); }
            set { SaveValue("PlayerName", value); }
        }

        public uint HighScore
        {
            get { return GetValueOrDefault<uint>("HighScore", 0); }
            set { SaveValue("HighScore", value); }
        }

        public void SaveGameState(void)
        {
            /*
             * Tu bendziem sejwować. Ale nie wiem jak wyciągnąć tablicę. 
             */
        }

        public T GetValueOrDefault<T>(string key, T defaultValue)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                return (T)IsolatedStorageSettings.ApplicationSettings[key];
            }
            else
            {
                return defaultValue;
            }
        }

        private void SaveValue(string key, object value)
        {
            IsolatedStorageSettings.ApplicationSettings[key] = value;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }
    }
}
