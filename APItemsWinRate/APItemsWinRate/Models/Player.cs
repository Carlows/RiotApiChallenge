using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APItemsWinRate.Models
{
    public class Player
    {
        public Player()
        {
        }

        public int Id { get; set; }

        public Rank Rank { get; set; }
        public bool Winner { get; set; }

        [Required]
        public virtual Champion ChampionUsed { get; set; }
        public string ItemsBought { get; set; }
    }
}
