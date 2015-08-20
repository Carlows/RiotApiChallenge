using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace APItemsWinRate.Models
{
    public class AllItemsRecord
    {
        public int Id { get; set; }
        // # of times each item was bought
        public virtual IList<ItemRecord> Items { get; set; }
    }

    public class ItemRecord
    {
        public int Id { get; set; }

        public int ItemID { get; set; }

        // # of times this item was bought before and after the change
        public int PreChangeRecord { get; set; }
        public int PostChangeRecord { get; set; }
    }
}