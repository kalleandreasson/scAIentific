using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Data
{
    public class ResearchRegistration
    {
        public string ResearcherFullName {get; set;}
        public string ResearcherEmailAdress {get; set;}
        [Required]
        public string ResearchName {get; set;}

    }
}