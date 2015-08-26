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
        public DataUpload(string dataurl)
        {
            DataUrl = dataurl;
            fileNames = Directory.GetFiles(DataUrl, "*.json")
                                    .Select(path => Path.GetFileName(path))
                                    .ToList();

            IDMatches = new Queue<string>();
            Matches = new List<APItemsWinRate.Models.Match>();
            Context = new AppDbContext();

            ServicePointManager.DefaultConnectionLimit = 10000;
        }

        public void ParseJsonFiles()
        {
            foreach (string filename in fileNames)
            {
                var fileids = DeserializeFile(filename);

                foreach(string id in fileids)
                {
                    IDMatches.Enqueue(id);
                }
            }
        }

        public IEnumerable<string> TakeAndRemove(Queue<string> queue, int count)
        {
            var list = new List<string>();
            for (int i = 0; i < Math.Min(queue.Count, count); i++)
                list.Add(queue.Dequeue());

            return list;
        }

        public void GetDataFromUrl()
        {
            int howManyRequestsAtATime = 100;
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
        }

        private List<string> GetListOfUrls(IEnumerable<string> idMatches)
        {
            List<string> urls = new List<string>();

            foreach(string id in idMatches)
            {
                // quitar de aqui
                string apiKey = ConfigurationManager.AppSettings["apiKey"];
                string requestUri = String.Format("https://lan.api.pvp.net/api/lol/lan/v2.2/match/{0}?api_key={1}", id, apiKey);
                urls.Add(requestUri);
            }

            return urls;
        }

        private void AddMatch(MatchData matchData)
        {
            var regex = new Regex(@"^([0-9].11)");
            var match = new APItemsWinRate.Models.Match();
            match.MatchId = matchData.matchId;
            match.Is_Ranked = matchData.queueType.Equals("RANKED_SOLO_5x5");
            match.Match_Region = dictRegion[matchData.region];
            match.Pre_Change = regex.IsMatch(matchData.matchVersion);
            match.Players = new List<Player>();

            foreach (Participant participant in matchData.participants)
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
                    Winner = participant.stats.winner,
                    ItemsBought = itemsBought
                };

                player.ChampionUsed = Context.Champions.Where(c => c.ChampionId == participant.championId).Single();
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
        private Queue<string> IDMatches { get; set; }
        public List<APItemsWinRate.Models.Match> Matches { get; set; }
        public AppDbContext Context { get; set; }

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
