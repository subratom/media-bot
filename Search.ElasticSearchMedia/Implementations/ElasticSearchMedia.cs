using System;
using System.Globalization;
using Nest;
using Search.API.Classes;
using Search.ElasticSearchMedia.Interfaces;

namespace Search.ElasticSearchMedia.Implementations
{
    public class Index : NestElasticSearch, IElasticSearchMedia
    {
        private ISearchApi _search;

        public Index(ISearchApi searchApi)
        {
            _search = searchApi;
        }

        public bool ClearParts(Articles newArticle)
        {
            if (string.IsNullOrEmpty(newArticle.Url) == true)
            {
                throw new MissingRequiredArticleFieldException("Url");
            }

            // let's search for an article with the given URL
            string url = newArticle.Url;
            //SearchApi search = new SearchApi();
            var results = _search.Url(url);
            double resultCount = results.TotalItems;
            bool indexResult = false;

            // if we find 0 results - we add the new article
            if (resultCount == 0)
            {
                // make sure that required fields have been populated 
                if (string.IsNullOrEmpty(newArticle.Content) == true)
                {
                    throw new MissingRequiredArticleFieldException("Content");
                }

                if (string.IsNullOrEmpty(newArticle.Description) == true)
                {
                    throw new MissingRequiredArticleFieldException("Description");
                }

                if (string.IsNullOrEmpty(newArticle.Title) == true)
                {
                    throw new MissingRequiredArticleFieldException("Title");
                }

                indexResult = this.InsertArticle(newArticle);
            }

            // if we find one result we update the article
            else if (resultCount == 1)
            {
                Articles existingArticle = results.Hits[0].Source;
                string existingArticleId = results.Hits[0].Id;

                // we need to sync the contents of the new article into the existing article
                newArticle.Sync(existingArticle);
                newArticle.PartNumbers = null;

                // update
                indexResult = this.UpdateArticle(newArticle, results.Hits[0].Id);
            }

            // if we find more than 1 result we throw an exception
            // all urls must be unique
            else
            {
                throw new Exception($"more than one copy of the article exists with url:{newArticle.Url}");
            }

            return indexResult;
        }
        public bool Upsert(Articles newArticle)
        {
            if (string.IsNullOrEmpty(newArticle.Url) == true)
            {
                throw new MissingRequiredArticleFieldException("Url");
            }

            // let's search for an article with the given URL
            string url = newArticle.Url;
            //SearchApi search = new SearchApi();
            var results = _search.Url(url);
            double resultCount = results.TotalItems;
            bool indexResult = false;

            // if we find 0 results - we add the new article
            if (resultCount == 0)
            {
                // make sure that required fields have been populated 
                if (string.IsNullOrEmpty(newArticle.Content) == true)
                {
                    throw new MissingRequiredArticleFieldException("Content");
                }

                if (string.IsNullOrEmpty(newArticle.Description) == true)
                {
                    throw new MissingRequiredArticleFieldException("Description");
                }

                if (string.IsNullOrEmpty(newArticle.Title) == true)
                {
                    throw new MissingRequiredArticleFieldException("Title");
                }

                indexResult = this.InsertArticle(newArticle);
            }

            // if we find one result we update the article
            else if (resultCount == 1)
            {
                Articles existingArticle = results.Hits[0].Source;
                string existingArticleId = results.Hits[0].Id;

                // we need to sync the contents of the new article into the existing article
                newArticle.Sync(existingArticle);

                // update
                indexResult = this.UpdateArticle(newArticle, results.Hits[0].Id);
            }
            else if (resultCount > 1)
            {
                int currentItemNumber = 0;
                foreach(var result in results.Hits)
                {
                    if(currentItemNumber == 0)
                    {
                        Articles existingArticle = results.Hits[0].Source;
                        string existingArticleId = results.Hits[0].Id;

                        // we need to sync the contents of the new article into the existing article
                        newArticle.Sync(existingArticle);

                        // update
                        indexResult = this.UpdateArticle(newArticle, results.Hits[0].Id);
                    }
                    else
                    {
                        this.DeleteDocument(result.Id);
                    }
                    currentItemNumber++;
                }
            }

            // if we find more than 1 result we throw an exception
            // all urls must be unique
            else
            {
                throw new Exception($"more than one copy of the article exists with url:{newArticle.Url}");
            }

            return indexResult;
        }
        private bool UpdateArticle(Articles article, string Id)
        {
            //var lastUpdatedDate = new
            //{
            //    pub_date = DateTime.UtcNow
            //};

            //// do the partial update. 
            //// Page is TDocument, object is TPartialDocument
            //var partialUpdateResponse = elastic.Update<Articles, object>(article.Id, u => u
            //    .Doc(lastUpdatedDate)
            //);

            //IndexName indexName = new IndexName();
            //indexName.Name = mediaIndexName;

            var update = elastic.Index(article, i => i.Index(mediaIndexName).Id(Id));
            var bResult = update.IsValid;
            return bResult;
        }
        private bool InsertArticle(Articles article)
        {
            IndexRequest<Articles> indexRequest = new IndexRequest<Articles>(mediaIndexName, mediaTypeName);
            IndexRequest<Articles> ir = indexRequest;
            ir.Document = article;

            // TODO: this is a temporary work around so that the pubdate is not empty
            if (article.Pubdate == null)
            {
                article.Pubdate = DateTime.ParseExact(DateTime.Now.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }

            // index the article
            var indexResult = elastic.Index(ir);
            bool result = indexResult.IsValid;
            return result;
        }
        private void VerifyRequiredFieldsSupplied(Articles article)
        {

        }
        public void CreateTestDocuments(int total)
        {
            int baseIndex = 1000;
            int i = baseIndex;

            // let's try to index some new content
            while (i < baseIndex + total)
            {
                Articles article = new Articles
                {
                    Title = $"This is an autogenerated article {DateTime.Now}",
                    //article.Url = $"http://www.google.com/#q=ABC{i}&";
                    //article.Url = $"http://www.google.com/q=ABC{i}";
                    Url = $"ABC-{i}",
                    Content = $"This is an autogenerated article - content section {DateTime.Now} ABC{i}",
                    Description = $"ABC{i}",
                    ImageUrl = @"https://www.google.com/images/branding/googlelogo/2x/googlelogo_color_120x44dp.png",
                    Cmskeywords = new string[] { "AutoGenerate", "test", $"{DateTime.Now}" },
                    PartNumbers = new string[] { $"ABC{i}" }
                };

                //IndexName indexName = new IndexName();
                //indexName.Name = mediaIndexName;
                //TypeName typeName = new TypeName();
                //typeName.Name = mediaTypeName;
                IndexRequest<Articles> ir = new IndexRequest<Articles>(mediaIndexName, mediaTypeName)
                {
                    Document = article
                };

                var indexResult = elastic.Index(ir);

                i++;
            }

        }
        public void DeleteDocument(string Id)
        {
            try
            {
                IDeleteResponse response = elastic.Delete<Articles>(Id, d => d.Index(mediaIndexName).Type(mediaTypeName));
            }
            catch(Exception e)
            {
                throw e;
            }
            
            //elastic.DeleteByQuery<Articles>(q => q.Query(rq => rq.Term(f => f.Site, site)));
        }

    }
}
