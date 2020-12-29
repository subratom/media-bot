using Elasticsearch.Net;
using Nest;
using Search.API.Classes;
using Search.ElasticSearchMedia.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Search.ElasticSearchMedia.Implementations
{
    public class DevSearchApi : DevNestElasticSearch, ISearchApi
    {
        public const int MAXResults = 25;
        public const int DefaultPageNumber = 0;

        public DevSearchApi()
        {
            if (!elastic.Ping().IsValid)
            {
                throw new ElasticsearchClientException("Elastic Search is unreachable");
            }
        }

        /// <summary>
        /// Search for part family media content. Use optional additional term to influence ranking.
        /// </summary>
        /// <param name="partFamily">Used as a begins with term to search media content</param>
        /// <param name="additionalTerms">Array of optional additional keywords. If found in content, will rank that content higher.</param>
        /// <param name="numResults">Number of results to return</param>
        /// <returns>Top media content that matches the search criteria</returns>
        public SearchHits RelatedPartNumber(string partFamily, string[] additionalTerms, int numResults)
        {
            return RelatedPartNumber(partFamily, additionalTerms, numResults, 0, null, null);
        }

        /// <summary>
        /// Search for part family media content. Use optional additional term to influence ranking. Date Range can be used to filter out content by 
        /// using Start date and End date
        /// </summary>
        /// <param name="partFamily">Used as a begins with term to search media content</param>
        /// <param name="additionalTerms">Array of optional additional keywords. If found in content, will rank that content higher.</param>
        /// <param name="numResults">Number of results to return</param>
        /// <param name="pageNumber">Default value is 0, this will be used to page through results.</param>
        /// <returns>Top media content that matches the search criteria</returns>
        public SearchHits RelatedPartNumber(string partFamily, string[] additionalTerms, int numResults, int pageNumber, DateTime? StartDate, DateTime? EndDate)
        {
            SearchHits relatedContentSearchResults = new SearchHits();

            if (StartDate == null)
            {
                StartDate = new DateTime(1969, 1, 1); //Why 35
            }

            if (EndDate == null)
            {
                EndDate = DateTime.Now;
            }

            QueryContainer shouldContainer = null;
            if (additionalTerms != null)
            {
                foreach (var term in additionalTerms)
                {
                    shouldContainer |= new TermQuery() { Field = "content", Value = term };
                }

                // The media content .Must contain a .Prefix (begins with) that matches the contents of the partFamily variable
                // The media content .Should (optional - doesn't have to) contain items in additional terms
                var searchResults = elastic.Search<Articles>(s => s
                    .Index(mediaIndexName)
                    .From(pageNumber)
                    .Size(numResults)
                    .Query(q => q
                           .Bool(b => b.Should(shouldContainer)
                                 .Must(m => m
                                       .Prefix(p => p
                                               .Field("content")
                                               .Value(partFamily))
                                      )
                                 )
                          && q.DateRange(dr => dr.Field(f => f.Pubdate).GreaterThanOrEquals(StartDate).LessThanOrEquals(EndDate))
                          )
                          //.Sort(x => x.Descending("pubdate"))
                          );

                //.Sort(sort => sort.Ascending("pubdate"))

                relatedContentSearchResults = this.ConvertSearchResultsToHits(searchResults);
            }
            else
            {
                // The media content .Must contain a .Prefix (begins with) that matches the contents of the partFamily variable
                var searchResults = elastic.Search<Articles>(s => s
                    .Index(mediaIndexName)
                    .From(pageNumber)
                    .Size(numResults)
                    .Query(q => q.Bool(b => b.Must(m => m.Prefix(p => p.Field("content").Value(partFamily))))
                    && q.DateRange(dr => dr.Field(f => f.Pubdate).GreaterThanOrEquals(StartDate).LessThanOrEquals(EndDate)))
                    //.Sort(x => x.Descending("pubdate"))
                    );

                relatedContentSearchResults = this.ConvertSearchResultsToHits(searchResults);
            }
            return relatedContentSearchResults;
        }

        /// <summary>
        /// Get related content for a media url
        /// </summary>
        /// <param name="url">URL of the media content to want related content for</param>
        /// <param name="numResults">Number of results</param>
        /// <returns>Related content for the given media URL</returns>
        public SearchHits RelatedContent(string url)
        {
            return RelatedContent(url, MAXResults, DefaultPageNumber, null, null);
        }

        /// <summary>
        /// This will return related content when an URL is passed to Search. You can specify how many results can be returned, paginate it and specify start date and end date for search 
        /// Oldest item is from 1969 at this point so specifying the start date might be a good idea.
        /// </summary>
        /// <param name="url">URL that </param>
        /// <param name="numResults"></param>
        /// <param name="pageNumber"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public SearchHits RelatedContent(string url, int numResults, int pageNumber, DateTime? StartDate, DateTime? EndDate)
        {
            SearchHits relatedContentSearchResults = new SearchHits
            {
                Hits = new List<Hit>(),
                TotalItems = 0
            };

            if (string.IsNullOrEmpty(url))
            {
                return relatedContentSearchResults;
            }

            // use a media URL to find a piece of content
            var articleSearchResult = this.Url(url, MAXResults, DefaultPageNumber);

            if (StartDate == null)
            {
                StartDate = new DateTime(1969, 1, 1); //Why 35
            }

            if (EndDate == null)
            {
                EndDate = DateTime.Now;
            }

            // there should only be one piece of content for every URL
            if (articleSearchResult.Hits.Count == 1)
            {
                // fields use on the morelikethis algorithm
                string[] fieldNames = new string[] { "content" }; //, "title", "partnumbers" 

                var dateFilter = new TermQuery();

                var relatedContent = elastic.Search<Articles>(s => s
                   .From(pageNumber)
                   .Size(numResults)
                   .Index(mediaIndexName)
                   .Type(mediaTypeName)
                   .Query(q => q
                      .MoreLikeThis(mlt => mlt
                         .MinWordLength(3)
                         .Fields(f => f.Fields(fieldNames))
                         .Like(l => l
                            .Document(d => d
                               .Id(articleSearchResult.Hits[0].Id)
                               .Index(mediaIndexName)
                               .Type(mediaTypeName)
                            )
                         )
                      )
                      && q.DateRange(dr => dr.Field(f => f.Pubdate).GreaterThanOrEquals(StartDate).LessThanOrEquals(EndDate))
                   )
                //.Explain(true)
                //.Sort(x => x.Descending("pubdate"))
                );
                string debug = relatedContent.DebugInformation;
                TimeSpan t = new TimeSpan(relatedContent.Took);
                relatedContentSearchResults = ConvertSearchResultsToHits(relatedContent);
            }
            return relatedContentSearchResults;
        }

        // TODO: we should combine the two All methods
        /// <summary>
        /// Performs a simple text query against media content
        /// </summary>
        /// <param name="queryTxt">string with one or more keywords seperated by spaces</param>
        /// <returns>Top ranking media content based on the query string</returns>
        public SearchHits All(string queryTxt, int numResults, DateTime? StartDate, DateTime? EndDate, string SiteName = "", int pageNumber = 0)
        {

            if (numResults == 0)
            {
                numResults = MAXResults;
            }

            if (StartDate == null)
            {
                StartDate = new DateTime(1969, 1, 1);
            }

            if (EndDate == null)
            {
                EndDate = DateTime.Now;
            }

            ISearchResponse<Articles> mediaResponse = null;

            if (string.IsNullOrEmpty(SiteName))
            {
                mediaResponse = elastic.Search<Articles>(s => s
                                                          .Explain(true)
                                                          .Pretty(true)
                                                          .Human(true)
                                                          .Index(mediaIndexName)
                                                          .Type(mediaTypeName)
                                                          .From(pageNumber)
                                                          .Size(numResults)
                                                          .Query(q => q
                                                                       .Match(m => m
                                                                                    .Field(new Field("content", null)).Query(queryTxt)
                                                                             )
                                                                )
                                                   );


            }
            else
            {
                mediaResponse = elastic.Search<Articles>(s => s
                                      .Pretty(true)
                                        .Human(true)
                                        .Index(mediaIndexName)
                                        .Type(mediaTypeName)
                                        .From(pageNumber)
                                        .Size(numResults)
                                        .Query(q => q
                                                .FunctionScore(fs => fs
                                                .Query(qs => qs
                                                        .Bool(b => b
                                                                .Must(m => m
                                                                        .MultiMatch(mm => mm.Fields
                                                                           (
                                                                               f => f
                                                                                   .Field(fn => fn.Title)
                                                                                   .Field(fn => fn.Content)
                                                                           )
                                                                           .Query(queryTxt)),
                                                                           mm => mm.MatchPhrase(mp => mp.Field(new Field("site.keyword", null)).Query(SiteName)))
                                                                )
                                                                && q.DateRange(dr => dr.Field(f => f.Pubdate).GreaterThanOrEquals(StartDate).LessThanOrEquals(EndDate))
                                                             )
                                                   .Functions(f => f
                                                                .GaussDate(gd => gd.Field(df => df.Pubdate).Decay(0.5).Origin(DateMath.Now).Decay(0.5).Scale("30d"))
                                                   )
                                                   .ScoreMode(FunctionScoreMode.Sum)
                                                   .BoostMode(FunctionBoostMode.Multiply)

                                            )));
            }
            return ConvertSearchResultsToHits(mediaResponse);

        }

        // TODO: we should combine the two All methods
        public SearchHits All(int from = 0, int to = 10)
        {
            string queryTxt = string.Empty;

            var mediaResponse = elastic.Search<Articles>(s => s
                .Pretty(true)
                .Human(true)
                .Index(mediaIndexName)
                .Type(mediaTypeName)
                .From(from)
                .Size(to)
                .Query(q => q
                    .SimpleQueryString(x => x.
                        Query(queryTxt)
                    )
                 )
            );

            return ConvertSearchResultsToHits(mediaResponse);

        }

        /// <summary>
        /// This will look up URL in Elastic Search
        /// </summary>
        /// <param name="queryTxt">This will be URL parameter</param>
        /// <param name="numResults">This will always be 1 if the item exists. If it's more than than one we might have to handle that as exception case</param>
        /// <param name="pageNumber">This should always be 0 as all the URLs in our Index should be unqiue</param>
        /// <returns></returns>
        public SearchHits Url(string queryTxt, int numResults, int pageNumber)
        {
            try
            {
                var mediaResponse = elastic.Search<Articles>(s => s
                .Pretty(true)
                .Human(true)
                .Index(mediaIndexName)
                .Type(mediaTypeName)
                .From(pageNumber)
                .Size(numResults)
                .Query(q => q
                             .Match(m => m.Field(
                                        f => f.Url.Suffix("keyword")).Query(queryTxt)
                                    )

                      )
                 ); //.Sort(x => x.Descending("pubdate"))

                return ConvertSearchResultsToHits(mediaResponse);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// This will call BeginsWithContentField with these default values queryTxt, MAXResults, DefaultPageNumber, null, null
        /// </summary>
        /// <param name="queryTxt">Text that begins with is sent as queryText</param>
        /// <returns>Returns SearchHits class which is then converted to SearchResults class.</returns>
        public SearchHits BeginsWithContentField(string queryTxt)
        {
            return BeginsWithContentField(queryTxt, MAXResults, DefaultPageNumber, null, null);
        }

        /// <summary>
        /// This will search items for a specific site. There is no current use case but in case we want filters we can use this as a starter method. 
        /// Only one site name can be taken as the input
        /// </summary>
        /// <param name="siteName">Send the Site name (e.g.: Electronic Products, EETimes)</param>
        /// <returns>Returns SearchHits that corresponds to one site (Electronic Products, EETimes, etc)</returns>
        public SearchHits ContentBySite(string siteName, string queryText)
        {
            return ContentBySite(siteName, queryText, DefaultPageNumber, MAXResults, null, null);
        }

        /// <summary>
        /// This will search items for a specific site. There is no current use case but in case we want filters we can use this as a starter method. 
        /// Only one site name can be taken as the input. Search results can be constrained by start & end date. This method also handles pagination
        /// </summary>
        /// <param name="siteName">Site Name (This field corresponds to SiteName in Elastic Search)</param>
        /// <param name="pageNumber">Page is set to zero by default.</param>
        /// <param name="numResults">Default number of results that we return is set to 25. This number can be as high as 10000 (Elastic Search limit is 10000)</param>
        /// <param name="StartDate">If we want to limit search by date range we can use start date. This is optional and will set it to 1/1/1969 as the start date if null is passed</param>
        /// <param name="EndDate">If we want to limit search by date range we can use end date. This is optional and maximum date can be today if null is passed</param>
        /// <returns></returns>
        public SearchHits ContentBySite(string siteName, string queryText, int pageNumber, int numResults, DateTime? StartDate, DateTime? EndDate)
        {
            SearchHits relatedContentSearchResults = new SearchHits();

            if (StartDate == null)
            {
                StartDate = new DateTime(1969, 1, 1);
            }

            if (EndDate == null)
            {
                EndDate = DateTime.Now;
            }

            var mediaResponse = elastic.Search<Articles>(s => s
                .Pretty(true)
                .Human(true)
                .Index(mediaIndexName)
                .Type(mediaTypeName)
                .From(pageNumber)
                .Size(numResults)
                .Query(
                    q => q.ConstantScore(
                        filter => filter.Filter(b => b.Bool(m => m.Must(
                                                            mmq => mmq.MatchAll(),
                                                            mmq => mmq.MultiMatch(mm => mm.Fields(
                                                                                f => f
                                                                                    .Field(fn => fn.Title)
                                                                                    .Field(fn => fn.Content)
                                                                                    .Field(fn => fn.Description)
                                                                            ).Query(queryText)),
                                                            mmq => mmq.MatchPhrase(mp => mp.Field("site.keyword").Query(siteName))
                                                        )
                                                   )
                                                   && q.DateRange(dr => dr.Field(f => f.Pubdate).GreaterThanOrEquals(StartDate).LessThanOrEquals(EndDate))))));

            //.Field(fn => fn.Content)

            return ConvertSearchResultsToHits(mediaResponse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryTxt"></param>
        /// <param name="numResults"></param>
        /// <param name="pageNumber"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public SearchHits BeginsWithContentField(string queryTxt, int numResults, int pageNumber, DateTime? StartDate, DateTime? EndDate)
        {
            if (StartDate == null)
            {
                StartDate = new DateTime(1969, 1, 1); //Why 35
            }

            if (EndDate == null)
            {
                EndDate = DateTime.Now;
            }

            var mediaResponse = elastic.Search<Articles>(s => s
                .Pretty(true)
                .Human(true)
                .Index(mediaIndexName)
                .Type(mediaTypeName)
                .From(pageNumber)
                .Size(numResults)
                .Query(q => q
                    .Prefix(c => c
                        .Field(p => p.Content)
                        .Value(queryTxt)
                    )
                    && q.DateRange(dr => dr.Field(f => f.Pubdate).GreaterThanOrEquals(StartDate).LessThanOrEquals(EndDate))
                )
            //.Sort(x => x.Descending("pubdate")).Sort(score=>score.Descending(f=>f.Score))
            );

            return ConvertSearchResultsToHits(mediaResponse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numResults"></param>
        /// <returns></returns>
        public SearchHits LatestContent(int numResults)
        {
            return LatestContent(numResults, "", null, null, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numResults"></param>
        /// <param name="siteName"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public SearchHits LatestContent(int numResults, string siteName, DateTime? StartDate, DateTime? EndDate, int pageNumber)
        {
            if (StartDate == null)
            {
                StartDate = new DateTime(1969, 1, 1);
            }

            if (EndDate == null)
            {
                EndDate = DateTime.Now;
            }

            var results = elastic.Search<Articles>(s => s
                .Pretty(true)
                .Human(true)
                .Index(mediaIndexName)
                .Type(mediaTypeName)
                .From(pageNumber)
                .Size(numResults)
                .Query(
                    q => q.MatchAll()
                    && q.DateRange(dr => dr.Field(f => f.Pubdate).GreaterThanOrEquals(StartDate).LessThanOrEquals(EndDate))
                    )
                    .Sort(
                            sort => sort.Descending(
                                                    f => f.Pubdate)
                          )

                );

            return ConvertSearchResultsToHits(results);
        }

        /// <summary>
        /// This will take ISearchResponse object and return SearchHits (custom class) that can be used on the front end
        /// </summary>
        /// <param name="searchResponse">ISearchResponse object of Articles type</param>
        /// <returns>Returns SearchHits object which is then converted into SearchResults object which maps items from Elastic Search to custom class SearchResults</returns>
        private SearchHits ConvertSearchResultsToHits(ISearchResponse<Articles> searchResponse)
        {
            SearchHits result = null;


            if (searchResponse == null)
            {
                return result;
            }

            long total = searchResponse.HitsMetadata.Total;
            double maxScore = searchResponse.HitsMetadata.MaxScore;

            var mediaDocs = searchResponse.Hits.Select(hit =>
            {
                Hit h = new Hit
                {
                    Score = hit.Score,
                    Index = hit.Index,
                    Type = hit.Type,
                    Id = hit.Id,
                    Source = hit.Source
                };
                return h;
            });

            result = new SearchHits
            {
                Hits = new List<Hit>()
            };
            result.Hits = mediaDocs.ToList<Hit>();
            result.TotalItems = searchResponse.HitsMetadata.Total;
            return result;
        }
    }
}
