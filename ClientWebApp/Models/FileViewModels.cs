using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClientWebApp.Models
{
    public class FileCreateModel
    {
        public string AuthorId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public HttpPostedFileBase File { get; set; }
    }
}