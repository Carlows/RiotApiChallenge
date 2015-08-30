using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APItemsWinRate.Models.ViewModels
{
    public class ItemDataRequest
    {
        public string id { get; set; }
        public string name { get; set; }
        public string sanitizedDescription { get; set; }
        public string plaintext { get; set; }
        public string[] from { get; set; }
        public GoldItem gold { get; set; }
    }

    public class GoldItem
    {
        public string Base { get; set; }
        public string total { get; set; }
    }
}