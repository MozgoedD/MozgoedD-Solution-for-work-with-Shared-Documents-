using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClientWebApp.Models
{
    public class CreateModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string SecondName { get; set; }
        public string Patronymic { get; set; }
        public Genders Gender { get; set; }
        public string DOB { get; set; }
        public string PlaceOfWork { get; set; }
        public string JobPosition { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
    }
}