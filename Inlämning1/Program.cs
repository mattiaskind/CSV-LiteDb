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

                    // Kör den här en gång och kommentera därefter ut den.
                    //col.Insert(svar);
                }
            }

            // Svar / analyser

            // Mängd svar
            Console.WriteLine("---------------------------------");
            var antalSvar = col.Query().Count();
            Console.WriteLine("Antal svarande: " + antalSvar);

            // Mängd fullständiga svar (där inget värde == NULL || 0)             
            Console.WriteLine("---------------------------------");

            var fullständigaSvar = col.Find(Query.And(Query.Not("TidigareYrke", null), Query.Not("ErfarenhetÅr", 0), Query.Not("FörväntadLön", 0)));
            Console.WriteLine("Antal fullständiga svar: " + fullständigaSvar.Count());

            //foreach (var svar in fullständigaSvar)
            //{
            //    Console.WriteLine(svar.TidigareYrke + " " + svar.ErfarenhetÅr + " " + svar.FörväntadLön + " " + svar.Projekt);   
            //}

            // Genomsnittlig yrkeserfarenhet i år
            Console.WriteLine("---------------------------------");
            double summaYrkeserfarenhet = col.Find(Query.All("ErfarenhetÅr")).Sum(x => x.ErfarenhetÅr);     
            Console.WriteLine("Genomsnittlig yrkeserfarenhet i år: " + Math.Round((summaYrkeserfarenhet/ col.Find(Query.Not("ErfarenhetÅr", 0)).Count()) ,2));

            // Genomsnittlig förhoppning på lön
            Console.WriteLine("---------------------------------");
            double summaLön = col.Find(Query.All("FörväntadLön")).Sum(x => x.FörväntadLön);
            Console.WriteLine("Genomsnittlig förhoppning på lön: " + Math.Round((summaLön / col.Find(Query.Not("FörväntadLön", 0)).Count()), 2));
                       
            // Antal miniräknare+kalkylator
            Console.WriteLine("---------------------------------");
            string[] miniräknare = { "Miniräknare", "miniräknare", "Kalkylator", "kalkylator" };
            var antalMiniräknare = col.Find(x => miniräknare.Contains(x.Projekt)).Count();            
            Console.WriteLine("Antal projekt som var miniräknare: " + antalMiniräknare);

            // Antal unika projekt 
            Console.WriteLine("---------------------------------");
            List<string> uniqueProjects = new List<string>();
            var allProjects = col.FindAll().Select(x => x.Projekt).ToList();

            foreach (var project in allProjects)
            {
                if(!uniqueProjects.Contains(project.ToLower())) uniqueProjects.Add(project.ToLower());
            }

            Console.WriteLine("Antal unika projekt: " + uniqueProjects.Count());
            //foreach (var project in uniqueProjects)
            //{
            //    Console.WriteLine(project);
            //}
        }
    }
}
