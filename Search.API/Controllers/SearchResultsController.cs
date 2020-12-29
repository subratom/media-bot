using Search.ElasticSearchMedia.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Search.Helpers;
using Search.API.Models;
using Search.ElasticSearchMedia;
using Search.API.Classes;
using System.Text;
using System.Web.Security.AntiXss;
using System.Web;
using Search.API.Implementation;
using Search.API.Interfaces;
using Microsoft.ApplicationInsights;

namespace Search.API.Controllers
{
    [AllowAnonymous]
#if !DEBUG
    [RoutePrefix("api/v1")]
#endif

    public class SearchResultsController : ApiController
    {
        private ISearchApi _search;
        private TelemetryClient telemetry = new TelemetryClient();

        public SearchResultsController(ISearchApi searchApi)
        {
            _search = searchApi;
        }

        [HttpGet]
        [Route("search/parts")]
        public IHttpActionResult SearchPartsWithAdditionalKeywords([FromUri] PartsQuery query)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("An error occured while processing the request.");
                }
                else
                {
                    //Model Validation is done using ValidateActionFilter
                    string partFamily = HtmlHelper.GetUrl(AntiXssEncoder.UrlEncode(query.PartFamily.ToLower()));
                    SearchResults data = DataHelpers.ConvertSearchResults(_search.RelatedPartNumber(partFamily, query.AdditionalKeywords, query.MaxCount, query.PageNumber, query.StartDate, query.EndDate), query.MaxCount == 0 ? 25 : query.MaxCount);
                    if (data.ResultCount == 0)
                    {
                        return Ok(data);
                    }
                    else
                    {
                        return Ok(data);
                    }
                }

            }
            catch (Exception e)
            {
                telemetry.TrackException(e);
                return BadRequest("An error occured while processing the request.");
            }
        }

        [HttpGet]
        [Route("search/relatedcontent")]
        public IHttpActionResult SearchKeywords([FromUri] UrlQuery query)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Please provide all the paramters to process the request.");
                }
                else
                {
                    string url = HtmlHelper.GetUrl(AntiXssEncoder.UrlEncode(query.Url.ToLower()));

                    SearchResults data = DataHelpers.ConvertSearchResults(_search.RelatedContent(url.CleanUrl(), query.MaxCount, query.PageNumber, query.StartDate, query.EndDate), query.MaxCount == 0 ? 25 : query.MaxCount, false);
                    if (data.ResultCount == 0)
                    {
                        return Ok(data);
                    }
                    else
                    {
                        if (data.ResultCount > query.MaxCount)
                        {
                            return Ok(data.Results.Take(query.MaxCount));
                        }
                        return Ok(data);
                    }
                }
            }
            catch (Exception e)
            {
                telemetry.TrackException(e);
                return BadRequest("An error occured while processing the request.");
            }
        }

        [HttpGet]
        [Route("search/global")]
        public IHttpActionResult SearchGlobally([FromUri] KeywordQuery query)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("An error occured while processing the request.");
                }
                else
                {
                    if (query == null)
                    {
                        return BadRequest("Query Parameters cannot be null, Please provide queryText parameter to search for content.");
                    }
                    else
                    {
                        SearchResults data = DataHelpers.ConvertSearchResults(_search.All(HttpUtility.UrlDecode(AntiXssEncoder.UrlEncode(query.QueryText.ToLower())), query.MaxCount, query.StartDate, query.EndDate, query.SiteName, query.PageNumber), query.MaxCount == 0 ? 25 : query.MaxCount);
                        if (data.ResultCount == 0)
                        {
                            return Ok(data);
                        }
                        else
                        {
                            return Ok(data);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                telemetry.TrackException(e);
                return BadRequest("An error occured while processing the request." + e);
            }
        }
    }
}