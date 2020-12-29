using Nest;
using Search.ElasticSearchMedia.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search.ElasticSearchMedia.Implementations
{
    public abstract class NestElasticSearch : INestElasticSearch
    {
        protected Uri local = null;
        protected ConnectionSettings settings = null;
        protected ElasticClient elastic = null;
        protected string mediaIndexName = "media"; // "media2";
        protected string mediaTypeName = "articles";


        public NestElasticSearch()
        {
            local = new Uri("http://lb-4do3bjdwej5x2.eastus.cloudapp.azure.com:9200"); // Uri(ConfigurationManager.AppSettings["elastic-search-url"].ToString() + ":" + ConfigurationManager.AppSettings["port"].ToString());
            settings = new ConnectionSettings(local);
            settings.DisableDirectStreaming(true);
            elastic = new ElasticClient(settings);
        }
    }
}
