using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wikipedia_Clone.Models;

namespace Wikipedia_Clone.Controllers
{
    [Authorize(Roles = "God, Admin")]
    public class UserController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();

        private int _perPage = 3;

        [HttpGet]
        public ActionResult Index()
        {
            var users = from user in db.Users
                        orderby user.UserName
                        select user;

            // users pagination
            ViewBag.PerPage = _perPage;

            int lastPage = users.Count() / _perPage;

            ViewBag.LastPage = (users.Count() % _perPage == 0) ? lastPage : lastPage + 1;
            ViewBag.UsersCount = users.Count();

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

            ViewBag.Users = users.OrderBy(u => u.UserName).Skip(offset).Take(_perPage);
            return View();
        }

        [HttpGet]
        public ActionResult Show(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            return View(user);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetRoles();
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;
            return View(user);
        }

        [HttpPut]
        public ActionResult Edit(string id, ApplicationUser reqUser)
        {
            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetRoles();
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;

            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                if (TryUpdateModel(user))
                {
                    user.UserName = reqUser.UserName;
                    user.Email = reqUser.Email;
                    
                    var roles = from role in db.Roles select role;
                    foreach (var role in roles)
                    {
                        UserManager.RemoveFromRole(id, role.Name);
                    }

                    var selectedRole = db.Roles.Find(HttpContext.Request.Params.Get("newRole"));
                    UserManager.AddToRole(id, selectedRole.Name);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View(user);
            }
        }

        [HttpDelete]
        public ActionResult Delete(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = from role in db.Roles 
                        select role;
            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }

            return selectList;
        }
    }
}