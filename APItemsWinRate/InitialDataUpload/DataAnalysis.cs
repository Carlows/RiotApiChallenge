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

        public DataAnalysis(AppDbContext context)
        {
            _context = context;
        }

        public void CreateRecord()
        {           
            try
            {
                Record record = new Record();

                record.Matches = _context.Matches.ToList().Count;
                record.Players = _context.Participants.ToList().Count;

                var matchesData = _context.Matches.ToList();
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
    }
}
