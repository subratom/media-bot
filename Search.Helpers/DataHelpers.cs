using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Search.API.Classes;
using Search.ElasticSearchMedia;

namespace Search.Helpers
{
    public static class DataHelpers
    {

        /// <summary>
        /// This will convert Elastic Search Results to Search Results class. 
        /// </summary>
        /// <param name="hits">Results based on criteria from the calling function</param>
        /// <param name="MaxResults">Max Number of results to be returned</param>
        /// <param name="bReturnContents">Should we return contents section. Returning this is not recommended as the payload of the JSON object is much higher</param>
        /// <param name="latestContent">This flag will be used to determine what images should be returned. Network news uses author images, related content uses article thumbnails</param>
        /// <returns></returns>
        public static SearchResults ConvertSearchResults(SearchHits hits, int MaxResults = 0, bool bReturnContents = true, bool latestContent = false)
        {
            SearchResults _items = new SearchResults();

            List<Hit> list = hits.Hits;

            if (list != null)
            {
                List<Articles> _results = new List<Articles>();

                if (list.Count == 0)
                {
                    _items.Results = _results;
                    _items.ResultCount = 0;
                    _items.TotalCount = 0;
                    _items.Status = "No Results Found";
                }
                else
                {
                    foreach (var hit in list)
                    {
                        Articles article = new Articles
                        {
                            Id = hit.Id,
                            Title = hit.Source.Title,
                            Url = hit.Source.Url,
                            Description = hit.Source.Description,
                            Site = hit.Source.Site
                        };
                        string imageUrl = string.Empty;

                        imageUrl = (latestContent) ? 
                            (string.IsNullOrEmpty(hit.Source.Author) ? hit.Source.ImageUrl.GetImageForDisplay(hit.Source.ImageUrl) : (string.IsNullOrEmpty(HtmlHelper.GetAuthorImage(hit.Source.Author)) ? hit.Source.ImageUrl : HtmlHelper.GetAuthorImage(hit.Source.Author)))
                            : hit.Source.ImageUrl.GetImageForDisplay(hit.Source.ImageUrl);

                        article.ImageUrl = string.IsNullOrEmpty(imageUrl) == true ? article.Url.GetDefaultImageUrls() : imageUrl;

                        article.Content = bReturnContents ? hit.Source.Content : string.Empty;
                        article.Pubdate = hit.Source.Pubdate;
                        article.PartNumbers = hit.Source.PartNumbers ?? (new string[0]);
                        article.Author = hit.Source.Author;
                        article.Score = hit.Score;
                        _results.Add(article);
                    }
                    _items.Results = _results.Take(MaxResults).ToList();
                    _items.ResultCount = _results.Count;
                    _items.Status = "Success";
                    _items.TotalCount = hits.TotalItems;
                }
                return _items;
            }
            _items.Status = "Error Processing Request";
            _items.ResultCount = 0;
            _items.TotalCount = 0;
            return _items;
        }
    }
}
