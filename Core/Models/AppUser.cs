using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
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
        public string SecondName { get; set; }
        public string Patronymic { get; set; }
        public Genders Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string Workplace { get; set; }
        public string JobPosition { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public bool IsApproved { get; set; }
        public string RawPassword { get; set; }
        public int SharePointId { get; set; }
    }
}
