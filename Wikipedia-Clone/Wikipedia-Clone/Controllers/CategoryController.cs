﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wikipedia_Clone.Models;

namespace Wikipedia_Clone.Controllers
{
    [Authorize(Roles = "God, Admin")]
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

        [HttpGet]
        public ActionResult Show(int id)
        {
            Category category = db.Categories.Find(id);
            return View(category);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);
            return View(category);
        }

        [HttpPut]
        public ActionResult Edit(int id, Category reqCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Category category = db.Categories.Find(id);
                    if (TryUpdateModel(category))
                    {
                        category.CategoryTitle = reqCategory.CategoryTitle;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(reqCategory);
                }
            }
            catch (Exception e)
            {
                return View(reqCategory);
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ShowCategoryArticles(int CategoryId, int orderOption)
        {
            var articles = GetArticles(CategoryId);
            ViewBag.categoryId = CategoryId;
            ViewBag.Articles = articles;

            // orderOption == 1 => order by date, else order by title
            ViewBag.orderOption = orderOption;
            return View();
        }

        [NonAction]
        private ICollection<Article> GetArticles(int CategoryId)
        {
            var list = new List<Article>();
            var articles = from article in db.Articles select article;

            foreach (var article in articles)
            {
                if (article.CategoryId == CategoryId)
                {
                    list.Add(article);
                }
            }

            return list;
        }
    }
}