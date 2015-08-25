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
        public DbSet<ItemData> Items { get; set; }
        public DbSet<Data> Data { get; set; }
        public DbSet<Gold> Gold { get; set; }
        public DbSet<CalculatedRecord> CalculatedRecord { get; set; }
        public DbSet<DataItem> CalculatedDataItem { get; set; }
        public DbSet<DataChampion> CalculatedDataChampion { get; set; }
    }

    public class DbInitializer : DropCreateDatabaseIfModelChanges<AppDbContext>
    {
        protected override void Seed(AppDbContext context)
        {
            // Getting champions data
            var champions = GetChampionsData();

            foreach(Champion champ in champions)
            {
                context.Champions.Add(champ);
                context.SaveChanges();
            }

            // Getting items data
            foreach (var key in APItems.Items.Keys)
            {
                ItemData item = new ItemData();

                var dataPrePatch = GetItemData(APItems.Items[key], "5.11.1");
                var dataPostPatch = GetItemData(APItems.Items[key], "5.14.1");
                item.ItemId = int.Parse(dataPrePatch.id);

                var preChangeData = new Data
                {
                    Name = dataPrePatch.name,
                    Plaintext = dataPrePatch.plaintext,
                    Description = dataPrePatch.description,
                    From = string.Join(",", dataPrePatch.from),
                    Gold = new Gold
                    {
                        Base = dataPrePatch.gold.Base,
                        Total = dataPrePatch.gold.total
                    }
                };

                var postChangeData = new Data
                {
                    Name = dataPostPatch.name,
                    Plaintext = dataPostPatch.plaintext,
                    Description = dataPostPatch.description,
                    From = string.Join(",", dataPostPatch.from),
                    Gold = new Gold
                    {
                        Base = dataPostPatch.gold.Base,
                        Total = dataPostPatch.gold.total
                    }
                };

                item.DataPreChange = preChangeData;
                item.DataPostChange = postChangeData;

                context.Items.Add(item);
                context.SaveChanges();
            }


            base.Seed(context);
        }

        private ItemDataRequest GetItemData(int itemId, string version)
        {
            string apiKey = System.Web.Configuration.WebConfigurationManager.AppSettings["ApiKey"];
            string requestUri = String.Format("https://global.api.pvp.net/api/lol/static-data/na/v1.2/item/{0}?version={1}&itemData=from,gold,stats&api_key={2}", itemId, version, apiKey);
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

            ItemDataRequest data = JsonConvert.DeserializeObject<ItemDataRequest>(json);

            return data;
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
