using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Search.API.Classes
{
    //public class Query
    //{
    //    public string Url { get; set; }
    //    public string QueryText { get; set; }
    //    public string PartFamily { get; set; }
    //}

    //Need to modify this. Already exists in Search.API. Need to simply class structure.
    //public class SearchQuery
    //{
    //    [System.ComponentModel.DefaultValue("")]
    //    public string Url { get; set; }
    //    [System.ComponentModel.DefaultValue("")]
    //    public string QueryText { get; set; }
    //    [System.ComponentModel.DefaultValue("")]
    //    public string PartFamily { get; set; }
    //    [System.ComponentModel.DefaultValue(null)]
    //    public string[] AdditionalKeywords { get; set; }
    //    [System.ComponentModel.DefaultValue(5)]
    //    public int MaxCount { get; set; }
    //}

    public class KeywordQuery
    {
        [Required]
        public string QueryText { get; set; }

        [DefaultValue(0)]
        public int PageNumber { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        //[Range(5, 25)]
        [DefaultValue(5)]
        public int MaxCount { get; set; }

        [DefaultValue("")]
        public string SiteName { get; set; }
    }


    public class ContentQuery
    {
        [DefaultValue(5)]
        [Range(1, 25)]
        public int MaxCount { get; set; }

        [DefaultValue(0)]
        public int PageNumber { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [DefaultValue("")]
        public string SiteName { get; set; }
    }

    public class UrlQuery
    {
        [Required]
        public string Url { get; set; }

        [DefaultValue(5)]
        [Range(5, 25)]
        public int MaxCount { get; set; }

        [DefaultValue(0)]
        public int PageNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class PartsQuery
    {
        [Required]
        public string PartFamily { get; set; }
        [DefaultValue(null)]
        public string[] AdditionalKeywords { get; set; } = null;

        [Range(5, 25)]
        [DefaultValue(5)]
        public int MaxCount { get; set; }

        [DefaultValue(0)]
        public int PageNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
