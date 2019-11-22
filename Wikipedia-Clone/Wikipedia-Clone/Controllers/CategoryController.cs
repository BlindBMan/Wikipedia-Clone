using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wikipedia_Clone.Models;

namespace Wikipedia_Clone.Controllers
{
    public class CategoryController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        
        [HttpGet]
        public ActionResult Index()
        {
            var categories = db.Categories;
            ViewBag.Categories = categories;
            return View();
        }

        [HttpGet]
        public ActionResult New()
        {
            Category category = new Category();
            return View(category);
        }

        [HttpPost]
        public ActionResult New(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Categories.Add(category);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(category);
                }
            }
            catch (Exception e)
            {
                return View(category);
            }
        }


    }
}