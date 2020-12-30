using Core.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext() : base("name=Task2SolutionDb") { }

        static AppDbContext()
        {
            Database.SetInitializer<AppDbContext>(new IdentityDbInit());
        }

        public static AppDbContext Create()
        {
            return new AppDbContext();
        }

        public class IdentityDbInit : NullDatabaseInitializer<AppDbContext> { }

        public DbSet<AppFileModel> Files { get; set; }
    }
}
