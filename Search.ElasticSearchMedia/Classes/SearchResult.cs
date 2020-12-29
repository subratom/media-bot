using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search.ElasticSearchMedia
{
    public class SearchResult
    {
        public long Total { get; set; } // searchResponse.HitsMetaData.Total;
        public double MaxScore { get; set; } // searchResponse.HitsMetaData.MaxScore;
        public List<Hit> hits { get; set; }
    }
}
