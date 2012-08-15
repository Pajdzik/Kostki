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
            get { return this.isJoker; }
            set { this.isJoker = value; }
        }

        public Boolean Blocked
        {
            get { return this.blocked; }
            set { this.blocked = value; }
        }

        public Figures Figure
        {
            get { return this.figure; }
            set { this.figure = value; }
        }

        public CardColors Color
        {
            get { return this.color; }
            set { this.color = value; }
        }

        public Image Image
        {
            get { return this.image; }
            set { this.image = value; }
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
            return this.figure.ToString() + ' ' + this.color.ToString() + ' ' + this.isJoker.ToString() + ' ' + this.Blocked.ToString();
        }
    }
}
