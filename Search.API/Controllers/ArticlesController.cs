using Search.API.Classes;
using Search.API.Implementation;
using Search.API.Interfaces;
using Search.ElasticSearchMedia.Interfaces;
using Search.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security.AntiXss;
using Microsoft.ApplicationInsights;

namespace Search.API.Controllers
{
    [AllowAnonymous]
#if !DEBUG
    [RoutePrefix("api/v1")]
#endif

    public class ArticlesController : ApiController
    {
        private ISearchApi _search;
        private TelemetryClient telemetry = new TelemetryClient();

        public ArticlesController(ISearchApi searchApi)
        {
            _search = searchApi;
        }

        [HttpGet]
        [Route("news/latest")]
        public IHttpActionResult LatestArticles([FromUri] ContentQuery query)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("An error occured while processing the request.");
                }
                else
                {
                    SearchResults data = DataHelpers.ConvertSearchResults(_search.LatestContent(query.MaxCount), query.MaxCount == 0 ? 25 : query.MaxCount, false, true);
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

    }
}
