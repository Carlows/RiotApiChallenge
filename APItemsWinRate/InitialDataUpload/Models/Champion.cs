using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InitialDataUpload.Models
{
    public class Champion
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ChampionId { get; set; }
        public string Name { get; set; }
    }
}
