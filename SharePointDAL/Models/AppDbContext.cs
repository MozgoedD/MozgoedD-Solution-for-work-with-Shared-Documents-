using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointDAL.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=MvcDatabase") { }

        public DbSet<AppFileModel> Files { get; set; }
    }
}
