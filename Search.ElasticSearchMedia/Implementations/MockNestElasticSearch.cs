using Search.ElasticSearchMedia.Interfaces;
using System;
using Nest;

namespace Search.ElasticSearchMedia.Implementations
{
    public class MockNestElasticSearch : INestElasticSearch
    {
        protected Uri local = null;
        protected ConnectionSettings settings = null;
        protected ElasticClient elastic = null;
        protected string mediaIndexName = "media2"; // "media2";
        protected string mediaTypeName = "articles";

        public ElasticClient ConnectElasticSearch()
        {
            local = new Uri("http://lb-4do3bjdwej5x2.eastus.cloudapp.azure.com:9200");
            settings = new ConnectionSettings(local);
            settings.DisableDirectStreaming(true);
            elastic = new ElasticClient(settings);
            return elastic;
        }
    }
}
