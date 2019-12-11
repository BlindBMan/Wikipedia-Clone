using LocalProject.Models;
using Microsoft.AspNet.Identity;
using Nest;
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

        private ElasticClient client = new ElasticClient(
            new ConnectionSettings().DefaultIndex("article")
        );
        
        private int _perPage = 3;

        [HttpGet]
        public ActionResult Index()
        {
            // if a search phrase has been specified use elasticSearch to get the ids of the found articles
            List<int> searchResultedIds = new List<int>();

            string searchPhrase = Request.Params.Get("SearchPhrase");

            // if a search phrase has been mentioned
            if (searchPhrase != null)
            {
                // search using the provided phrase
                var searchResponse = client.Search<ElasticArticle>(s => s
                    // get a partial result => we need only the id of the found articles
                    .Source(sf => sf
                        .Includes(i => i
                            .Fields(f => f.Id)
                        )
                    )
                    // query for the wanted articles that contain the searchPhrase in the title or the content
                    .Query(q => q
                        .MatchPhrase(m => m
                            .Field(f => f.Title)
                            .Query(searchPhrase)
                        ) || q
                        .MatchPhrase(m => m
                            .Field(f => f.Content)
                            .Query(searchPhrase)
                       )
                    )
                );

                var partialDocuments = searchResponse.Documents;

                foreach (var partialDocument in partialDocuments)
                {
                    searchResultedIds.Add(partialDocument.Id);
                }
            }

            var articles = db.Articles.Include("Category").Include("User").OrderBy(a => a.Date);
            IQueryable<Article> paginatedArticles;

            // keep count how many articles per page there are
            ViewBag.PerPage = _perPage;

            // if a search phrase was specified, we have to filter and keep only the found articles
            if (searchPhrase != null)
            {
                paginatedArticles = articles.Where(a => searchResultedIds.Contains(a.Id));

                // also include the search phrase so it is not lost from the html search box input
                ViewBag.SearchPhrase = searchPhrase;
            }
            else
            {
                paginatedArticles = articles;
            }

            // data needed for pagination

            int lastPage = paginatedArticles.Count() / _perPage;

            ViewBag.LastPage = (paginatedArticles.Count() % _perPage == 0) ? lastPage : lastPage + 1;
            ViewBag.ArticleCount = paginatedArticles.Count();

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
                catch(Exception)
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

            // show only the articles on this page
            ViewBag.Articles = paginatedArticles.Skip(offset).Take(_perPage);

            return View();
        }

        [HttpGet]
        public ActionResult New()
        {
            Article article = new Article
            {
                Categories = GetCategories(),
                LastContent = null
            };

            return View(article);
        }

        [HttpPost]
        public ActionResult New(Article article)
        {
            article.Categories = GetCategories();
            article.UserId = User.Identity.GetUserId();

            try
            {
                if (ModelState.IsValid)
                {
                    db.Articles.Add(article);
                    db.SaveChanges();

                    // after creating a new article, make an elasticArticle representation and send it
                    // to the elasticSearch server, to be indexed
                    if (!IndexElasticArticle(article))
                    {
                        // if the indexing fails
                        TempData["Message"] = "Your article could not be properly indexed. It might not show up in searches.";
                    }
                    else
                    {
                        TempData["Message"] = "Your article was successfully created.";
                    }

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

            article.LastContent = article.Content;

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
                        article.LastContent = reqArticle.LastContent;
                        article.Content = reqArticle.Content;
                        article.Title = reqArticle.Title;
                        article.Date = reqArticle.Date;
                        article.Protected = reqArticle.Protected;
                        db.SaveChanges();

                        // after editing an article, make an elasticArticle representation and send it
                        // to the elasticSearch server, to be indexed for searches
                        if (!IndexElasticArticle(article))
                        {
                            // if the indexing fails
                            TempData["Message"] = "Your article could not be properly indexed. It might not show up in searches.";
                        }
                        else
                        {
                            TempData["Message"] = "Your article was successfully edited.";
                        }
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

        [HttpGet]
        public ActionResult ShowChanges(int id)
        {
            var article = db.Articles.Find(id);

            return View(article);
        }

        [HttpPut]
        [Authorize(Roles = "Editor, Admin, God")]
        public ActionResult Revert(int id, Article reqArticle)
        {
            try
            {
                Article article = db.Articles.Find(id);

                if (TryUpdateModel(article))
                {
                    article.Content = reqArticle.Content;
                    article.LastContent = null;
                    article.Date = reqArticle.Date;
                    db.SaveChanges();

                    // after reverting an article, make an elasticArticle representation and send it
                    // to the elasticSearch server, to be indexed for searches
                    if (!IndexElasticArticle(article))
                    {
                        // if the indexing fails
                        TempData["Message"] = "Your article could not be properly indexed. It might not show up in searches.";
                    }
                    else
                    {
                        TempData["Message"] = "Article revert was successful!";
                    }
                    
                }

                return RedirectToAction("Index");
            }
            catch(Exception)
            { 
                TempData["Message"] = "Something went wrong while trying to revert the article's changes.";

                return RedirectToAction("Index");
            }
        }

        [HttpDelete]
        [Authorize(Roles = "God, Admin")]
        public ActionResult Delete(int id)
        {
            Article article = db.Articles.Find(id);
            db.Articles.Remove(article);
            db.SaveChanges();

            // after removing the article from the db, issue a delete request to the ElasticSearch engine
            client.Delete<ElasticArticle>(id);

            TempData["Message"] = "Your article has been successfully deleted.";

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

        // make an elasticArticle representation and send it to the elasticSearch server, to be indexed
        [NonAction]
        public bool IndexElasticArticle(Article article)
        {
            ElasticArticle elasticArticle = new ElasticArticle
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                Date = article.Date.ToString(),
                CategoryName = article.Category.CategoryTitle
            };

            var indexResponse = client.IndexDocument(elasticArticle);

            if (!indexResponse.IsValid)
            {
                // the indexing of the article failed
                return false;
            }

            // index was succesful
            return true;
        }
    }
}