using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace EscapeFromTheWoods
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
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
            //Time elapsed: 00:00:02.1623876 | 1272 | 5000

            ///New Refactored/Async way
            ///

            #region t1/Refactored

            Task writeWood1Task = w1.AsyncWriteWoodToDB();
            Task writeWood2Task = w2.AsyncWriteWoodToDB();
            Task writeWood3Task = w3.AsyncWriteWoodToDB();

            await Task.WhenAll(writeWood1Task, writeWood2Task, writeWood3Task);

            Task escapeWood1 = w1.AsyncEscape(m1);
            Task escapeWood2 = w2.AsyncEscape(m2);
            Task escapeWood3 = w3.AsyncEscape(m3);

            await Task.WhenAll(escapeWood1, escapeWood2, escapeWood3);
            //Time elapsed: 00:00:01.2334299 remove from tree grid
            //Time elapsed: 00:00:01.0762059 | 1350 | 5000 use hashset visisted

            #endregion

            #region test2
            //w2.WriteWoodToDB();
            //w2.Escape();

            //Andere route dan non refactored want: 1) apen springen tegelijkertijd dus lege bomen ipv een boom met een aap erin die wacht om te springen
            //en b) zoals hieronder (2 punten met zelfde afstand, kiest bij mij telkens het andere punt dan in non async/refactored, is wel consequent er in)
            //m.a.w. mijn apen hebben pech (kiezen de langere route aangezien dat in totaal 1350 records van sprongen zijn ipv 1272) (ze zijn ook ongeduldiger
            //want ze wachten niet tot als de vorige aap klaar is met springen, beginnen allemaal tegelijk ;) )

            //double dOG = Math.Sqrt(Math.Pow(67 - 64, 2) + Math.Pow(330 - 328, 2));
            //double dREF = Math.Sqrt(Math.Pow(61 - 64, 2) + Math.Pow(326 - 328, 2));
            //Console.WriteLine($"dog: {dOG} | dref: {dREF}");

            //Task writeWood2Task = w2.AsyncWriteWoodToDB();
            //await Task.WhenAll(writeWood2Task);
            //Task escapeWood2 = w2.AsyncEscape(m2);
            //await Task.WhenAll(escapeWood2);
            #endregion

            ///
            ///End Refactored/Async way


            stopwatch.Stop();
            // Write result.
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("end");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
