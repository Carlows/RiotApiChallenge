using APItemsWinRate.Infrastructure;
using APItemsWinRate.Models;
using InitialDataUpload.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InitialDataUpload
{
    public class DataAnalysis
    {
        private readonly AppDbContext _context;
        private readonly MatchesDBContext _matchesContext;
        private List<Match> _matches { get; set; }

        public DataAnalysis(AppDbContext context, MatchesDBContext mcontext, List<Match> matches)
        {
            _context = context;
            _matchesContext = mcontext;
            _matches = matches;
        }

        public void CreateRecord()
        {
            try
            {
                Record record = new Record();

                record.Matches = _matches.Count;
                record.Players = _matches.Select(m => m.Players).ToList().Count;

                var matchesData = _matches.ToList();
                var matchesPreChange = matchesData.Where(m => m.Pre_Change == true).ToList();
                var matchesPostChange = matchesData.Where(m => m.Pre_Change == false).ToList();

                Record_Buy_Percentage recordBuy = new Record_Buy_Percentage();

                AllItemsRecord itemsRecord = new AllItemsRecord();

                var listOfItemRecords = new List<ItemRecord>();

                foreach (var key in APItems.Items.Keys)
                {
                    var item = new ItemRecord()
                    {
                        ItemID = APItems.Items[key],
                        PreChangeRecord = matchesPreChange.SelectMany(m => m.Players)
                                            .Where(p => p.ItemsBought
                                            .Contains(APItems.Items[key].ToString()))
                                            .ToList()
                                            .Count,
                        PostChangeRecord = matchesPostChange.SelectMany(m => m.Players)
                                            .Where(p => p.ItemsBought
                                            .Contains(APItems.Items[key].ToString()))
                                            .ToList()
                                            .Count
                    };

                    listOfItemRecords.Add(item);
                }

                itemsRecord.Items = listOfItemRecords;
                recordBuy.ItemsRecord = itemsRecord;
                record.RecordsByBuyPercentage = recordBuy;

                Record_Champions recordChampions = new Record_Champions();

                var listOfChampionRecords = new List<ChampionRecord>();

                var championIds = _context.Champions.Select(c => c.ChampionId).ToList();

                foreach (int championId in championIds)
                {
                    var championRecord = new ChampionRecord();
                    championRecord.ChampionId = championId;

                    var allitemsChampionRecord = new AllItemsRecord();
                    var listRecords = new List<ItemRecord>();

                    foreach (var key in APItems.Items.Keys)
                    {
                        var item = new ItemRecord()
                        {
                            ItemID = APItems.Items[key],
                            PreChangeRecord = matchesPreChange.SelectMany(m => m.Players)
                                            .Where(p => p.ChampionUsed.ChampionId == championId && p.ItemsBought.Contains(APItems.Items[key].ToString()))
                                            .ToList()
                                            .Count,
                            PostChangeRecord = matchesPostChange.SelectMany(m => m.Players)
                                            .Where(p => p.ChampionUsed.ChampionId == championId && p.ItemsBought.Contains(APItems.Items[key].ToString()))
                                            .ToList()
                                            .Count
                        };

                        listRecords.Add(item);
                    }

                    allitemsChampionRecord.Items = listRecords;
                    championRecord.ItemsRecord = allitemsChampionRecord;

                    listOfChampionRecords.Add(championRecord);
                }

                recordChampions.ChampionsRecords = listOfChampionRecords;
                record.RecordsByChampions = recordChampions;

                _context.Records.Add(record);
                _context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        private KDAAverage CalculateKDA(List<Player> players)
        {
            var kda = new KDAAverage();

            kda.Kills = 0;
            kda.Assists = 0;
            kda.Deaths = 0;

            if (players.Count > 0)
            {
                kda.Kills = Convert.ToInt32(players.Select(p => p.Kills).Sum() / (float)players.Count);
                kda.Assists = Convert.ToInt32(players.Select(p => p.Assists).Sum() / (float)players.Count);
                kda.Deaths = Convert.ToInt32(players.Select(p => p.Deaths).Sum() / (float)players.Count);
            }

            return kda;
        }

        private MultiKills CalculateMultiKills(List<Player> players)
        {
            var multikills = new MultiKills();

            multikills.PentaKills = players.Select(p => p.PentaKills).Sum();
            multikills.QuadraKills = players.Select(p => p.QuadraKills).Sum();
            multikills.TripleKills = players.Select(p => p.TripleKills).Sum();
            multikills.DoubleKills = players.Select(p => p.DoubleKills).Sum();

            return multikills;
        }

        private int CalculateWinRate(List<Player> players)
        {
            var playersWon = players.Where(p => p.Winner == true).ToList();

            return players.Count > 0 ? Convert.ToInt32((playersWon.Count / (float)players.Count) * 100f) : 0;
        }

        private List<DataChampion> MostUsedChampions(Record data, bool PreChange, int iditem, List<int> champions)
        {
            List<Tuple<ChampionRecord, float>> dataPerChampion = new List<Tuple<ChampionRecord, float>>();
            foreach (int champId in champions)
            {
                var champRecord = data.RecordsByChampions.ChampionsRecords.Where(r => r.ChampionId == champId).Single();
                var recordItem = champRecord.ItemsRecord.Items.Where(i => i.ItemID == iditem).Single();
                var totalPlayersChamp = _matches.Where(m => m.Pre_Change == PreChange).SelectMany(ma => ma.Players).Where(p => p.ChampionUsed.ChampionId == champId).ToList().Count;

                float percentageUsed = 0;
                if (totalPlayersChamp > 0)
                {
                    if (PreChange)
                    {
                        percentageUsed = (recordItem.PreChangeRecord / (float)totalPlayersChamp) * 100f;
                    }
                    else
                    {
                        percentageUsed = (recordItem.PostChangeRecord / (float)totalPlayersChamp) * 100f;
                    }
                }

                dataPerChampion.Add(new Tuple<ChampionRecord, float>(champRecord, percentageUsed));
            }

            dataPerChampion.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            var mostUsed = dataPerChampion.Take(5).ToList();

            var listMostUsed = mostUsed.Select(m => new DataChampion()
            {
                Name = _context.Champions.Where(c => c.ChampionId == m.Item1.ChampionId).Single().Name,
                Value = m.Item2
            }).ToList();

            return listMostUsed;
        }

        private List<DataChampion> MostDamageChampions(List<Player> players, int iditem, List<int> champions)
        {
            var listAvgDmgPerChampion = new List<DataChampion>();
            foreach (int champId in champions)
            {
                var playersUsedChampion = players.Where(p => p.ChampionUsed.ChampionId == champId).ToList();

                float avgChampionMagicDamage = 0;
                if (playersUsedChampion.Count > 0)
                {
                    avgChampionMagicDamage = playersUsedChampion.Select(p => p.MagicDamageDealt).Sum() / (float)playersUsedChampion.Count;
                }

                var champ = _context.Champions.Where(c => c.ChampionId == champId).Single();
                listAvgDmgPerChampion.Add(new DataChampion()
                    {
                        ChampID = champId.ToString(),
                        Value = avgChampionMagicDamage,
                        Name = champ.Name,
                        Key = champ.Key
                    });
            }

            listAvgDmgPerChampion.Sort((x, y) => y.Value.CompareTo(x.Value));
            var mostDamage = listAvgDmgPerChampion.Take(5).ToList();

            return mostDamage;
        }

        public void CreateCalculatedRecord()
        {
            var calculatedRecord = new CalculatedRecord();
            var dataItemItems = new List<DataItem>();

            var data = _context.Records.OrderByDescending(r => r.DateCreated).First();
            var items = _context.Items.ToList();

            var champions = APChampions.Champions;
            foreach (var key in APItems.Items.Keys)
            {
                var iditem = APItems.Items[key];

                // Usage by champion (Top 5 champions that used this item the most)
                var listMostUsedPreChange = MostUsedChampions(data, true, iditem, champions);
                var listMostUsedPostChange = MostUsedChampions(data, false, iditem, champions);

                var playersThatUsedAPChampionsPreChange = _matches.Where(m => m.Pre_Change == true).SelectMany(ma => ma.Players).Where(p => champions.Contains(p.ChampionUsed.ChampionId)).ToList();
                var playersThatUsedAPChampionsPostChange = _matches.Where(m => m.Pre_Change == false).SelectMany(ma => ma.Players).Where(p => champions.Contains(p.ChampionUsed.ChampionId)).ToList();
                var playersAPChampionsUsedItemPreChange = playersThatUsedAPChampionsPreChange.Where(p => p.ItemsBought.Contains(iditem.ToString())).ToList();
                var playersAPChampionsUsedItemPostChange = playersThatUsedAPChampionsPostChange.Where(p => p.ItemsBought.Contains(iditem.ToString())).ToList();

                // KDA
                var KDAAvgPreChange = CalculateKDA(playersAPChampionsUsedItemPreChange);
                var KDAAvgPostChange = CalculateKDA(playersAPChampionsUsedItemPostChange);

                // Total of Multikills
                var multikillsPreChange = CalculateMultiKills(playersAPChampionsUsedItemPreChange);
                var multikillsPostChange = CalculateMultiKills(playersAPChampionsUsedItemPostChange);

                // Winrate
                var winratePreChange = CalculateWinRate(playersAPChampionsUsedItemPreChange);
                var winratePostChange = CalculateWinRate(playersAPChampionsUsedItemPostChange);

                // Champions who dealt the most magic damage (Top 5)
                var listMostDamagePreChange = MostDamageChampions(playersAPChampionsUsedItemPreChange, iditem, champions);
                var listMostDamagePostChange = MostDamageChampions(playersAPChampionsUsedItemPostChange, iditem, champions);


                var playersThatUsedAPChampionsPreChangeCount = playersThatUsedAPChampionsPreChange.Count;
                var playersThatUsedAPChampionsPostChangeCount = playersThatUsedAPChampionsPostChange.Count;

                var itemRecords = data.RecordsByChampions
                               .ChampionsRecords
                               .Where(r => champions.Contains(r.ChampionId))
                               .Select(c => c.ItemsRecord)
                               .SelectMany(ir => ir.Items)
                               .Where(i => i.ItemID == iditem)
                               .ToList();


                // Usage per rank
                var listUsePerRankPrePatch = new List<DataPerRank>();
                var listUsePerRankPostPatch = new List<DataPerRank>();
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    var playersInRankPreChangeCount = playersThatUsedAPChampionsPreChange.Where(p => p.Rank == rank).ToList().Count;
                    var playersInRankPostChangeCount = playersThatUsedAPChampionsPostChange.Where(p => p.Rank == rank).ToList().Count;
                    var playersThatBoughtThisItemInRankPreChangeCount = playersThatUsedAPChampionsPreChange
                                                                        .Where(p => p.ItemsBought
                                                                        .Contains(APItems.Items[key].ToString()) && p.Rank == rank)
                                                                        .ToList()
                                                                        .Count;
                    var playersThatBoughtThisItemInRankPostChangeCount = playersThatUsedAPChampionsPostChange
                                                                        .Where(p => p.ItemsBought
                                                                        .Contains(APItems.Items[key].ToString()) && p.Rank == rank)
                                                                        .ToList()
                                                                        .Count;

                    DataPerRank dataPrePatch = new DataPerRank()
                    {
                        Rank = rank.ToString(),
                        Data = playersInRankPreChangeCount != 0 ? Convert.ToInt32((playersThatBoughtThisItemInRankPreChangeCount / (float)playersInRankPreChangeCount) * 100f) : 0
                    };

                    DataPerRank dataPostPatch = new DataPerRank()
                    {
                        Rank = rank.ToString(),
                        Data = playersInRankPostChangeCount != 0 ? Convert.ToInt32((playersThatBoughtThisItemInRankPostChangeCount / (float)playersInRankPostChangeCount) * 100f) : 0
                    };

                    listUsePerRankPrePatch.Add(dataPrePatch);
                    listUsePerRankPostPatch.Add(dataPostPatch);
                }

                // Usage per region
                var listUsePerRegionPrePatch = new List<DataPerRegion>();
                var listUsePerRegionPostPatch = new List<DataPerRegion>();
                foreach (Region region in Enum.GetValues(typeof(Region)))
                {
                    var playersInRegionPreChangeCount = playersThatUsedAPChampionsPreChange.Where(p => p.Region == region).ToList().Count;
                    var playersInRegionPostChangeCount = playersThatUsedAPChampionsPostChange.Where(p => p.Region == region).ToList().Count;
                    var playersThatBoughtThisItemInRegionPreChangeCount = playersThatUsedAPChampionsPreChange
                                                                        .Where(p => p.ItemsBought
                                                                        .Contains(APItems.Items[key].ToString()) && p.Region == region)
                                                                        .ToList()
                                                                        .Count;
                    var playersThatBoughtThisItemInRegionPostChangeCount = playersThatUsedAPChampionsPostChange
                                                                        .Where(p => p.ItemsBought
                                                                        .Contains(APItems.Items[key].ToString()) && p.Region == region)
                                                                        .ToList()
                                                                        .Count;

                    DataPerRegion dataPrePatch = new DataPerRegion()
                    {
                        Region = region.ToString(),
                        Data = playersInRegionPreChangeCount != 0 ? Convert.ToInt32((playersThatBoughtThisItemInRegionPreChangeCount / (float)playersInRegionPreChangeCount) * 100f) : 0
                    };

                    DataPerRegion dataPostPatch = new DataPerRegion()
                    {
                        Region = region.ToString(),
                        Data = playersInRegionPostChangeCount != 0 ? Convert.ToInt32((playersThatBoughtThisItemInRegionPostChangeCount / (float)playersInRegionPostChangeCount) * 100f) : 0
                    };

                    listUsePerRegionPrePatch.Add(dataPrePatch);
                    listUsePerRegionPostPatch.Add(dataPostPatch);
                }

                // Total usage
                var item = new ItemRecord()
                {
                    PreChangeRecord = itemRecords.Select(i => i.PreChangeRecord).Sum(),
                    PostChangeRecord = itemRecords.Select(i => i.PostChangeRecord).Sum()
                };

                int percentagePrePatch = Convert.ToInt32((item.PreChangeRecord / (float)playersThatUsedAPChampionsPreChangeCount) * 100f);
                int percentagePostPatch = Convert.ToInt32((item.PostChangeRecord / (float)playersThatUsedAPChampionsPostChangeCount) * 100f);
                string itemName = APItems.ItemsNames[iditem];
                dataItemItems.Add(new DataItem
                {
                    ItemId = iditem,
                    ItemName = itemName,
                    PrePatch = percentagePrePatch,
                    PostPatch = percentagePostPatch,
                    MostUsedChampionsPrePatch = listMostUsedPreChange,
                    MostUsedChampionsPostPatch = listMostUsedPostChange,
                    DataPerRankPrePatch = listUsePerRankPrePatch,
                    DataPerRankPostPatch = listUsePerRankPostPatch,
                    DataPerRegionPrePatch = listUsePerRegionPrePatch,
                    DataPerRegionPostPatch = listUsePerRegionPostPatch,
                    KDAAvgPrePatch = KDAAvgPreChange,
                    KDAAvgPostPatch = KDAAvgPostChange,
                    MultiKillsPrePatch = multikillsPreChange,
                    MultiKillsPostPatch = multikillsPostChange,
                    WinRatePrePatch = winratePreChange,
                    WinRatePostPatch = winratePostChange,
                    ChampionsWithMoreMagicDamagePrePatch = listMostDamagePreChange,
                    ChampionsWithMoreMagicDamagePostPatch = listMostDamagePostChange
                });
            }

            calculatedRecord.Items = dataItemItems;
            _context.CalculatedRecord.Add(calculatedRecord);
            _context.SaveChanges();
        }
    }
}
