using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Models
{
    public class UserRegistrationModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
    }
}