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
        public Id(Figures figure, CardColors color)
        {
            this.color = color;
            this.figure = figure;
        }
    }
}
