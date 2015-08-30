using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APItemsWinRate.Models.ViewModels
{
    public class DataByItemViewModel
    {
        public IList<DataItemViewModel> Items { get; set; }
    }

    public class DataItemViewModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public float PrePatch { get; set; }
        public float PostPatch { get; set; }

        public string[] MostUsedChampionsPrePatchLabels { get; set; }
        public string[] MostUsedChampionsPostPatchLabels { get; set; }
        public int[] MostUsedChampionsPrePatchData { get; set; }
        public int[] MostUsedChampionsPostPatchData { get; set; }

        // By rank
        public string[] ItemDataByRankPrePatchLabels { get; set; }
        public string[] ItemDataByRankPostPatchLabels { get; set; }
        public int[] ItemDataByRankPrePatchData { get; set; }
        public int[] ItemDataByRankPostPatchData { get; set; }

        // By region
        public string[] ItemDataByRegionPrePatchLabels { get; set; }
        public string[] ItemDataByRegionPostPatchLabels { get; set; }
        public int[] ItemDataByRegionPrePatchData { get; set; }
        public int[] ItemDataByRegionPostPatchData { get; set; }

        // KDA
        public KDAAverage KDAAvgPrePatch { get; set; }
        public KDAAverage KDAAvgPostPatch { get; set; }

        // Multikills
        public MultiKills MultiKillsPrePatch { get; set; }
        public MultiKills MultiKillsPostPatch { get; set; }

        // WinRate
        public int WinRatePrePatch { get; set; }
        public int WinRatePostPatch { get; set; }

        // Most Magic Dmg Champions
        public List<DataChampion> ChampionsMostDmgPrePatch { get; set; }
        public List<DataChampion> ChampionsMostDmgPostPatch { get; set; }

        public ItemDataViewModel Data { get; set; }
    }
}