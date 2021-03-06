﻿using APItemsWinRate.Infrastructure;
using APItemsWinRate.Models;
using APItemsWinRate.Models.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace APItemsWinRate.Controllers
{
    public class StatsByItemController : Controller
    {
        private readonly AppDbContext _context;

        public StatsByItemController(AppDbContext context)
        {
            _context = context;
        }

        // GET: StatsByItem
        public ActionResult Index()
        {
            var model = new DataByItemViewModel();
            var data = _context.CalculatedRecord.OrderByDescending(r => r.DateCreated).FirstOrDefault();
            var items = _context.Items.ToList();
            
            model.Items = data.Items.Select(i => new DataItemViewModel()
            {
                ItemId = i.ItemId,
                ItemName = i.ItemName,
                PrePatch = i.PrePatch,
                PostPatch = i.PostPatch,
                MostUsedChampionsPrePatchData = i.MostUsedChampionsPrePatch.Select(c => Convert.ToInt32(c.Value)).ToArray(),
                MostUsedChampionsPrePatchLabels = i.MostUsedChampionsPrePatch.Select(c => c.Name).ToArray(),
                MostUsedChampionsPostPatchData = i.MostUsedChampionsPostPatch.Select(c => Convert.ToInt32(c.Value)).ToArray(),
                MostUsedChampionsPostPatchLabels = i.MostUsedChampionsPostPatch.Select(c => c.Name).ToArray(),
                ItemDataByRankPrePatchLabels = i.DataPerRankPrePatch.Select(d => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(d.Rank.ToLower())).ToArray(),
                ItemDataByRankPrePatchData = i.DataPerRankPrePatch.Select(d => d.Data).ToArray(),
                ItemDataByRankPostPatchLabels = i.DataPerRankPostPatch.Select(d => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(d.Rank.ToLower())).ToArray(),
                ItemDataByRankPostPatchData = i.DataPerRankPostPatch.Select(d => d.Data).ToArray(),
                ItemDataByRegionPrePatchLabels = i.DataPerRegionPrePatch.Select(d => d.Region).ToArray(),
                ItemDataByRegionPrePatchData = i.DataPerRegionPrePatch.Select(d => d.Data).ToArray(),
                ItemDataByRegionPostPatchLabels = i.DataPerRegionPostPatch.Select(d => d.Region).ToArray(),
                ItemDataByRegionPostPatchData = i.DataPerRegionPostPatch.Select(d => d.Data).ToArray(),
                KDAAvgPrePatch = i.KDAAvgPrePatch,
                KDAAvgPostPatch = i.KDAAvgPostPatch,
                MultiKillsPrePatch = i.MultiKillsPrePatch,
                MultiKillsPostPatch = i.MultiKillsPostPatch,
                WinRatePrePatch = i.WinRatePrePatch,
                WinRatePostPatch = i.WinRatePostPatch,
                ChampionsMostDmgPrePatch = i.ChampionsWithMoreMagicDamagePrePatch,
                ChampionsMostDmgPostPatch = i.ChampionsWithMoreMagicDamagePostPatch,
                Data = items.Where(item => item.ItemId == i.ItemId).Select(item => new ItemDataViewModel()
                {
                    Id = item.Id,
                    ItemId = item.ItemId,
                    DataPreChange = new DataViewModel()
                    {
                        Id = item.DataPreChange.Id,
                        Description = item.DataPreChange.Description,
                        From = item.DataPreChange.From.Split(','),
                        Gold = item.DataPreChange.Gold,
                        Name = item.DataPreChange.Name,
                        Plaintext = item.DataPreChange.Plaintext
                    },
                    DataPostChange = new DataViewModel()
                    {
                        Id = item.DataPostChange.Id,
                        Description = item.DataPostChange.Description,
                        From = item.DataPostChange.From.Split(','),
                        Gold = item.DataPostChange.Gold,
                        Name = item.DataPostChange.Name,
                        Plaintext = item.DataPostChange.Plaintext
                    }
                }).Single()                
            }).ToList();

            return View(model);
        }
    }
}