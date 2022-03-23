using LiteDB;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Inlämning1
{

    internal class Program
    {
        static void Main(string[] args)
        {
            using var db = new LiteDatabase("formulärsvar.db");
            var col = db.GetCollection<Svar>("svar");

            using (var reader = new TextFieldParser(@"Jobbintervju15.csv"))
            {
                reader.SetDelimiters(new string[] { "," });
                reader.ReadLine(); // Läs och glöm bort första raden

                while (!reader.EndOfData)
                {
                    string[] columns = reader.ReadFields();
                    Svar svar = new Svar();

                    svar.TidigareYrke = columns[1];
                    try
                    {
                        svar.ErfaranhetÅr = int.Parse(Regex.Replace(columns[2], @"\D", ""));
                    }
                    catch (Exception)
                    {

                        svar.ErfaranhetÅr = 0;
                    }

                    try
                    {
                        svar.FörväntadLön = int.Parse(Regex.Replace(columns[3], @"\D", ""));
                    }
                    catch (Exception)
                    {

                        svar.FörväntadLön = 0;
                    }                    
                    
                    svar.Projekt = columns[4];

                    //col.Insert(svar);
                }
            }

            var totalFörväntadLön = col.Query().Select(s => s.FörväntadLön).ToList().Sum();
            var antal = col.Query().Count();

            Console.WriteLine("Sammanlagd förväntad lön: " + totalFörväntadLön);
            Console.WriteLine("Antal svarande: " + antal);
            Console.WriteLine("Genomsnittlig förväntad lön: " + totalFörväntadLön / antal);
        }

    }
}
