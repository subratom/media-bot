using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Search.RelatedContent.Models
{
    public class KeywordQuery
    {
        [Required]
        public string QueryText { get; set; }
        [Required]
        [Range(5, 25)]
        [System.ComponentModel.DefaultValue(5)]
        public int MaxCount { get; set; }
    }

    public class UrlQuery
    {
        [Required]
        public string Url { get; set; }
        [System.ComponentModel.DefaultValue(5)]
        [Required]
        [Range(5, 25)]
        public int MaxCount { get; set; }
    }

    public class PartsQuery
    {
        [Required]
        public string PartFamily { get; set; }
        [System.ComponentModel.DefaultValue(null)]
        public string[] AdditionalKeywords { get; set; } = null;
        [Required]
        [Range(5, 25)]
        [System.ComponentModel.DefaultValue(5)]
        public int MaxCount { get; set; }
    }
}