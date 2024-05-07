using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Models
{
    public class UserResearch
    {
        // public string? userId { get; set; }
        [Required]
        public string? ResearchArea { get; set; }
        
    }
}
    