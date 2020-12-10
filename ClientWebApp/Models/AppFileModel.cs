using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ClientWebApp.Models
{
    public class AppFileModel
    {
        public int Id { get; set; }
        public string AuthorId { get; set; }
        public string Name { get; set; }
        public byte[] File { get; set; }
        public int SharePointId { get; set; }
    }
}