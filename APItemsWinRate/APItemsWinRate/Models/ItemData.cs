using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APItemsWinRate.Models
{
    public class ItemData
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public virtual Data DataPreChange { get; set; }
        public virtual Data DataPostChange { get; set; }
    }

    public class Data
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Plaintext { get; set; }
        public string From { get; set; }
        public virtual Gold Gold { get; set; }
    }

    public class Gold
    {
        public int Id { get; set; }
        public string Base { get; set; }
        public string Total { get; set; }
    }
}