using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APItemsWinRate.Models
{
    public class DataItem
    {
        public int Id { get; set; }

        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public float PrePatch { get; set; }
        public float PostPatch { get; set; }
                        
        public int WinRatePrePatch { get; set; }
        public int WinRatePostPatch { get; set; }

        public virtual KDAAverage KDAAvgPrePatch { get; set; }
        public virtual KDAAverage KDAAvgPostPatch { get; set; }
        public virtual MultiKills MultiKillsPrePatch { get; set; }
        public virtual MultiKills MultiKillsPostPatch { get; set; }
        public virtual List<DataChampion> ChampionsWithMoreMagicDamagePrePatch { get; set; }
        public virtual List<DataChampion> ChampionsWithMoreMagicDamagePostPatch { get; set; }
        public virtual List<DataChampion> MostUsedChampionsPrePatch { get; set; }
        public virtual List<DataChampion> MostUsedChampionsPostPatch { get; set; }
        public virtual List<DataPerRank> DataPerRankPrePatch { get; set; }
        public virtual List<DataPerRank> DataPerRankPostPatch { get; set; }
        public virtual List<DataPerRegion> DataPerRegionPrePatch { get; set; }
        public virtual List<DataPerRegion> DataPerRegionPostPatch { get; set; }
    }

    public class KDAAverage
    {
        public int Id { get; set; }

        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
    }

    // Total multikills
    public class MultiKills
    {
        public int Id { get; set; }

        public int PentaKills { get; set; }
        public int QuadraKills { get; set; }
        public int TripleKills { get; set; }
        public int DoubleKills { get; set; }
    }
}