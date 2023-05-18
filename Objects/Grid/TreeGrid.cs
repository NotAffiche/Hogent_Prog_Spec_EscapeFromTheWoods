using System;
using System.Collections.Generic;
using System.Text;

namespace EscapeFromTheWoods.Objects.Grid
{
    public class TreeGrid
    {
        public int Delta { get; set; }
        public XYBoundary XYBoundary { get; set; }
        public int NX { get; set; }
        public int NY { get; set; }
        public List<Tree>[][] Trees { get; set; }

        public TreeGrid(int delta, XYBoundary xyb)
        {
            Delta = delta;
            XYBoundary = xyb;
            NX = (int)((XYBoundary.DX / delta) + 1);
            NY = (int)((XYBoundary.DY / delta) + 1);
            Trees = new List<Tree>[NX][];
            for (int i = 0; i<NX; i++)
            {
                Trees[i] = new List<Tree>[NY];
                for (int j = 0; j<NY; j++)
                {
                    Trees[i][j] = new List<Tree>();
                }
            }
        }

        public TreeGrid(int delta, XYBoundary xyb, List<Tree> data) : this(delta, xyb)
        {
            foreach (Tree t in data)
            {
                AddTree(t);
            }
        }

        public void AddTree(Tree tree)
        {
            if ((tree.x < XYBoundary.MinX) || (tree.y < XYBoundary.MinY) ||
                (tree.x > XYBoundary.MaxX) || (tree.y > XYBoundary.MaxY)) throw new ArgumentOutOfRangeException("out of bounds");
            int i = (int)((tree.x - XYBoundary.MinX) / Delta);
            int j = (int)((tree.y - XYBoundary.MinY) / Delta);
            if (i == NX) i--;
            if (j == NY) j--;
            Trees[i][j].Add(tree);
        }
    }
}
