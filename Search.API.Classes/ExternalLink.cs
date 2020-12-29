using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search.API.Classes
{
    public class ExternalLinkParent
    {
        //This is the ID in external_links_parent table
        public int Id { get; set; }
        public string SiteName { get; set; }
        public string SiteUrl { get; set; }
        public bool IsProcessed { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class ExternalLinkChild
    {
        public int Id { get; set; }
        public string ParentId { get; set; }
        public string Url { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
