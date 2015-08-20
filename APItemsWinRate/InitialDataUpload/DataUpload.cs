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

namespace InitialDataUpload
{
    class DataUpload
    {
        public DataUpload(string dataurl, string apiKey)
        {
            ApiKey = apiKey;
            DataUrl = dataurl;
            fileNames = Directory.GetFiles(DataUrl, "*.json")
                                    .Select(path => Path.GetFileName(path))
                                    .ToList();

            IDMatches = new List<string>();
            Matches = new List<APItemsWinRate.Models.Match>();
            Context = new AppDbContext();
        }

        public void ParseJsonFiles()
        {
            foreach (string filename in fileNames)
            {
                var fileids = DeserializeFile(filename);

                // Taking the first 100 elements to avoid the rate limitation of requests to the api
                // remove this later
                IDMatches.AddRange(fileids.Take(20));
            }
        }

        public void GetDataFromUrl()
        {
            var regex = new Regex(@"^([0-9].11)");
            foreach (string matchId in IDMatches)
            {            
                var matchData = GetMatchData(matchId);

                if (matchData != null)
                {
                    Console.WriteLine("Requested match {0} out of {1}", ++matchesRequested, IDMatches.Count);
                    AddMatch(regex, matchData);
                }
                else
                {
                    Console.WriteLine("Error on the request, requesting the next match");
                }

                Thread.Sleep(1000);
            }

        }

        private void AddMatch(Regex regex, MatchData matchData)
        {
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

        private MatchData GetMatchData(string matchId)
        {
            try
            {
                string requestUri = String.Format("https://lan.api.pvp.net/api/lol/lan/v2.2/match/{0}?api_key={1}", matchId, ApiKey);
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

                MatchData match = JsonConvert.DeserializeObject<MatchData>(json);

                return match;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        private IEnumerable<string> DeserializeFile(string filename)
        {
            // read file into a string and deserialize JSON to a type
            var ids = JsonConvert.DeserializeObject<IEnumerable<string>>(File.ReadAllText(Path.Combine(DataUrl, filename)));

            return ids;
        }

        private int matchesRequested = 0;
        private string DataUrl { get; set; }
        private string ApiKey { get; set; }
        private IEnumerable<string> fileNames { get; set; }
        private List<string> IDMatches { get; set; }
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
            {"CHALLENGER", Rank.CHALLENGER}
        };
    }
}
