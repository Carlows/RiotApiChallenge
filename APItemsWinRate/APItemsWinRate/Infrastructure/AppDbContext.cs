using APItemsWinRate.Models;
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
            Database.SetInitializer<AppDbContext>(new DbInitializer());
        }

        public DbSet<Match> Matches { get; set; }
        public DbSet<Player> Participants { get; set; }
        public DbSet<Champion> Champions { get; set; }
        public DbSet<Record> Records { get; set; }
        public DbSet<Record_Buy_Percentage> RecordsByBuyPercentage { get; set; }
        public DbSet<Record_Champions> RecordsByChampion { get; set; }
        public DbSet<AllItemsRecord> AllItemsRecords { get; set; }
        public DbSet<ChampionRecord> ChampionRecords { get; set; }
        public DbSet<ItemRecord> ItemRecords { get; set; }
    }

    public class DbInitializer : DropCreateDatabaseIfModelChanges<AppDbContext>
    {
        protected override void Seed(AppDbContext context)
        {
            // Geting champions data
            var champions = GetChampionsData();

            foreach(Champion champ in champions)
            {
                context.Champions.Add(champ);
                context.SaveChanges();
            }


            base.Seed(context);
        }

        private List<Champion> GetChampionsData()
        {
            string requestUri = "https://global.api.pvp.net/api/lol/static-data/na/v1.2/champion?champData=blurb,lore,tags&api_key=73cb4b46-ae25-440f-bb17-da66159eff9b";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Method = WebRequestMethods.Http.Get;
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Proxy = null;
            WebResponse response = httpWebRequest.GetResponse();

            string json;

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                json = sr.ReadToEnd();
            }

            dynamic data = JObject.Parse(json);

            var champions = new List<Champion>();

            foreach(dynamic champion in data.data)
            {
                var champ = champion.Value;
                var newChamp = new Champion
                {
                    ChampionId = champ.id,
                    Name = champ.name
                };

                champions.Add(newChamp);
            }

            return champions;
        }
    }


}
