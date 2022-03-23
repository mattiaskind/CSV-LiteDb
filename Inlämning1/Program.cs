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
                        svar.ErfarenhetÅr = int.Parse(Regex.Replace(columns[2], @"\D", ""));
                    }
                    catch (Exception)
                    {

                        svar.ErfarenhetÅr = 0;
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

            var allaSvar = col.FindAll();
            //foreach (var svar in allaSvar)
            //{                
            //    //Console.WriteLine(svar.TidigareYrke == null);
            //    Console.WriteLine(svar.TidigareYrke + " " + svar.ErfarenhetÅr + " " + svar.FörväntadLön + " " + svar.Projekt);
            //}



            //Console.WriteLine("Sammanlagd förväntad lön: " + totalFörväntadLön);




            // Svar / analyser

            // Mängd svar
            Console.WriteLine("---------------------------------");
            var antalSvar = col.Query().Count();
            Console.WriteLine("Antal svarande: " + antalSvar);

            // Mängd fullständiga svar (där inget värde == NULL || 0) 
            Console.WriteLine("---------------------------------");
            var fullständigaSvar = 0;
            foreach (var svar in allaSvar)
            {
                if (!String.IsNullOrEmpty(svar.TidigareYrke))
                {
                    Console.WriteLine(svar.TidigareYrke);
                    if (svar.ErfarenhetÅr > 0)
                    {
                        if (svar.FörväntadLön > 0)
                        {
                            if (svar.Projekt != null || svar.Projekt != "")
                            {
                                fullständigaSvar++;
                            }
                        }
                    }
                }
            }
            Console.WriteLine("\nAntal fullständiga svar: " + fullständigaSvar);

            // Genomsnittlig yrkeserfarenhet i år
            Console.WriteLine("---------------------------------");

            // Genomsnittlig förhoppning på lön
            //var totalFörväntadLön = col.Query().Select(s => s.FörväntadLön).ToList().Sum();
            //Console.WriteLine("Genomsnittlig förväntad lön: " + totalFörväntadLön / antal);    // <------------ dividera med antal fullständiga svar, dvs inte 0 ?

            // Antal miniräknare+kalkylator
            // Antal unika projekt
        }

    }
}
