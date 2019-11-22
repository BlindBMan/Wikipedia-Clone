﻿using System;
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
            var articles = db.Articles;
            ViewBag.Articles = articles;
            return View();
        }

        [HttpGet]
        public ActionResult New()
        {
            Article article = new Article();
            return View(article);
        }

        [HttpPost]
        public ActionResult New(Article article)
        {
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
            return View(article);
        }

        [HttpPut]
        public ActionResult Edit(int id, Article reqArticle)
        {
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
        public ActionResult Delete(int id)
        {
            Article article = db.Articles.Find(id);
            db.Articles.Remove(article);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}