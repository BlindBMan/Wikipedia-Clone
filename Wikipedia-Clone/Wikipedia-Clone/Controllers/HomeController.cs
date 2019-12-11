using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wikipedia_Clone.Models;

namespace Wikipedia_Clone.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();

        public ActionResult Index()
        {
            // var numOfArticles = 4;
            // var articles = db.Articles.Include("Category").Include("User").OrderByDescending(m => m.Date).Take(numOfArticles);
            ViewBag.Articles = from category in db.Categories
                               select category.Articles.OrderByDescending(m => m.Date).FirstOrDefault();
            return View();
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