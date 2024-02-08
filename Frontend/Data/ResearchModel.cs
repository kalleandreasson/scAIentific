using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Data
{
    public class ResearchModel
    {
        public int Id {get; set;}
        public string? Title {get; set;}
        public string? ResearcherFullName {get; set;}
        public string? FileName {get; set;}
    }
}