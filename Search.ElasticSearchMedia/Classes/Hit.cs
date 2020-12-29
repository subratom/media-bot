using Search.API.Classes;
using System.Collections.Generic;

namespace Search.ElasticSearchMedia
{
    public class Hit
    {
        public string Index { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
        public double? Score { get; set; }
        public Articles Source { get; set; }
    }

    public class SearchHits
    {
        public List<Hit> Hits { get; set; }
        public double TotalItems { get; set; }
    }
}
