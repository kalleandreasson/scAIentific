using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Data
{
    public class ResearchRegistration
    {
        [StringLength(50, ErrorMessage = "The NAme is too short")]
        [Required]
        public string ResearcherFullName {get; set;}
        [Required]
        public string ResearcherEmailAdress {get; set;}
        [Required]
        public string ResearchName {get; set;}

    }
}