using System;
using Search.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Search.API.Classes;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Search.ElasticSearchMedia.Interfaces;

namespace Search.API.Test
{
    [TestClass]
    public class TestSearchResultsController
    {
        public HttpClient httpClient;

        //ISearchApi searchApi;
        //public SearchResultsController()
        //{
        //    httpClient.BaseAddress = new Uri("https://search.aspencore.com/api/api/v1/?");
        //    httpClient.DefaultRequestHeaders.Accept.Clear();
        //    httpClient.DefaultRequestHeaders.Accept.Add(
        //        new MediaTypeWithQualityHeaderValue("application/json"));
        //}

        //[TestMethod]
        //public void SearchResultsTest()
        //{


        //}

        [TestInitialize]
        public void TestSearchResultsControllerInitialize()
        {
            //var kernel = NinjectWebCommon.CreatePublicKernel();
        }

        [TestMethod]
        //public async Task SearchKeywords_GetResults()
        //{
        //    //UrlQuery queryItem = new UrlQuery();
        //    //queryItem.Url = "";
        //    //queryItem.MaxCount = 5; 
        //    //Assert.Equals()

        //    //httpClient.BaseAddress = new Uri("https://search.aspencore.com/api/v1/?");
        //    //httpClient.DefaultRequestHeaders.Accept.Clear();
        //    //httpClient.DefaultRequestHeaders.Accept.Add(
        //    //    new MediaTypeWithQualityHeaderValue("application/json"));

        //    //searchApi = new SearchApi


        //    //UrlQuery q = new UrlQuery();
        //    //q.MaxCount = 5;
        //    //q.Url = "";

        //    //SearchResultsController r = new SearchResultsController();
        //    //IHttpActionResult httpResponse = r.SearchKeywords(q);


        //    //Assert.IsNotNull(result);

        //}


        private string GetURL()
        {

            return string.Empty;
        }
    }
}
