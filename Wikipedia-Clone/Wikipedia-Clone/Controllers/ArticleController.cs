using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wikipedia_Clone.Models;

namespace Wikipedia_Clone.Controllers
{
    public class ArticleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index()
        {
            var articles = db.Articles.Include("Category");
            ViewBag.Articles = articles;
            return View();
        }

        [HttpGet]
        public ActionResult New()
        {
            Article article = new Article();
            article.Categories = GetCategories();
            return View(article);
        }

        [HttpPost]
        public ActionResult New(Article article)
        {
            article.Categories = GetCategories();

            try
            {
                if (ModelState.IsValid)
                {
                    db.Articles.Add(article);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(article);
                }
            }
            catch (Exception e)
            {
                return View(article);
            }
        }

        [HttpGet]
        public ActionResult Show(int id)
        {
            Article article = db.Articles.Find(id);
            return View(article);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Article article = db.Articles.Find(id);
            article.Categories = GetCategories();
            return View(article);
        }

        [HttpPut]
        public ActionResult Edit(int id, Article reqArticle)
        {
            reqArticle.Categories = GetCategories();

            try
            {
                if (ModelState.IsValid)
                {
                    Article article = db.Articles.Find(id);

                    if (TryUpdateModel(article))
                    {
                        article.Content = reqArticle.Content;
                        article.Title = reqArticle.Title;
                        article.Date = reqArticle.Date;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(reqArticle);
                }
            }
            catch (Exception e)
            {
                return View(reqArticle);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "God, Admin")]
        public ActionResult Delete(int id)
        {
            Article article = db.Articles.Find(id);
            db.Articles.Remove(article);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetCategories()
        {
            var list = new List<SelectListItem>();
            var categories = from cat in db.Categories select cat;

            foreach (var cat in categories)
            {
                list.Add(new SelectListItem
                {
                    Value = cat.CategoryId.ToString(),
                    Text = cat.CategoryTitle.ToString()
                });
            }

            return list;
        }
    }
}