using APItemsWinRate.Models;
using APItemsWinRate.Models.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace APItemsWinRate.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
            : base("DefaultConnection")
        {
            //Database.SetInitializer<AppDbContext>(new DbInitializer());
        }

        public DbSet<Champion> Champions { get; set; }
        public DbSet<Record> Records { get; set; }
        public DbSet<Record_Buy_Percentage> RecordsByBuyPercentage { get; set; }
        public DbSet<Record_Champions> RecordsByChampion { get; set; }
        public DbSet<AllItemsRecord> AllItemsRecords { get; set; }
        public DbSet<ChampionRecord> ChampionRecords { get; set; }
        public DbSet<ItemRecord> ItemRecords { get; set; }
        public DbSet<ItemData> Items { get; set; }
        public DbSet<Data> Data { get; set; }
        public DbSet<Gold> Gold { get; set; }
        public DbSet<CalculatedRecord> CalculatedRecord { get; set; }
        public DbSet<DataItem> CalculatedDataItem { get; set; }
        public DbSet<DataChampion> CalculatedDataChampion { get; set; }
        public DbSet<DataPerRank> DataPerRank { get; set; }
        public DbSet<DataPerRegion> DataPerRegion { get; set; }
        public DbSet<KDAAverage> KDAAverage { get; set; }
        public DbSet<MultiKills> MultiKills { get; set; }
    }      
}
