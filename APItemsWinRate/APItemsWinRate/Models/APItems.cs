using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APItemsWinRate.Models
{
    public class APItems
    {
        public static Dictionary<string, int> Items = new Dictionary<string, int>
        {
            { "Rabadons", 3089 },
            { "Zhonyas", 3157 },
            { "Ludens", 3285 },
            { "Rylais", 3116 },
            { "Seraphs", 3040 },
            { "RodofAges", 3027 },
            { "Liandrys", 3151 },
            { "Void", 3135 },
            { "Nashors", 3115 },
            { "Ancients", 3152},
            { "Morellonomicon", 3165 },
            { "Athenes", 3174 }
        };

        public static Dictionary<int, string> ItemsNames = new Dictionary<int, string>
        {
            { 3089, "Rabadon's Deathcap" },
            { 3157, "Zhonya's Hourglass" },
            { 3285, "Luden's Echo" },
            { 3116, "Rylai's Scepter" },
            { 3040, "Seraph's Embrace" },
            { 3027, "Rod of Ages" },
            { 3151, "Liandry's Torment" },
            { 3135, "Void Staff" },
            { 3115, "Nashor's Tooth" },
            { 3152, "Will of the Ancients"},
            { 3165, "Morellonomicon" },
            { 3174, "Athene's Unholy Grail" }
        };
    }
}