using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Search.RelatedContent.Helpers;
using Search.ElasticSearchMedia;
using Search.ElasticSearchMedia.Interfaces;
using Ninject;
using System.Reflection;
using Search.RelatedContent.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Search.RelatedContent.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISearchApi _search;
        private readonly HttpClient _client;

        public SearchController(ISearchApi searchapi)
        {
            _search = searchapi;
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://search.aspencore.com/api/")
            };
        }

        // GET: Search
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RelatedContent()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Keywords()
        {
            return View();
        }

        public ActionResult PartNumbersMediaSearch()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> PartNumbersMediaSearch(PartsResults x)
        {
            string[] additionalParameters = null;
            if (x.Query.AdditionalKeywords != null)
            {
                additionalParameters = x.Query.AdditionalKeywords;
            }

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            PartsQuery q = new PartsQuery() { PartFamily = x.Query.PartFamily, AdditionalKeywords = x.Query.AdditionalKeywords, MaxCount = x.Query.MaxCount };
            StringBuilder sb = new StringBuilder();
            if (x.Query.AdditionalKeywords != null)
            {
                sb.Append("additonalKeyword=");
                foreach (string item in x.Query.AdditionalKeywords)
                {
                    //first time would be different that next occurances
                }
            }

            //This will need to change to take IHttpActionResult as the return type.
            HttpResponseMessage message = await _client.GetAsync(string.Format("v1/searchparts?partFamily={0}&additionalKeywords={1}&maxCount=5", q.PartFamily, null));

            string data = await message.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(data))
                return View();
            else
            {
                PartsResults p = new PartsResults();
                p = JsonConvert.DeserializeObject<PartsResults>(data);
                return View(p);
            }
        }

        [HttpPost]
        public ActionResult Keywords(KeywordQuery x)
        {

            var results = _search.All(x.QueryText);
            List<Items> result = new List<Items>();
            KeywordResults _items = new KeywordResults();
            //_items.QueryItem = new SearchQuery();
            //_items.Results = result;// as IEnumerable<Search.Results>;

            if (results != null)
            {
                foreach (var hit in results.Hits)
                {
                    Items article = new Items
                    {
                        Title = hit.Source.Title,
                        Url = hit.Source.Url,
                        Description = hit.Source.Description
                    };
                    string imageUrl = hit.Source.ImageUrl;
                    if (string.IsNullOrEmpty(imageUrl) == true)
                    {
                        article.ImageUrl = article.Url.GetImageByUrl();
                    }

                    article.PartNumbers = hit.Source.PartNumbers;

                    result.Add(article);
                }
                //_items.Query.Url = x.Url;
                _items.Results = result;// as IEnumerable<Search.Results>;
            }
            return View(_items);
        }

        [HttpPost]
        public ActionResult RelatedContent(UrlQuery x)
        {
            var results = _search.RelatedContent(x.Url.Trim(), 10);
            List<Items> result = new List<Items>();
            UrlResults _items = new UrlResults();

            if (results != null)
            {
                foreach (var hit in results.Hits)
                {
                    Items article = new Items
                    {
                        Title = hit.Source.Title,
                        Url = hit.Source.Url,
                        Description = hit.Source.Description
                    };
                    string imageUrl = hit.Source.ImageUrl;
                    if (string.IsNullOrEmpty(imageUrl) == true)
                    {
                        article.ImageUrl = article.Url.GetImageByUrl();
                    }
                    article.PartNumbers = hit.Source.PartNumbers;
                    result.Add(article);
                }
                //_items.Query.Url = x.Url;
                _items.Results = result;// as IEnumerable<Results>;
            }
            return View(_items);
        }

        [HttpGet]
        public ActionResult RelatedContent(string Url)
        {
            var results = _search.RelatedContent(Url, 10);
            List<Items> result = new List<Items>();
            UrlResults _items = new UrlResults();
            //_items.Query = new SearchQuery();
            //_items.ResultItems = new List<Results>();

            if (results != null)
            {
                foreach (var hit in results.Hits)
                {
                    Items article = new Items
                    {
                        Title = hit.Source.Title,
                        Url = hit.Source.Url,
                        Description = hit.Source.Description
                    };
                    string imageUrl = hit.Source.ImageUrl;
                    if (string.IsNullOrEmpty(imageUrl) == true)
                    {
                        article.ImageUrl = article.Url.GetImageByUrl();
                    }
                    article.PartNumbers = hit.Source.PartNumbers;

                    result.Add(article);
                }
                //_items.Query.Url = Url;

                var originalItemHit = _search.Url(Url);
                if (originalItemHit != null)
                {
                    //This should return only one result
                    UrlResults r = new UrlResults();
                    List<Items> article = new List<Items>
                    {
                        new Items()
                        {
                            Title = originalItemHit.Hits[0].Source.Title,
                            Url = originalItemHit.Hits[0].Source.Url,
                            ImageUrl = originalItemHit.Hits[0].Source.Url.GetImageByUrl(),
                            Description = originalItemHit.Hits[0].Source.Description,
                            PartNumbers = originalItemHit.Hits[0].Source.PartNumbers
                        }
                    };
                    //r.ResultItems = article;
                    ViewBag.OriginalArticleHit = r;
                }
            }
            return View(_items);
        }
    }
}