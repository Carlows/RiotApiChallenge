using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APItemsWinRate.Models
{
    public class CalculatedRecord
    {
        public CalculatedRecord()
        {
            DateCreated = DateTime.Now;
        }

        public int Id { get; set; }

        public DateTime DateCreated { get; set; }

        [Required]
        public virtual List<DataItem> Items { get; set; }
    }
}