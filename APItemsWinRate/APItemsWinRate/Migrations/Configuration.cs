namespace APItemsWinRate.Migrations
{
    using APItemsWinRate.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using APItemsWinRate.Infrastructure;
    using APItemsWinRate.Models.ViewModels;
    using System.Net;
    using System.IO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<APItemsWinRate.Infrastructure.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "APItemsWinRate.Infrastructure.AppDbContext";
        }

        protected override void Seed(APItemsWinRate.Infrastructure.AppDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            // Getting champions data
            if (context.Champions.ToList().Count == 0 && context.Data.ToList().Count == 0)
            {
                var champions = GetChampionsData();

                foreach (Champion champ in champions)
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
                        Description = dataPrePatch.sanitizedDescription,
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
                        Description = dataPostPatch.sanitizedDescription,
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
            }
        }

        public ItemDataRequest GetItemData(int itemId, string version)
        {
            string apiKey = System.Web.Configuration.WebConfigurationManager.AppSettings["ApiKey"];
            string requestUri = String.Format("https://global.api.pvp.net/api/lol/static-data/na/v1.2/item/{0}?version={1}&itemData=all&api_key={2}", itemId, version, apiKey);
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
            string apiKey = System.Web.Configuration.WebConfigurationManager.AppSettings["ApiKey"];
            string requestUri = String.Format("https://global.api.pvp.net/api/lol/static-data/na/v1.2/champion?champData=blurb,lore,tags&api_key={0}", apiKey);
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

            foreach (dynamic champion in data.data)
            {
                var champ = champion.Value;
                var newChamp = new Champion
                {
                    ChampionId = champ.id,
                    Name = champ.name,
                    Key = champ.key
                };

                champions.Add(newChamp);
            }

            return champions;
        }
    }
}
