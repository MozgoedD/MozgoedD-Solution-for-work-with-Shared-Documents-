using ClientWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ClientWebApp.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=MvcDatabase") { }

        public DbSet<AppFileModel> Files { get; set; }
    }
}