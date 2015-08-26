using APItemsWinRate.Infrastructure;
using APItemsWinRate.Models;
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
        private List<Match> _matches { get; set; }

        public DataAnalysis(AppDbContext context, List<Match> matches)
        {
            _context = context;
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
                               
                List<Tuple<ChampionRecord, float, float>> dataPerChampion = new List<Tuple<ChampionRecord, float, float>>();
                foreach (int champId in champions)
                {
                    var champRecord = data.RecordsByChampions.ChampionsRecords.Where(r => r.ChampionId == champId).Single();
                    var recordItem = champRecord.ItemsRecord.Items.Where(i => i.ItemID == iditem).Single();
                    var totalPlayersChampPreChange = _matches.Where(m => m.Pre_Change == true).SelectMany(ma => ma.Players).Where(p => p.ChampionUsed.ChampionId == champId).ToList().Count;
                    var totalPlayersChampPostChange = _matches.Where(m => m.Pre_Change == false).SelectMany(ma => ma.Players).Where(p => p.ChampionUsed.ChampionId == champId).ToList().Count;

                    float percentageUsedPreChange = 0;
                    float percentageUsedPostChange = 0;
                    if (totalPlayersChampPreChange > 0)
                        percentageUsedPreChange = (recordItem.PreChangeRecord / (float)totalPlayersChampPreChange) * 100f;

                    if (totalPlayersChampPostChange > 0)
                        percentageUsedPostChange = (recordItem.PostChangeRecord / (float)totalPlayersChampPostChange) * 100f;

                    dataPerChampion.Add(new Tuple<ChampionRecord, float, float>(champRecord, percentageUsedPreChange, percentageUsedPostChange));
                }

                dataPerChampion.Sort((x, y) => y.Item2.CompareTo(x.Item2));
                var mostUsedPreChange = dataPerChampion.Take(5).ToList();
                dataPerChampion.Sort((x, y) => y.Item3.CompareTo(x.Item3));
                var mostUsedPostChange = dataPerChampion.Take(5).ToList();

                var listMostUsedPreChange = mostUsedPreChange.Select(m => new DataChampion()
                {
                    Name = _context.Champions.Where(c => c.ChampionId == m.Item1.ChampionId).Single().Name,
                    Value = m.Item2
                }).ToList();

                var listMostUsedPostChange = mostUsedPostChange.Select(m => new DataChampion()
                {
                    Name = _context.Champions.Where(c => c.ChampionId == m.Item1.ChampionId).Single().Name,
                    Value = m.Item3
                }).ToList();

                var playersThatUsedAPChampionsPreChange = _matches.Where(m => m.Pre_Change == true).SelectMany(ma => ma.Players).Where(p => champions.Contains(p.ChampionUsed.ChampionId)).ToList();
                var playersThatUsedAPChampionsPostChange = _matches.Where(m => m.Pre_Change == false).SelectMany(ma => ma.Players).Where(p => champions.Contains(p.ChampionUsed.ChampionId)).ToList();
                var playersThatUsedAPChampionsPreChangeCount = playersThatUsedAPChampionsPreChange.Count;
                var playersThatUsedAPChampionsPostChangeCount = playersThatUsedAPChampionsPostChange.Count;

                 var itemRecords = data.RecordsByChampions
                                .ChampionsRecords
                                .Where(r => champions.Contains(r.ChampionId))
                                .Select(c => c.ItemsRecord)
                                .SelectMany(ir => ir.Items)
                                .Where(i => i.ItemID == iditem)
                                .ToList();


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
                        Data = playersInRankPreChangeCount != 0 ? Convert.ToInt32((playersThatBoughtThisItemInRankPreChangeCount /(float)playersInRankPreChangeCount) * 100f) : 0
                    };

                    DataPerRank dataPostPatch = new DataPerRank()
                    {
                        Rank = rank.ToString(),
                        Data = playersInRankPostChangeCount != 0 ? Convert.ToInt32((playersThatBoughtThisItemInRankPostChangeCount / (float)playersInRankPostChangeCount) * 100f) : 0
                    };

                    listUsePerRankPrePatch.Add(dataPrePatch);
                    listUsePerRankPostPatch.Add(dataPostPatch);
                }

               
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
                    DataPerRankPostPatch = listUsePerRankPostPatch
                });
            }

            calculatedRecord.Items = dataItemItems;
            _context.CalculatedRecord.Add(calculatedRecord);
            _context.SaveChanges();
        }
    }
}
