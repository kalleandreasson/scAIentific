using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Data
{
    public class AbstractModel
    {
        public int Id {get; set;}
        public string? ArticleId {get; set;}
        public string? Author {get; set;}
        public string? Title {get; set;}
        public string? Year {get; set;}
        public string? Abstract {get; set;}
        public string? FullReference {get; set;}
        public string? Notes {get; set;}
    }
}