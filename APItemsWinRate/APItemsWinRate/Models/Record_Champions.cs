using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace APItemsWinRate.Models
{
    public class Record_Champions
    {
        public int Id { get; set; }

        public virtual IList<ChampionRecord> ChampionsRecords { get; set; }
    }

    public class ChampionRecord
    {
        public int Id { get; set; }

        public int ChampionId { get; set; }

        // # of times each item was bought by this champion
        public virtual AllItemsRecord ItemsRecord { get; set; }
    }
}