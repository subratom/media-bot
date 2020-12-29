using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search.ElasticSearchMedia.Interfaces
{
    public interface ISearchApi
    {
        SearchHits RelatedPartNumber(string partFamily, string[] additionalTerms = null, int numResults = 25);
        SearchHits RelatedPartNumber(string partFamily, string[] additionalTerms = null, int numResults = 25, int pageNumber = 0, DateTime? StartDate = null, DateTime? EndDate = null);
        SearchHits RelatedContent(string url, int numResults = 25, int pageNumber = 0, DateTime? StartDate = null, DateTime? EndDate = null);
        SearchHits RelatedContent(string url);
        SearchHits All(string queryTxt, int numResults = 25, DateTime? StartDate = null, DateTime? EndDate = null, string SiteName = "", int pageNumber = 0);
        SearchHits All(int from = 0, int to = 10);
        SearchHits Url(string queryTxt, int numResults = 25, int pageNumber = 0);
        SearchHits BeginsWithContentField(string queryTxt, int numResults = 25, int pageNumber = 0, DateTime? StartDate = null, DateTime? EndDate = null);
        SearchHits BeginsWithContentField(string queryTxt);
        SearchHits ContentBySite(string siteName, string queryText);
        SearchHits ContentBySite(string siteName, string queryText, int pageNumber = 0, int numResults = 25, DateTime? StartDate = null, DateTime? EndDate = null);
        SearchHits LatestContent(int numResults = 25);
        SearchHits LatestContent(int numResults = 25, string siteName = "", DateTime? StartDate = null, DateTime? EndDate = null, int pageNumber = 0);
    }
}
