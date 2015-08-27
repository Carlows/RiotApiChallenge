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

        public virtual List<DataChampion> MostUsedChampionsPrePatch { get; set; }
        public virtual List<DataChampion> MostUsedChampionsPostPatch { get; set; }
        public virtual List<DataPerRank> DataPerRankPrePatch { get; set; }
        public virtual List<DataPerRank> DataPerRankPostPatch { get; set; }
        public virtual List<DataPerRegion> DataPerRegionPrePatch { get; set; }
        public virtual List<DataPerRegion> DataPerRegionPostPatch { get; set; }
    }
}