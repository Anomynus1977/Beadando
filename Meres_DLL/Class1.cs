using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Meres_DLL
{
    public class Szenzor
    {

        public int ID { get; set; }
        public double Homerseklet { get; set; }
        public double Paratartalom { get; set; }
        public double Vizszint { get; set; }
        public double Folyoszint { get; set; } 
        public bool Amerikai { get; set; }

        public Szenzor(int homerseklet, int paratartalom, int vizszint, int folyoszint)
        {
            Homerseklet = homerseklet;
            Paratartalom = paratartalom;
            Vizszint = vizszint;
            Folyoszint = folyoszint;
        }
        public Szenzor() { }
        public event EventHandler<string> Riasztas;

        public void Ellenoriz() 
        {
        if (Homerseklet > 80) 
        {
                Riasztas?.Invoke(this, $"Riasztás! Túl magas hőmérséklet: {Math.Round(Homerseklet, 2)}");
        }
        }



public override string ToString()
        {
            return Homerseklet + " C°" + "; " + Paratartalom + " %" + "; " + Vizszint + " cm" + "; " + Folyoszint + " m";
        }

    }//Szenzor osztály

    public class Json
    {
        public static void Jsonki(List<Szenzor> értékek)
        {
            StreamWriter ki = new StreamWriter("adatok.json");
            ki.WriteLine(JsonConvert.SerializeObject(értékek, Formatting.Indented));
            ki.Flush();
            ki.Close();
        }//Json file-ba írás
    }//Json osztály

    public class Delegált
    {
        public delegate void Atvaltas(double x);

        static public bool amerikai = false;

        public delegate double Amerikai(double ertek);

        static public double CelsiusToFahrenheit(double c) => c * 9 / 5 + 32;
        static public double CmToInch(double cm) => cm / 2.54;
        static public double MeterToInch(double m) => m * 39.3701;

        public static void Atvalt(Szenzor s, Amerikai hoAtv, Amerikai vizAtv, Amerikai folyoAtv)
        {
            s.Homerseklet = hoAtv(s.Homerseklet);
            s.Vizszint = vizAtv(s.Vizszint);
            s.Folyoszint = folyoAtv(s.Folyoszint);
            Console.WriteLine(s.Homerseklet + " F°" + "; " + s.Paratartalom + " %" + "; " + s.Vizszint + " inch" + "; " + s.Folyoszint + " inch");
        }

    }//Delegált osztály
}
