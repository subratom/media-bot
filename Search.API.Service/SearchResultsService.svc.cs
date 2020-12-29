using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Search.ElasticSearchMedia;
using Newtonsoft.Json;
using Search.API.Service.Classes;
using Search.ElasticSearchMedia.Interfaces;
using Search.API.Classes;
using Ninject;
using System.ServiceModel.Activation;

namespace Search.API.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode =AspNetCompatibilityRequirementsMode.Allowed)]
    public class SearchResultsService : ISearchResultsService
    {
        private ISearchApi _search;
        public SearchResultsService(ISearchApi search)
        {
            _search = search;
        }

        //public IQueryable<ResultList> GetResultsByPartNumbers(string partNumbers, string[] additionalKeywords, int maxResults)
        //{

        //    _search = new SearchApi();
        //    if (_search == null)
        //        throw new NullReferenceException();
        //    //_search = new SearchApi();

        //    List<ResultList> _objects = new List<ResultList>();

        //    try
        //    {
        //        List<Hit> _results = _search.RelatedPartNumber(partNumbers, additionalKeywords, maxResults);

        //        if (_results != null && _results.Count > 0)
        //        {
        //            _objects[0].ResultCount = _results.Count;
        //            _objects[0].Status = "OK, Results Found";
        //            List<Results> resultsFromSearch = new List<Results>();

        //            foreach (var item in _results)
        //            {
        //                //resultsFromSearch.Add(new Results() { Title = item.Source.Title, Content = item.Source.Content, Description = item.Source.Description, ImageUrl = item.Source.Imageurl, PartNumbers = item.Source.Partnumbers, Pubdate = item.Source.Pubdate, Url = item.Source.Url });
        //            }
        //            _objects[0].Results = resultsFromSearch;

        //            return _objects.AsQueryable<ResultList>();
        //        }
        //        else
        //        {
        //            _objects[0].ResultCount = 0;
        //            _objects[0].Status = "OK, No Results Found";
        //            _objects[0].Results = new List<Results>();
        //            return _objects.AsQueryable<ResultList>();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        JsonConvert.SerializeObject(_objects);
        //    }

        //    _objects[0].ResultCount = 0;
        //    _objects[0].Status = "OK, No Results Found";
        //    _objects[0].Results = new List<Results>();
        //    return _objects.AsQueryable<ResultList>();
        //}

        public string GetResultsByPartNumbers(string partNumbers, string[] additionalKeywords, int maxResults)
        {

            //_search = new SearchApi();
            if (_search == null)
                throw new NullReferenceException();
            //_search = new SearchApi();

            //List<Search.API.Classes.ResultList> _objects = new List<Search.API.Classes.ResultList>();

            //try
            //{
            //    List<Hit> _results = _search.RelatedPartNumber(partNumbers, additionalKeywords, maxResults);

            //    if (_results != null && _results.Count > 0)
            //    {
            //        _objects[0].ResultCount = _results.Count;
            //        _objects[0].Status = "OK, Results Found";
            //        List<Results> resultsFromSearch = new List<Results>();

            //        foreach (var item in _results)
            //        {
            //            //resultsFromSearch.Add(new Results() { Title = item.Source.Title, Content = item.Source.Content, Description = item.Source.Description, ImageUrl = item.Source.Imageurl, PartNumbers = item.Source.Partnumbers, Pubdate = item.Source.Pubdate, Url = item.Source.Url });
            //        }
            //        _objects[0].Results = resultsFromSearch;

            //        return JsonConvert.SerializeObject(_objects);
            //    }
            //    else
            //    {
            //        _objects[0].ResultCount = 0;
            //        _objects[0].Status = "OK, No Results Found";
            //        _objects[0].Results = new List<Results>();
            //        return JsonConvert.SerializeObject(_objects);
            //        //return _objects.AsQueryable<ResultList>();
            //    }
            //}
            //catch (Exception e)
            //{
            //    JsonConvert.SerializeObject(_objects);
            //}

            //_objects[0].ResultCount = 0;
            //_objects[0].Status = "OK, No Results Found";
            //_objects[0].Results = new List<Results>();
            //return JsonConvert.SerializeObject(_objects);
            //return _objects.AsQueryable<ResultList>();
            return string.Empty;
        }
    }
}
