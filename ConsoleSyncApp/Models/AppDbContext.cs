using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Entity;

namespace ConsoleSyncApp.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=MvcDatabase") { }

        public DbSet<AppFileModel> Files { get; set; }
    }
}
