using LiteDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beadando
{
    class Szenzor
    {
        public int ID { get; set; }
        public double Homerseklet {  get; set; }
        public double Paratartalom {  get; set; }
        public double Vizszint {  get; set; }
        public double Folyoszint { get; set; }

        public Szenzor(int homerseklet, int paratartalom, int vizszint, int folyoszint)
        {
            Homerseklet = homerseklet;
            Paratartalom = paratartalom;
            Vizszint = vizszint;
            Folyoszint = folyoszint;
        } 

        public Szenzor() { }

        public override string ToString()
        {
            return Math.Round(Homerseklet, 3) + " C°" + "; " + Math.Round(Paratartalom, 3) + " %" + "; " + Math.Round(Vizszint, 3) + " cm" + "; " + Math.Round(Folyoszint, 3) + " m";
        }
    }//Szenzor osztály


    internal class Program
    {
        static List<Szenzor> értékek = new List<Szenzor>();
        delegate void Atvaltas(int x);


        static void Main(string[] args)
        {
            
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

            for (int i = 0; i < n; i++) //kiiratás
            {
                Console.WriteLine(értékek[i].ToString());
            }



            using (var db = new LiteDatabase("Meres.db"))
            {
                var meresek = db.GetCollection<Szenzor>("meresek"); 
                foreach(var item in értékek)
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
                var query = meresek.FindAll();
                foreach (var item in query)
                {
                    Console.WriteLine(item.ID + "\n" + item.Homerseklet + "\n" + item.Paratartalom + "\n" + item.Vizszint + "\n" + item.Folyoszint);
                }
            }

            Json();


            Console.ReadKey();
        }//Main

        private static void Json()
        {
            StreamWriter ki = new StreamWriter("adatok.json");
            ki.WriteLine(JsonConvert.SerializeObject(értékek, Formatting.Indented));
            ki.Flush();
            ki.Close();
        }//Json file-ba írás
    }//Program
}//Namespace
