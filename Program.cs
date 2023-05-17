﻿using System;
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

            ///New Refactored/Async way
            ///

            Task writeWood1Task = w1.AsyncWriteWoodToDB();
            Task writeWood2Task = w2.AsyncWriteWoodToDB();
            Task writeWood3Task = w3.AsyncWriteWoodToDB();

            await Task.WhenAll(writeWood1Task, writeWood2Task, writeWood3Task);

            Task escapeWood1 = w1.AsyncEscape();
            Task escapeWood2 = w2.AsyncEscape();
            Task escapeWood3 = w3.AsyncEscape();

            await Task.WhenAll(escapeWood1, escapeWood2, escapeWood3);

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
