using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Models
{
    public class UserRegistrationModel
    {
        // [StringLength(MinimumLength = 3, ErrorMessage = "The name is too short")]
        public string Username { get; set; }
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
    }
}