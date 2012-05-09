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

        public Boolean Blocked
        {
            get { return blocked; }
            set { blocked = value; }
        }

        public Image Image
        {
            get { return image; }
            set { image = value; }
        }

        public Id(Figures figure, CardColors color)
        {
            this.image = null;
            this.color = color;
            this.figure = figure;
            this.blocked = false;
        }
    }
}
