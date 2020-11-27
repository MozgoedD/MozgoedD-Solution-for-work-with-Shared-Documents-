using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClientWebApp.Models
{
    public enum Genders
    {
        Male,
        Female,
        Other
    }
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string SecondtName { get; set; }
        public string Patronymic { get; set; }
        public Genders Gender { get; set; }
        public string DOB { get; set; }
        public string PlaceOfWork { get; set; }
        public string JobPosition { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public bool IsApprove { get; set; }
        public string RawPassword { get; set; }
    }
}