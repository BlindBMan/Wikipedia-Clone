using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using Wikipedia_Clone.Models;

[assembly: OwinStartupAttribute(typeof(Wikipedia_Clone.Startup))]
namespace Wikipedia_Clone
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            // createRoles();
        }

        private void createRoles()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // Add God role 
            if (!RoleManager.RoleExists("God"))
            {
                var role = new IdentityRole();
                role.Name = "God";
                RoleManager.Create(role);

                // Add only one initial god account
                var user = new ApplicationUser();
                user.UserName = "god@god.com";
                user.Email = "god@god.com";
                var godCreated = UserManager.Create(user, "Sarajevo#1914");
                if (godCreated.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "God");
                }
            }

            if (!RoleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole();
                role.Name = "Admin";
                RoleManager.Create(role);
            }

            if (!RoleManager.RoleExists("Editor"))
            {
                var role = new IdentityRole();
                role.Name = "Editor";
                RoleManager.Create(role);
            }

            if (!RoleManager.RoleExists("User"))
            {
                var role = new IdentityRole();
                role.Name = "User";
                RoleManager.Create(role);
            }
        }
    }
}
