using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Models
{
    public class ResearchModel
    {
        public string? ArticleID { get; set; }
        public string? Authors { get; set; }
        public string? Title { get; set; }
        public int Year { get; set; }
        public string? Abstract { get; set; }
        public string? FullReference { get; set; }
        public string? Notes { get; set; }
    }
}