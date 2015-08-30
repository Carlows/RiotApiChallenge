using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;
using InitialDataUpload.Models;
using System.Text.RegularExpressions;
using APItemsWinRate.Models;
using System.Threading;
using APItemsWinRate.Infrastructure;
using System.Configuration;

namespace InitialDataUpload
{
    class DataUpload
    {
        public DataUpload(string dataurl, AppDbContext context, MatchesDBContext mcontext)
        {
            DataUrl = dataurl;
            fileNames = Directory.GetFiles(DataUrl, "*.json")
                                    .Select(path => Path.GetFileName(path))
                                    .ToList();

            // first string for id, second for region
            IDMatches = new Queue<RegionId>();
            Matches = new List<InitialDataUpload.Models.Match>();
            Context = context;
            MatchesContext = mcontext;

            ServicePointManager.DefaultConnectionLimit = 10000;
        }

        public void ParseJsonFiles()
        {
            // matches the [region]_xxx of the file
            // so the file must be named with the region of the matches first
            var regex = new Regex(@"^([A-Z]+)");
            foreach (string filename in fileNames)
            {
                var fileids = DeserializeFile(filename);
                System.Text.RegularExpressions.Match match = regex.Match(filename);
                foreach(string id in fileids.Skip(4000).Take(1000))
                {
                    var regionid = new RegionId()
                    {
                        Id = id,
                        Region = match.Value
                    };

                    IDMatches.Enqueue(regionid);
                }
            }
        }

        public IEnumerable<RegionId> TakeAndRemove(Queue<RegionId> queue, int count)
        {
            var list = new List<RegionId>();
            for (int i = 0; i < Math.Min(queue.Count, count); i++)
                list.Add(queue.Dequeue());

            return list;
        }

        public void GetDataFromUrl()
        {
            int howManyRequestsAtATime = 10;
            HttpSocket socket = new HttpSocket();
            var matchDataList = new List<MatchData>();

            Console.WriteLine("Total matches to request: {0}", IDMatches.Count);
            while (IDMatches.Count > 0)
            {
                var idMatches = TakeAndRemove(IDMatches, howManyRequestsAtATime);

                var urls = GetListOfUrls(idMatches);
                var tasks = urls.Select(socket.GetAsync).ToArray();
                var completed = Task.Factory.ContinueWhenAll(tasks, completedTasks =>
                                    {
                                        foreach (var result in completedTasks.Select(t => t.Result))
                                        {
                                            if (!string.IsNullOrEmpty(result))
                                            {
                                                MatchData match = JsonConvert.DeserializeObject<MatchData>(result);
                                                matchDataList.Add(match);
                                            }
                                        }
                                    });
                completed.Wait();
            }

            foreach(MatchData match in matchDataList)
            {
                if(match != null)
                {
                    AddMatch(match);
                }
            }

            MatchesContext.Configuration.AutoDetectChangesEnabled = false;
            MatchesContext.Configuration.ValidateOnSaveEnabled = false;
            int count = 0;
            foreach(InitialDataUpload.Models.Match match in Matches)
            {
                MatchesContext.Matches.Add(match);
                count++;

                if(count % 100 == 0)
                {
                    count = 0;
                    MatchesContext.SaveChanges();
                }
            }

            MatchesContext.SaveChanges();
        }

        private List<string> GetListOfUrls(IEnumerable<RegionId> idMatches)
        {
            List<string> urls = new List<string>();

            foreach(RegionId id in idMatches)
            {
                string apiKey = ConfigurationManager.AppSettings["apiKey"];
                string requestUri = String.Format("https://{0}.api.pvp.net/api/lol/{1}/v2.2/match/{2}?api_key={3}", id.Region, id.Region.ToLower(), id.Id, apiKey);
                urls.Add(requestUri);
            }

            return urls;
        }

        private void AddMatch(MatchData matchData)
        {
            var regex = new Regex(@"^([0-9].11)");
            var match = new InitialDataUpload.Models.Match();
            match.MatchId = matchData.matchId;
            match.Is_Ranked = matchData.queueType.Equals("RANKED_SOLO_5x5");
            match.Match_Region = dictRegion[matchData.region];
            match.Pre_Change = regex.IsMatch(matchData.matchVersion);
            match.Players = new List<Player>();

            foreach (InitialDataUpload.Models.Participant participant in matchData.participants)
            {
                var itemsBought = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                    participant.stats.item0,
                    participant.stats.item1,
                    participant.stats.item2,
                    participant.stats.item3,
                    participant.stats.item4,
                    participant.stats.item5,
                    participant.stats.item6);

                var player = new Player
                {
                    Rank = dictRank[participant.highestAchievedSeasonTier],
                    Region = dictRegion[matchData.region],
                    Winner = participant.stats.winner,
                    ItemsBought = itemsBought,
                    Kills = participant.stats.kills,
                    Deaths = participant.stats.deaths,
                    Assists = participant.stats.assists,
                    TripleKills = participant.stats.tripleKills,
                    QuadraKills = participant.stats.quadraKills,
                    PentaKills = participant.stats.pentaKills,
                    MagicDamageDealt = participant.stats.magicDamageDealt,
                    LargestKillingSpree = participant.stats.largestKillingSpree
                };

                player.ChampionUsed = MatchesContext.Champions.Where(c => c.ChampionId == participant.championId).Single();
                match.Players.Add(player);
            }

            match.Highest_Rank = (Rank)match.Players.Select(p => p.Rank).Max();

            Matches.Add(match);
        }

        private IEnumerable<string> DeserializeFile(string filename)
        {
            // read file into a string and deserialize JSON to a type
            var ids = JsonConvert.DeserializeObject<IEnumerable<string>>(File.ReadAllText(Path.Combine(DataUrl, filename)));

            return ids;
        }

        private int matchesRequested = 0;
        private string DataUrl { get; set; }
        private IEnumerable<string> fileNames { get; set; }
        private Queue<RegionId> IDMatches { get; set; }
        public List<InitialDataUpload.Models.Match> Matches { get; set; }
        public AppDbContext Context { get; set; }
        public MatchesDBContext MatchesContext { get; set; }

        public class RegionId
        {
            public string Id { get; set; }
            public string Region { get; set; }
        }

        // dict to convert regions
        Dictionary<string, Region> dictRegion = new Dictionary<string, Region>()
        {
            {"BR", Region.BR},
            {"EUNE", Region.EUNE},
            {"EUW", Region.EUW},
            {"KR", Region.KR},
            {"LAN", Region.LAN},
            {"LAS", Region.LAS},
            {"NA", Region.NA},
            {"OCE", Region.OCE},
            {"RU", Region.RU},
            {"TR", Region.TR}
        };

        // dict to convert ranks
        Dictionary<string, Rank> dictRank = new Dictionary<string, Rank>()
        {
            {"UNRANKED", Rank.UNRANKED},
            {"BRONZE", Rank.BRONZE},
            {"SILVER", Rank.SILVER},
            {"GOLD", Rank.GOLD},
            {"PLATINUM", Rank.PLATINUM},
            {"DIAMOND", Rank.DIAMOND},
            {"MASTER", Rank.MASTER},
            {"CHALLENGER", Rank.CHALLENGER}
        };
    }
}
