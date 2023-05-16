using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace EscapeFromTheWoods
{
    class Program
    {
        static void Main(string[] args)
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            Console.WriteLine("Hello World!");
            string connectionString = @"Data Source=.\SQLExpress;Initial Catalog=EscapeFromTheWoods;Integrated Security=True";
            DBwriter db = new DBwriter(connectionString);

            string path = @"C:\NET\monkeys";
            Map m1 = new Map(0, 500, 0, 500);
            Wood w1 = WoodBuilder.GetWood(500, m1, path,db);
            w1.PlaceMonkey("Alice", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Janice", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Toby", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Mindy", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Jos", IDgenerator.GetMonkeyID());
            
            Map m2 = new Map(0, 200, 0, 400);
            Wood w2 = WoodBuilder.GetWood(2500, m2, path,db);
            w2.PlaceMonkey("Tom", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Jerry", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Tiffany", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Mozes", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Jebus", IDgenerator.GetMonkeyID());

            Map m3 = new Map(0, 400, 0, 400);
            Wood w3 = WoodBuilder.GetWood(2000, m3, path,db);
            w3.PlaceMonkey("Kelly", IDgenerator.GetMonkeyID());
            w3.PlaceMonkey("Kenji", IDgenerator.GetMonkeyID());
            w3.PlaceMonkey("Kobe", IDgenerator.GetMonkeyID());
            w3.PlaceMonkey("Kendra", IDgenerator.GetMonkeyID());

            //w1.WriteWoodToDB();
            //w2.WriteWoodToDB();
            //w3.WriteWoodToDB();
            //w1.Escape();
            //w2.Escape();
            //w3.Escape();
            Thread tOld = new Thread(() =>
            {
                Stopwatch stopwatchOld = new Stopwatch();
                Console.WriteLine("Old way:");
                stopwatchOld.Start();
                w1.WriteWoodToDB();
                w2.WriteWoodToDB();
                w3.WriteWoodToDB();
                w1.Escape();
                w2.Escape();
                w3.Escape();
                stopwatchOld.Stop();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Time elapsed OLD: {0}", stopwatchOld.Elapsed);
                Console.ForegroundColor = ConsoleColor.White;
            });
            tOld.Start();
            Thread tNewWay = new Thread(() =>
            {
                Stopwatch stopwatchRefactored = new Stopwatch();
                Console.WriteLine("Refactored:");
                stopwatchRefactored.Start();
                List<Task> tasks = new List<Task>();
                tasks.Add(Task.Run(() => w1.AsyncWriteWoodToDB()));
                tasks.Add(Task.Run(() => w2.AsyncWriteWoodToDB()));
                tasks.Add(Task.Run(() => w3.AsyncWriteWoodToDB()));
                tasks.Add(Task.Run(() => w1.AsyncEscape()));
                tasks.Add(Task.Run(() => w2.AsyncEscape()));
                tasks.Add(Task.Run(() => w3.AsyncEscape()));
                Task.WaitAll(tasks.ToArray());
                stopwatchRefactored.Stop();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Time elapsed REFACTORED: {0}", stopwatchRefactored.Elapsed);
                Console.ForegroundColor = ConsoleColor.White;
            });
            tNewWay.Start();

            //stopwatch.Stop();
            //// Write result.
            //Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
            Console.WriteLine("end");
        }
    }
}
