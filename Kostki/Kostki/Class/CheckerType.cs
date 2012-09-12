

using System;

namespace Kostki.Class
{
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

        public new string ToString()
        {
            return Convert.ToString(x) + " " + Convert.ToString(y) + " " + fourcardtype.ToString();
        }
    }
}
