using APItemsWinRate.Infrastructure;
using APItemsWinRate.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace APItemsWinRate.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            AppDbContext context = new AppDbContext();
            
            var model = context.Champions.ToList();
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}