using System;
using System.Collections.Generic;
using System.Text;

namespace EscapeFromTheWoods.Objects.Grid
{
    public class XYBoundary
    {
        public XYBoundary(int minX, int minY, int maxX, int maxY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }

        public int MinX { get; set; }
        public int MinY { get; set; }
        public int MaxX { get; set; }
        public int MaxY { get; set; }
        public int DX { get => MaxX- MinX; }
        public int DY { get => MaxY- MinY; }
        public bool WithinBounds(int x, int y)
        {
            if ((x<MinX) || (x>MaxX) || (y<MinY) || (y>MaxY)) return false;
            return true;
        }
    }
}
