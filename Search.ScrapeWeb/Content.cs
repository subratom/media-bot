using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Search.Scrapper
{
    public class Content
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        public string RawBodyContents { get; set; }
        public string Description { get; set; }
        public string PubDate { get; set; }
        public string RawResponse { get; set; }
        public HttpStatusCode Code { get; set; }

    }
}
