using System;
using System.Net;

namespace Search.MediaStore.DDL
{
    public class Mappings
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string SiteName { get; set; }
        public bool IsPartProcessed { get; set; }
        public bool IsUrlProcessed { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public int UrlMaxTries { get; set; }
        public int PartMaxTries { get; set; }
        public HttpStatusCode HttpCode { get; set; }
        public bool UpdatedUrl { get; set; }
    }

    public class ParentUrls
    {
        public string Urls { get; set; }
        public string SiteName { get; set; }
        public bool UrlProcessed { get; set; }
        public DateTime ProcessedDate { get; set; }
    }

    public class ChildUrls
    {
        public int ParentId { get; set; }
        public string Url { get; set; }
        public DateTime ProcessedDate { get; set; }
    }
}
