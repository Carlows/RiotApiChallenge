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
            DataUpload dataParsing = new DataUpload(path);
            dataParsing.ParseJsonFiles();
            dataParsing.GetDataFromUrl();
            
            Console.WriteLine("Done.");

            Console.WriteLine("Proceeding to create the analysis of all the data uploaded.");
            // Creating a data record
            // such context, where did that come from O_o
            // such code
            // such wow
            // i'm too sleepy to fix it!
            DataAnalysis data = new DataAnalysis(dataParsing.Context, dataParsing.Matches);
            data.CreateRecord();
            data.CreateCalculatedRecord();

            Console.WriteLine("Done. (Maybe, just check for errors(?)");

            Console.ReadKey();
        }
    }
}
