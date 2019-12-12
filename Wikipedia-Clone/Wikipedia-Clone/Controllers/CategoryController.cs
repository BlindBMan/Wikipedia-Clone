using System;
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

        private int _perPage = 3;

        [HttpGet]
        public ActionResult Index()
        {
            var categories = db.Categories;

            // categories pagination
            ViewBag.PerPage = _perPage;

            int lastPage = categories.Count() / _perPage;

            ViewBag.LastPage = (categories.Count() % _perPage == 0) ? lastPage : lastPage + 1;
            ViewBag.CategoryCount = categories.Count();

            int offset, currPage;

            // get the current requested page
            string pageParam;
            if ((pageParam = Request.Params.Get("Page")) != null)
            {
                try
                {
                    // if the parameter format is wrong it could crash the application
                    currPage = Convert.ToInt32(pageParam);
                }
                catch (Exception)
                {
                    currPage = 1;
                }
            }
            else
            {
                // if no parameter is provided, we are showing the first page
                currPage = 1;
            }

            offset = (currPage - 1) * _perPage;

            ViewBag.Categories = categories.OrderBy(c => c.CategoryTitle).Skip(offset).Take(_perPage);

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
            ViewBag.CategoryTitle = db.Categories.Find(CategoryId).CategoryTitle;


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