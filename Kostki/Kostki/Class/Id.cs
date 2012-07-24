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

namespace Kostki.Class
{
    public class Id
    {
        private Figures figure;
        private CardColors color;
        private Boolean blocked;
        private Image image;
        private Boolean isJoker;

        public Boolean IsJoker
        {
            get { return isJoker; }
            set { isJoker = value; }
        }

        public Boolean Blocked
        {
            get { return blocked; }
            set { blocked = value; }
        }

        public Figures Figure
        {
            get { return figure; }
            set { figure = value; }
        }

        public CardColors Color
        {
            get { return color; }
            set { color = value; }
        }

        public Image Image
        {
            get { return image; }
            set { image = value; }
        }

        public Id(Figures figure, CardColors color)
        {
            this.isJoker = false;
            this.image = null;
            this.color = color;
            this.figure = figure;
            this.blocked = false;
        }

        public override string ToString()
        {
            return figure.ToString() + ' ' + color.ToString() + ' ' + isJoker.ToString() + ' ' + Blocked.ToString();
        }
    }
}
