using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APItemsWinRate.Models
{
    public class Record_Buy_Percentage
    {
        public int Id { get; set; }

        // # of times each item was bought
        public virtual AllItemsRecord ItemsRecord { get; set; }
    }   
}