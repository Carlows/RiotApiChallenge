using InitialDataUpload.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InitialDataUpload
{
    public class MatchesDBContext : DbContext
    {
        public MatchesDBContext()
            : base("MatchesDbConnection")
        {
            //Database.SetInitializer<MatchesDBContext>(new DbInitializer());
        }

        public DbSet<Match> Matches { get; set; }
        public DbSet<Player> Participants { get; set; }
        public DbSet<Champion> Champions { get; set; }
    }

    public class DbInitializer : DropCreateDatabaseIfModelChanges<MatchesDBContext>
    {
        protected override void Seed(MatchesDBContext context)
        {
            // Getting champions data
            var champions = GetChampionsData();

            foreach (Champion champ in champions)
            {
                context.Champions.Add(champ);
                context.SaveChanges();
            }       

            base.Seed(context);
        }

        private List<Champion> GetChampionsData()
        {
            string apiKey = ConfigurationManager.AppSettings["apiKey"];
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
                    Name = champ.name
                };

                champions.Add(newChamp);
            }

            return champions;
        }
    }

}
