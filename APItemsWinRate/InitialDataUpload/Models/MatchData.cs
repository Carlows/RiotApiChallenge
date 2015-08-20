using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InitialDataUpload.Models
{
    public class MatchData
    {
        public string matchId { get; set; }
        public string region { get; set; }
        public string queueType { get; set; }
        public string matchVersion { get; set; }
        public IEnumerable<Participant> participants { get; set; }
    }

    public class Participant
    {
        public int championId { get; set; }
        public string highestAchievedSeasonTier { get; set; }
        public Stats stats { get; set; }
    }

    public class Stats
    {
        public bool winner { get; set; }
        public string item0 { get; set; }
        public string item1 { get; set; }
        public string item2 { get; set; }
        public string item3 { get; set; }
        public string item4 { get; set; }
        public string item5 { get; set; }
        public string item6 { get; set; }
    }
}
