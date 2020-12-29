using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Search.RelatedContent.Models
{
    public class Items
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public string[] PartNumbers { get; set; }
        public DateTime? Pubdate { get; set; }
    }

    public class UrlResults
    {
        public UrlQuery Query { get; set; }
        public int ResultCount { get; set; }
        public List<Items> Results { get; set; }
    }

    public class PartsResults
    {
        public PartsQuery Query { get; set; }
        public int ResultCount { get; set; }
        public List<Items> Results { get; set; }
    }

    public class KeywordResults
    {
        public KeywordQuery Query { get; set; }
        public int ResultCount { get; set; }
        public List<Items> Results { get; set; }
    }
}