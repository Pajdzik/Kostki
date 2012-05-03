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
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace Kostki.Class
{
    public class CardImage : FrameworkElement
    {
        private int figure;
        private int cardcolor;
        public Image image;

        public CardImage(int figure, int cardColor) : base()
        {
            this.figure = figure;
            this.cardcolor = cardColor;
        }

        public void SetImage(Image image) 
        {
            this.image = image;
        }

    }
}
