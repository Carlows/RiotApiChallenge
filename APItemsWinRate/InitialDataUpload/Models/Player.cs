using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InitialDataUpload.Models
{
    public class Player
    {
        public Player()
        {
        }

        public int Id { get; set; }

        public Rank Rank { get; set; }
        public Region Region { get; set; }
        public bool Winner { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }

        public int PentaKills { get; set; }
        public int QuadraKills { get; set; }
        public int TripleKills { get; set; }
        public int DoubleKills { get; set; }

        public int MagicDamageDealt { get; set; }
        public int LargestKillingSpree { get; set; }

        [Required]
        public virtual Champion ChampionUsed { get; set; }
        public string ItemsBought { get; set; }
    }
}
