using LiteDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Meres_DLL;

namespace Beadando
{
    internal class Program
    {
        static List<Szenzor> értékek = new List<Szenzor>();

        static void Main(string[] args)
        {
            Console.Write("Szeretne amerikai mértékegységeket latni? (i/n): ");
            string valasz = Console.ReadLine().ToLower();
            if (valasz == "i")
            { Delegált.amerikai = true; }



            Random r = new Random();

            Console.Write("Mennyi adatot szeretne leolvasi? ");
            int n = Convert.ToInt32(Console.ReadLine());
            

            for (int i = 0; i < n; i++)
            {
                Szenzor x = new Szenzor();
                double homerseklet = r.Next(20, 101) + r.NextDouble();
                double paratartalom = r.Next(1,91) + r.NextDouble();
                double vizszint = r.Next(0, 101) + r.NextDouble();
                double folyoszint = r.Next(2,11) + r.NextDouble(); 
                x.Homerseklet = homerseklet;
                x.Paratartalom = paratartalom;
                x.Vizszint = vizszint;
                x.Folyoszint = folyoszint;
                értékek.Add(x);
            }//értékadás

            if (Delegált.amerikai == true)
            {
                Delegált.Amerikai ho = Delegált.CelsiusToFahrenheit;
                Delegált.Amerikai viz = Delegált.CmToInch;
                Delegált.Amerikai folyo = Delegált.MeterToInch;

                foreach (var s in értékek)
                {
                    Delegált.Atvalt(s, ho, viz, folyo);
                }
            }//delegate 
            else
            {
                for (int i = 0; i < n; i++) //kiiratás
                    {
                        Console.WriteLine(értékek[i].ToString());
                    }
            }

            string bemenet = "";
            do
            {
                using (var db = new LiteDatabase("Meres.db"))
                {
                    var meresek = db.GetCollection<Szenzor>("meresek");
                    Console.Write("Mit szeretne kezdeni az adatbázissal? mentés/frissítés/törlés/kiírás; befejezés => vége: ");
                    bemenet = Console.ReadLine().ToLower();

                    switch (bemenet)
                    {
                        case "mentés":
                            foreach (var item in értékek)
                            {
                                var person = new Szenzor
                                {
                                    Homerseklet = item.Homerseklet,
                                    Paratartalom = item.Paratartalom,
                                    Vizszint = item.Vizszint,
                                    Folyoszint = item.Folyoszint
                                };
                                meresek.Insert(person);
                            }
                            break;

                        case "kiírás":
                            var query = meresek.FindAll();
                            foreach (var item in query)
                            {
                                Console.WriteLine(item.ID + "\n" + item.Homerseklet + "\n" + item.Paratartalom + "\n" + item.Vizszint + "\n" + item.Folyoszint);
                            }
                            Console.ReadKey();
                            break;
                    }
                }
            }
            while (bemenet != "vége");

            Json.Jsonki(értékek);
            Console.ReadKey();
        }//Main

    }//Program
}//Namespace
