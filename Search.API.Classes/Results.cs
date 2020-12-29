using Search.API.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Search.API.Classes
{

    public class SearchResults
    {
        //public SearchQuery Query { get; set; }
        public int ResultCount { get; set; }
        public List<Articles> Results { get; set; }
        public string Status { get; set; }
        public double TotalCount { get; set; }
    }
}
