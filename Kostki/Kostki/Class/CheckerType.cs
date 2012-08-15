

namespace Kostki.Class
{
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

    public class CheckerType
    {
        public int x;
        public int y;
        public FourcardType fourcardtype;

        public CheckerType(int x, int y, FourcardType fourcardtype)
        {
            this.x = x;
            this.y = y;
            this.fourcardtype = fourcardtype;
        }
    }
}
