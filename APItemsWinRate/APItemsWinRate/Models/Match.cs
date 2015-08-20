using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APItemsWinRate.Models
{
    public class Match
    {
        public int Id { get; set; }

        public string MatchId { get; set; }
        // Is this pre-patch 5.13 match?
        public bool Pre_Change { get; set; }
        // Is this a ranked match?
        public bool Is_Ranked { get; set; }
        // What's the higher rank on this match?
        public Rank Highest_Rank { get; set; }
        // What region is this match from?
        public Region Match_Region { get; set; }

        // Id's of items bought by both teams (winner and loser)
        public virtual IList<Player> Players { get; set; }
    }
    
    public enum Rank
    {
        UNRANKED = 0,
        BRONZE = 1,
        SILVER = 2,
        GOLD = 3,
        PLATINUM = 4,
        DIAMOND = 5,
        CHALLENGER = 6
    }

    public enum Region
    {
        BR,
        EUNE,
        EUW,
        KR,
        LAN,
        LAS,
        NA,
        OCE,
        RU,
        TR
    }
}
