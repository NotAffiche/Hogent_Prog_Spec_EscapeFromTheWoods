using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading.Tasks;
using System.ComponentModel;
using EscapeFromTheWoods.Objects.Grid;

namespace EscapeFromTheWoods
{
    public class Wood
    {
        private const int drawingFactor = 8;
        private string path;
        private DBwriter db;
        private Random r = new Random(1);
        public int woodID { get; set; }
        public List<Tree> trees { get; set; }
        public List<Monkey> monkeys { get; private set; }
        private Map map;
        public Wood(int woodID, List<Tree> trees, Map map, string path, DBwriter db)
        {
            this.woodID = woodID;
            this.trees = trees;
            this.monkeys = new List<Monkey>();
            this.map = map;
            this.path = path;
            this.db = db;
        }
        public void PlaceMonkey(string monkeyName, int monkeyID)
        {
            int treeNr;
            do
            {
                treeNr = r.Next(0, trees.Count - 1);
            }
            while (trees[treeNr].hasMonkey);
            Monkey m = new Monkey(monkeyID, monkeyName, trees[treeNr]);
            monkeys.Add(m);
            trees[treeNr].hasMonkey = true;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{m.name} placed on x: {trees[treeNr].x}, y: {trees[treeNr].y}");
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void Escape()
        {
            List<List<Tree>> routes = new List<List<Tree>>();
            foreach (Monkey m in monkeys)
            {
                routes.Add(EscapeMonkey(m));
            }                
           WriteEscaperoutesToBitmap(routes);           
        }
        private void writeRouteToDB(Monkey monkey,List<Tree> route)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{woodID}:write db routes {woodID},{monkey.name} start");
            List<DBMonkeyRecord> records = new List<DBMonkeyRecord>();
            for (int j = 0; j < route.Count; j++)
            {
                records.Add(new DBMonkeyRecord(monkey.monkeyID, monkey.name, woodID,j, route[j].treeID, route[j].x, route[j].y));
            }
            db.WriteMonkeyRecords(records);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{woodID}:write db routes {woodID},{monkey.name} end");
        }       
        public void WriteEscaperoutesToBitmap(List<List<Tree>> routes)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{woodID}:write bitmap routes {woodID} start");
            Color[] cvalues = new Color[] { Color.Red, Color.Yellow, Color.Blue, Color.Cyan, Color.GreenYellow };
            Bitmap bm = new Bitmap((map.xmax - map.xmin) * drawingFactor, (map.ymax - map.ymin) * drawingFactor);
            Graphics g = Graphics.FromImage(bm);
            int delta = drawingFactor / 2;
            Pen p = new Pen(Color.Green, 1);
            foreach (Tree t in trees)
            {
                g.DrawEllipse(p, t.x * drawingFactor, t.y * drawingFactor, drawingFactor, drawingFactor);
            }
            int colorN = 0;
            foreach (List<Tree> route in routes)
            {
                int p1x = route[0].x * drawingFactor + delta;
                int p1y = route[0].y * drawingFactor + delta;
                Color color = cvalues[colorN % cvalues.Length];
                Pen pen = new Pen(color, 1);
                g.DrawEllipse(pen, p1x - delta, p1y - delta, drawingFactor, drawingFactor);
                g.FillEllipse(new SolidBrush(color), p1x - delta, p1y - delta, drawingFactor, drawingFactor);
                for (int i = 1; i < route.Count; i++)
                {
                    g.DrawLine(pen, p1x, p1y, route[i].x * drawingFactor + delta, route[i].y * drawingFactor + delta);
                    p1x = route[i].x * drawingFactor + delta;
                    p1y = route[i].y * drawingFactor + delta;
                }
                colorN++;
            }
            bm.Save(Path.Combine(path, woodID.ToString() + "_escapeRoutes.jpg"), ImageFormat.Jpeg);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{woodID}:write bitmap routes {woodID} end");
        }
        public void WriteWoodToDB()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{woodID}:write db wood {woodID} start");
            List<DBWoodRecord> records = new List<DBWoodRecord>();
            foreach(Tree t in trees)
            {
                records.Add(new DBWoodRecord(woodID, t.treeID,t.x,t.y));
            }
            db.WriteWoodRecords(records);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{woodID}:write db wood {woodID} end");
        }
        public List<Tree> EscapeMonkey(Monkey monkey)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{woodID}:start {woodID},{monkey.name}");
            Dictionary<int, bool> visited = new Dictionary<int, bool>();
            trees.ForEach(x => visited.Add(x.treeID, false));
            List<Tree> route = new List<Tree>() { monkey.tree };
            do
            {
                visited[monkey.tree.treeID] = true;
                SortedList<double, List<Tree>> distanceToMonkey = new SortedList<double, List<Tree>>();

                //zoek dichtste boom die nog niet is bezocht            
                foreach (Tree t in trees)
                {
                    if ((!visited[t.treeID]) && (!t.hasMonkey))
                    {
                        double d = Math.Sqrt(Math.Pow(t.x - monkey.tree.x, 2) + Math.Pow(t.y - monkey.tree.y, 2));
                        if (distanceToMonkey.ContainsKey(d)) distanceToMonkey[d].Add(t);
                        else distanceToMonkey.Add(d, new List<Tree>() { t });
                    }
                }
                //distance to border            
                //noord oost zuid west
                double distanceToBorder = (new List<double>(){ map.ymax - monkey.tree.y,
                map.xmax - monkey.tree.x,monkey.tree.y-map.ymin,monkey.tree.x-map.xmin }).Min();
                if (distanceToMonkey.Count == 0)
                {
                    writeRouteToDB(monkey, route);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{woodID}:end {woodID},{monkey.name}");
                    return route;
                }
                if (distanceToBorder < distanceToMonkey.First().Key)
                {
                    writeRouteToDB(monkey, route);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{woodID}:end {woodID},{monkey.name}");
                    return route;
                }

                route.Add(distanceToMonkey.First().Value.First());
                monkey.tree = distanceToMonkey.First().Value.First();
            }
            while (true);
        }


        //async
        public async Task AsyncEscape(Map map)
        {
            List<List<Tree>> routes = new List<List<Tree>>();
            foreach (Monkey m in monkeys)
            {
                routes.Add(await AsyncEscapeMonkey(m, map));
            }
            AsyncWriteEscaperoutesToBitmap(routes);
        }
        private async Task AsyncWriteRouteToDB(Monkey monkey, List<Tree> route)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{woodID}:write db routes {woodID},{monkey.name} start");
            List<DBMonkeyRecord> records = new List<DBMonkeyRecord>();
            for (int j = 0; j < route.Count; j++)
            {
                records.Add(new DBMonkeyRecord(monkey.monkeyID, monkey.name, woodID, j, route[j].treeID, route[j].x, route[j].y));
            }
            //await db.AsyncWriteMonkeyRecordsMSSQL(records);//old sql serv
            await db.AsyncWriteMonkeyRecordsMongoDB(records);//new mongodb
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{woodID}:write db routes {woodID},{monkey.name} end");
        }
        public void AsyncWriteEscaperoutesToBitmap(List<List<Tree>> routes)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{woodID}:write bitmap routes {woodID} start");
            Color[] cvalues = new Color[] { Color.Red, Color.Yellow, Color.Blue, Color.Cyan, Color.GreenYellow };
            Bitmap bm = new Bitmap((map.xmax - map.xmin) * drawingFactor, (map.ymax - map.ymin) * drawingFactor);
            Graphics g = Graphics.FromImage(bm);
            int delta = drawingFactor / 2;
            Pen p = new Pen(Color.Green, 1);
            foreach (Tree t in trees)
            {
                g.DrawEllipse(p, t.x * drawingFactor, t.y * drawingFactor, drawingFactor, drawingFactor);
            }
            int colorN = 0;
            foreach (List<Tree> route in routes)
            {
                int p1x = route[0].x * drawingFactor + delta;
                int p1y = route[0].y * drawingFactor + delta;
                Color color = cvalues[colorN % cvalues.Length];
                Pen pen = new Pen(color, 1);
                g.DrawEllipse(pen, p1x - delta, p1y - delta, drawingFactor, drawingFactor);
                g.FillEllipse(new SolidBrush(color), p1x - delta, p1y - delta, drawingFactor, drawingFactor);
                for (int i = 1; i < route.Count; i++)
                {
                    g.DrawLine(pen, p1x, p1y, route[i].x * drawingFactor + delta, route[i].y * drawingFactor + delta);
                    p1x = route[i].x * drawingFactor + delta;
                    p1y = route[i].y * drawingFactor + delta;
                }
                colorN++;
            }
            bm.Save(Path.Combine(path, woodID.ToString() + "_escapeRoutes.jpg"), ImageFormat.Jpeg);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{woodID}:write bitmap routes {woodID} end");
        }
        public async Task AsyncWriteWoodToDB()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{woodID}:write db wood {woodID} start");
            List<DBWoodRecord> records = new List<DBWoodRecord>();
            foreach (Tree t in trees)
            {
                records.Add(new DBWoodRecord(woodID, t.treeID, t.x, t.y));
            }
            //await db.AsyncWriteWoodRecordsMSSQL(records);//old sql serv
            await db.AsyncWriteWoodRecordsMongoDB(records);//new mongodb
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{woodID}:write db wood {woodID} end");
        }
        public async Task<List<Tree>> AsyncEscapeMonkey(Monkey monkey, Map map)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{woodID}:start {woodID},{monkey.name}");
            HashSet<int> visited = new HashSet<int>();
            List<Tree> route = new List<Tree>() { monkey.tree };
            TreeGrid treeGrid = new TreeGrid(5, new XYBoundary(map.xmin, map.ymin, map.xmax, map.ymax), trees);
            int n = 25;
            do
            {
                visited.Add(monkey.tree.treeID);
                SortedList<double, List<Tree>> distanceToMonkey = new SortedList<double, List<Tree>>();

                //zoek dichtste boom die nog niet is bezocht
                // refactor: loop door dichtsbijzijnde bomen op basis van grid

                #region test
                (int i, int j) = FindCell(monkey.tree.x, monkey.tree.y, treeGrid);
                ProcessCell(distanceToMonkey, treeGrid, i, j, monkey, n, visited);
                int ring = 0;
                while (distanceToMonkey.Count < n)
                {
                    ring++;
                    ProcessRing(i, j, ring, distanceToMonkey, monkey, n, treeGrid, visited);
                }
                ProcessRing(i, j, ring + 1, distanceToMonkey, monkey, n, treeGrid, visited);
                #endregion

                //distance to border            
                //noord oost zuid west
                double distanceToBorder = (new List<double>(){ map.ymax - monkey.tree.y,
                map.xmax - monkey.tree.x,monkey.tree.y-map.ymin,monkey.tree.x-map.xmin }).Min();
                if ((distanceToMonkey.Count == 0) || (distanceToBorder < distanceToMonkey.First().Key))
                {
                    await AsyncWriteRouteToDB(monkey, route);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{woodID}:end {woodID},{monkey.name}");
                    return route;
                }

                route.Add(distanceToMonkey.First().Value.First());
                monkey.tree = distanceToMonkey.First().Value.First();
            }
            while (true);
        }

        #region grid methods
        private (int, int) FindCell(int x, int y, TreeGrid tg)
        {
            if (!tg.XYBoundary.WithinBounds(x, y)) throw new ArgumentOutOfRangeException("out of bounds");
            int i = (int)((x-tg.XYBoundary.MinX) / tg.Delta);
            int j = (int)((y-tg.XYBoundary.MinY) / tg.Delta);
            if (i == tg.NX) i--;
            if (j == tg.NY) j--;
            return (i, j);
        }
        private void ProcessCell(SortedList<double, List<Tree>> dtm, TreeGrid tg, int i, int j, Monkey m, int n, HashSet<int> v)
        {
            foreach (Tree t in tg.Trees[i][j])
            {
                if (!v.Contains(t.treeID) && (!t.hasMonkey))
                {
                    double d = Math.Sqrt(Math.Pow(t.x - m.tree.x, 2) + Math.Pow(t.y - m.tree.y, 2));
                    if ((dtm.Count < n) || (d < dtm.Keys[dtm.Count - 1]))
                    {
                        if (dtm.ContainsKey(d)) dtm[d].Add(t);
                        else { dtm.Add(d, new List<Tree>() { t }); }
                    }
                }
            }
        }
        private bool IsValidCell(int i, int j, TreeGrid tg)
        {
            if ((j < 0) || (j >= tg.NY)) return false;
            if ((i < 0) || (i >= tg.NX)) return false;
            return true;

        }
        private void ProcessRing(int i, int j, int ring, SortedList<double, List<Tree>> dtm, Monkey m, int n, TreeGrid tg, HashSet<int> v)
        {
            for (int gx=i-ring;gx<=i+ring;gx++)
            {
                int gy = j - ring;
                if (IsValidCell(gx, gy, tg)) ProcessCell(dtm, tg, gx, gy, m, n, v);
                gy = j + ring;
                if (IsValidCell(gx, gy, tg)) ProcessCell(dtm, tg, gx, gy, m, n, v);
            }
            for (int gy = j - ring; gy <= j + ring - 1; gy++)
            {
                int gx = i - ring;
                if (IsValidCell(gx, gy, tg)) ProcessCell(dtm, tg, gx, gy, m, n, v);
                gx = i + ring;
                if (IsValidCell(gx, gy, tg)) ProcessCell(dtm, tg, gx, gy, m, n, v);
            }
        }
        #endregion
        //
    }
}
