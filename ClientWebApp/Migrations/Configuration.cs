namespace ClientWebApp.Migrations
{
    using ClientWebApp.Infrastructure;
    using ClientWebApp.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ClientWebApp.Infrastructure.AppIdentityDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(AppIdentityDbContext context)
        {
            var userMgr = new AppUserManager(new UserStore<AppUser>(context));
            var roleMgr = new AppRoleManager(new RoleStore<AppRole>(context));

            var roleName = "Administrators";
            var userName = "Admin";
            var password = "mypassword";
            var email = "admin@professorweb.ru";

            if (!roleMgr.RoleExists(roleName))
            {
                roleMgr.Create(new AppRole(roleName));
            }

            var user = userMgr.FindByName(userName);

            if (user == null)
            {
                userMgr.Create(new AppUser { UserName = userName, Email = email },
                    password);
                user = userMgr.FindByName(userName);
            }

            if (user != null)
            {
                if (!userMgr.IsInRole(user.Id, roleName))
                {
                    userMgr.AddToRole(user.Id, roleName);

                    user.FirstName = "Admin";
                    user.IsApproved = true;
                    userMgr.Update(user);
                }
            }

            roleName = "Users";

            if (!roleMgr.RoleExists(roleName))
            {
                roleMgr.Create(new AppRole(roleName));
            }
            context.SaveChanges();
        }
    }
}
