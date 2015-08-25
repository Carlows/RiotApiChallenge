using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APItemsWinRate.Models
{
    public class Record
    {
        public Record()
        {
            DateCreated = DateTime.Now;
        }

        public int Id { get; set; }
        
        // Number of matches studied
        public int Matches { get; set; }
        // Number of players studied
        public int Players { get; set; }

        public DateTime DateCreated { get; set; }

        [Required]
        public virtual Record_Buy_Percentage RecordsByBuyPercentage { get; set; }
        [Required]
        public virtual Record_Champions RecordsByChampions { get; set; }
    }
}