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

        public ItemDataViewModel Data { get; set; }
    }
}