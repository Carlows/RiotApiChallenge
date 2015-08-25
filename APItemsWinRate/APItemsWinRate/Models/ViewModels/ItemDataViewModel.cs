using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APItemsWinRate.Models.ViewModels
{
    public class ItemDataViewModel
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public DataViewModel DataPreChange { get; set; }
        public DataViewModel DataPostChange { get; set; }
    }

    public class DataViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Plaintext { get; set; }
        public string[] From { get; set; }
        public Gold Gold { get; set; }
    }
}