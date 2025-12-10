using LiteDB;
using Meres_DLL;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Beadando
{
    internal class Program
    {
        static List<Szenzor> értékek = new List<Szenzor>();
        static void Szenzor_Riasztas(object sender, string uzenet)  
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(uzenet);
            Console.ResetColor();
        }

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
                x.Riasztas += Szenzor_Riasztas;
                double homerseklet = r.Next(20, 101) + r.NextDouble();
                double paratartalom = r.Next(1,91) + r.NextDouble();
                double vizszint = r.Next(0, 101) + r.NextDouble();
                double folyoszint = r.Next(2,11) + r.NextDouble(); 
                x.Homerseklet = Math.Round(homerseklet, 3);
                x.Paratartalom = Math.Round(paratartalom, 3);
                x.Vizszint = Math.Round(vizszint, 3);
                x.Folyoszint = Math.Round(folyoszint, 3);
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
                    Console.Write("Mit szeretne kezdeni az adatbázissal? mentés/változtatás/törlés/kiírás/keresés; befejezés => vége: ");
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
                                    Folyoszint = item.Folyoszint, 
                                    Amerikai = Delegált.amerikai
                                };
                                meresek.Insert(person);
                            }
                            break;

                        case "kiírás":
                            var query = meresek.FindAll();
                            DbKiir(query, Delegált.amerikai);
                            break;

                        case "változtatás":
                            Console.Write("Kérem a frissíteni kívánt mérés sorszámát: ");
                            int beID = Convert.ToInt32(Console.ReadLine());
                            query = meresek.Find(q => q.ID == beID);
                            Console.WriteLine("Adja meg az új értékeket. Nyomjon enter-t ha meg szeretné tartani a régi értéket.");
                            string ujhomerseklet = "";
                            ujhomerseklet = Console.ReadLine();
                            string ujparatartalom = "";
                            ujparatartalom = Console.ReadLine();
                            string ujvizszint = "";
                            ujvizszint = Console.ReadLine();
                            string ujfolyoszint = "";
                            ujfolyoszint = Console.ReadLine();
                            string ujamerikai = "";
                            ujamerikai = Console.ReadLine();

                            var frissítés = DbFrissítés(beID, ujhomerseklet, ujparatartalom, ujvizszint, ujfolyoszint, query, ujamerikai);
                            meresek.Update(frissítés);
                            break;

                        case "törlés":
                            Console.Write("Melyik mérés(eke)t szeretni törölni? 0 => teljes törlés");
                            beID = Convert.ToInt32(Console.ReadLine()); 
                            if (beID == 0)
                            {
                                meresek.DeleteAll(); 
                            }
                            else
                            {
                                meresek.Delete(beID);
                            }
                                break;

                        case "keresés":
                            Console.WriteLine("Mi alapján szeretne keresni?");
                            valasz = Console.ReadLine().ToLower();
                            switch(valasz)
                            {
                                case "id":
                                    Console.Write("Adja meg az ID számát: ");
                                    beID = Convert.ToInt32(Console.ReadLine());
                                    query = meresek.Find(q => q.ID == beID);
                                    DbKiir(query, Delegált.amerikai);
                                    break;

                                case "hőmérséklet":
                                    Console.Write("Adja meg a keresett hőmérsékletet: ");
                                    double behő = Convert.ToDouble(Console.ReadLine());
                                    query = meresek.Find(q => q.Homerseklet == behő);
                                    DbKiir(query, Delegált.amerikai);
                                    break;

                                case "páratartalom":
                                    Console.Write("Adja meg a keresett páratartalmat: ");
                                    double bepára = Convert.ToDouble(Console.ReadLine());
                                    query = meresek.Find(q => q.Paratartalom == bepára);
                                    DbKiir(query, Delegált.amerikai);
                                    break;

                                case "vízszint":
                                    Console.Write("Adja meg a keresett vízszintet: ");
                                    double bevíz = Convert.ToDouble(Console.ReadLine());
                                    query = meresek.Find(q => q.Vizszint == bevíz);
                                    DbKiir(query, Delegált.amerikai);
                                    break;

                                case "folyószint":
                                    Console.Write("Adja meg a keresett folyószintet: ");
                                    double befolyó = Convert.ToDouble(Console.ReadLine());
                                    query = meresek.Find(q => q.Folyoszint == befolyó);
                                    DbKiir(query, Delegált.amerikai);
                                    break;

                                case "amerikai":
                                    Console.Write("Adja meg hogy amerikai vagy sem a hsznált mértékegység: ");
                                    bool beamerikai = Convert.ToBoolean(Console.ReadLine());
                                    query = meresek.Find(q => q.Amerikai == beamerikai);
                                    DbKiir(query, Delegált.amerikai);
                                    break;
                            }//switch2
                            break;
                    }//switch1
                }//using
            }
            while (bemenet != "vége");

            foreach (var s in értékek) 
            {
                s.Ellenoriz();
            }

            Console.WriteLine("Szeretné a mostani eredméyneket JSON fájlba is menteni? i/n");
            valasz = Console.ReadLine().ToLower();
            if(valasz == "i")
            {
                Json.Jsonki(értékek);
            }//JSON fájlba mentés

            Console.WriteLine("A program vége!");
            Console.ReadKey();
        }//Main

        static void DbKiir(IEnumerable<Szenzor> query, bool amerikai)
        {
            foreach (var item in query)
            {
                string hojel = " C°";
                string parajel = " %";
                string vizjel = " cm";
                string folyojel = " m"; 

                if(amerikai == true)
                {
                    hojel = " F°";
                    vizjel = " inch";
                    folyojel = " inch";
                }

                Console.WriteLine("ID: " + item.ID + "\n" + "Hőmérséklet: " +  item.Homerseklet + hojel + "\n" + "Páratartalom: " + item.Paratartalom + parajel + "\n" + "Vízszint: " + item.Vizszint + vizjel + "\n" + "Folyószint: " + item.Folyoszint + folyojel);
            }
            Console.ReadKey();
        }
        
        static Szenzor DbFrissítés(int ID, string ujhom, string ujpara, string ujviz, string ujfolyo, IEnumerable<Szenzor> query, string ujamerikai)
        {
            var updated = new Szenzor();
            updated.ID = ID;
            if(ujhom == "")
            { updated.Homerseklet = query.Single().Homerseklet; }
            else
            { updated.Homerseklet = Convert.ToDouble(ujhom); }
            if (ujpara == "")
            { updated.Paratartalom = query.Single().Paratartalom; }
            else
            { updated.Paratartalom = Convert.ToDouble(ujpara); }
            if (ujviz == "")
            { updated.Vizszint = query.Single().Vizszint; }
            else
            { updated.Vizszint = Convert.ToDouble(ujviz); }
            if (ujfolyo == "")
            { updated.Folyoszint = query.Single().Folyoszint; }
            else
            { updated.Folyoszint = Convert.ToDouble(ujhom); }
            if (ujamerikai == "")
            { updated.Amerikai = query.Single().Amerikai; }
            else
            { updated.Amerikai = Convert.ToBoolean(ujamerikai); }

            return updated;
        }




    }//Program
}//Namespace
