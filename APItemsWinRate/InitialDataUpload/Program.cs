using APItemsWinRate.Infrastructure;
using APItemsWinRate.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InitialDataUpload
{
    class Program
    {
        static void Main(string[] args)
        {
            // Please, run the website project before this project
            // that way, champion and item data will be already on the database

            string path = @"C:\RiotMatches";
            string apiKey = "73cb4b46-ae25-440f-bb17-da66159eff9b";
            DataUpload dataParsing = new DataUpload(path, apiKey);
            dataParsing.ParseJsonFiles();
            dataParsing.GetDataFromUrl();

            // Load match data to the database
            Console.WriteLine("Uploading Match data to the database...");
            foreach (Match match in dataParsing.Matches)
            {
                var cmatch = dataParsing.Context.Matches.FirstOrDefault(m => m.MatchId == match.MatchId);

                if (cmatch == null)
                {
                    dataParsing.Context.Matches.Add(match);
                    dataParsing.Context.SaveChanges();
                }
            }

            Console.WriteLine("Done.");

            Console.WriteLine("Proceeding to create the analysis of all the data uploaded.");
            // Creating a data record
            // such context, where did that come from O_o
            // such code
            // such wow
            // i'm too sleepy to fix it!
            DataAnalysis data = new DataAnalysis(dataParsing.Context);
            data.CreateRecord();

            Console.WriteLine("Done. (Maybe, just check for errors(?)");

            Console.ReadKey();
        }
    }
}
