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
            var context = new AppDbContext();
            var mcontext = new MatchesDBContext();
            bool downloading = false;

            if (downloading)
            {
                string path = @"C:\RiotMatches\Current";
                DataUpload dataParsing = new DataUpload(path, context, mcontext);
                dataParsing.ParseJsonFiles();
                dataParsing.GetDataFromUrl();

                Console.WriteLine("Done.");
            }
            else
            {
                var matches = mcontext.Matches.ToList();

                Console.WriteLine("Proceeding to create the analysis of all the data uploaded.");
                DataAnalysis data = new DataAnalysis(context, mcontext, matches);
                data.CreateRecord();
                data.CreateCalculatedRecord();

                Console.WriteLine("Done. (Maybe, just check for errors(?)");
            }

            Console.ReadKey();
        }
    }
}
