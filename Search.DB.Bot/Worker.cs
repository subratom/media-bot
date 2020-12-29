using Search.API.Classes;
using Search.ElasticSearchMedia;
using Search.ElasticSearchMedia.Implementations;
using Search.Helpers;
using Search.MediaStore;
using Search.MediaStore.DDL;
using Search.Scrapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Search.DB.Bot
{
    public class Worker
    {
        public delegate void DoneDelegate(string taskId);
        public static DoneDelegate Done { private get; set; }

        public async void DoWork(object id, string url, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;
            //await Task.Delay(100); //this will an await call to get content
            Content obj;
            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
                BotDDL botUpdate = new BotDDL();

                Mappings itemfound = botUpdate.GetItemByUrl(url);
                int tries = 0;
                bool IsUrlProcessed = true;

                //DateTime dtStart = DateTime.Now;
                string articleDate = string.Empty;

                try
                {
                    ScrapeWeb bot = new ScrapeWeb();

                    SearchApi searchApi = new SearchApi();
                    SearchHits searchHits = searchApi.Url(url, 5, 0);
                    if (searchHits.Hits.Count() == 0)
                    {
                        obj = await bot.ReturnArticleObject(url);
                        if (obj != null)
                        {
                            if (obj.Code != HttpStatusCode.OK)
                            {
                                Console.WriteLine(string.Format("\r Status is {0}", obj.Code));

                                tries = itemfound.UrlMaxTries + 1;
                                IsUrlProcessed = false;
                                itemfound.HttpCode = obj.Code;
                                UpdateItem(itemfound, tries, IsUrlProcessed);
                            }
                            else
                            {

                                string title = obj.Title;
                                string content = obj.Contents;
                                string description = obj.Description;

                                Articles article = new Articles
                                {
                                    Site = url.GetSite(),
                                    Content = content,
                                    Title = title,
                                    Url = url.ToLower(),
                                    Description = description
                                };
                                string strThumbNail = HtmlHelper.GetImageUrl(url, obj.RawResponse);
                                strThumbNail = url.GetImageForDisplay(strThumbNail);
                                article.Author = HtmlHelper.GetAuthor(url, obj.RawResponse);
                                if (!string.IsNullOrEmpty(strThumbNail))
                                {
                                    //This condition needs to be added to remove ?n=<number> from EP thumbnails
                                    if (strThumbNail.Contains("?"))
                                    {
                                        article.ImageUrl = strThumbNail.Substring(0, strThumbNail.IndexOf("?")).Replace("http:", "https:");
                                    }
                                    else
                                        article.ImageUrl = strThumbNail.Replace("http:", "https:");
                                }
                                else
                                {
                                    article.ImageUrl = string.IsNullOrEmpty(strThumbNail) ? article.Url.GetDefaultImageUrls() : strThumbNail.Replace("http:", "https:");
                                }

                                articleDate = HtmlHelper.GetPublishDate(url, obj.RawResponse);
                                if (string.IsNullOrEmpty(articleDate))
                                    article.Pubdate = DateTime.Now;
                                else
                                    article.Pubdate = DateTime.Parse(articleDate);

                                try
                                {
                                    var client = new Index(searchApi);
                                    var result = client.Upsert(article);
                                    itemfound.HttpCode = obj.Code;
                                    if (result)
                                    {
                                        itemfound.DateCreated = string.IsNullOrEmpty(articleDate) ? DateTime.Now : (DateTime.Parse(articleDate) == null ? DateTime.Now : DateTime.Parse(articleDate));
                                        itemfound.DateModified = string.IsNullOrEmpty(articleDate) ? DateTime.Now : (DateTime.Parse(articleDate) == null ? DateTime.Now : DateTime.Parse(articleDate));
                                        itemfound.IsUrlProcessed = true;
                                        UpdateItem(itemfound, 0, true);
                                    }
                                    else
                                    {
                                        tries = itemfound.UrlMaxTries + 1;
                                        IsUrlProcessed = false;
                                        itemfound.DateCreated = string.IsNullOrEmpty(articleDate) ? DateTime.Now : (DateTime.Parse(articleDate) == null ? DateTime.Now : DateTime.Parse(articleDate));
                                        itemfound.DateModified = string.IsNullOrEmpty(articleDate) ? DateTime.Now : (DateTime.Parse(articleDate) == null ? DateTime.Now : DateTime.Parse(articleDate));
                                        UpdateItem(itemfound, tries, IsUrlProcessed);
                                    }
                                }
                                catch (MissingRequiredArticleFieldException mrafe)
                                {
                                    //throw;
                                    tries = itemfound.UrlMaxTries + 1;
                                    IsUrlProcessed = false;
                                    itemfound.DateCreated = string.IsNullOrEmpty(articleDate) ? DateTime.Now : (DateTime.Parse(articleDate) == null ? DateTime.Now : DateTime.Parse(articleDate));
                                    itemfound.DateModified = string.IsNullOrEmpty(articleDate) ? DateTime.Now : (DateTime.Parse(articleDate) == null ? DateTime.Now : DateTime.Parse(articleDate));
                                    UpdateItem(itemfound, tries, IsUrlProcessed);
                                    //throw;
                                }
                            }
                        }
                        else if (obj == null)
                        {
                            tries = itemfound.UrlMaxTries + 1;
                            IsUrlProcessed = false;
                            itemfound.HttpCode = HttpStatusCode.NoContent;
                            UpdateItem(itemfound, tries, IsUrlProcessed);
                        }
                    }
                    else
                    {
                        //sb.AppendFormat("{0}", itemfound.Url);
                        //sb.AppendLine();
                        tries = itemfound.UrlMaxTries + 1;
                        IsUrlProcessed = true;
                        itemfound.IsUrlProcessed = true;
                        itemfound.HttpCode = HttpStatusCode.OK;
                        itemfound.DateCreated = string.IsNullOrEmpty(articleDate) ? DateTime.Now : (DateTime.Parse(articleDate) == null ? DateTime.Now : DateTime.Parse(articleDate));
                        itemfound.DateModified = string.IsNullOrEmpty(articleDate) ? DateTime.Now : (DateTime.Parse(articleDate) == null ? DateTime.Now : DateTime.Parse(articleDate));
                        UpdateItem(itemfound, tries, IsUrlProcessed);
                    }
                }
                catch (Exception e)
                {
                    tries = itemfound.UrlMaxTries + 1;
                    IsUrlProcessed = false;
                    itemfound.IsUrlProcessed = false;
                    itemfound.HttpCode = HttpStatusCode.BadRequest;
                    itemfound.DateCreated = string.IsNullOrEmpty(articleDate) ? DateTime.Now : (DateTime.Parse(articleDate) == null ? DateTime.Now : DateTime.Parse(articleDate));
                    itemfound.DateModified = string.IsNullOrEmpty(articleDate) ? DateTime.Now : (DateTime.Parse(articleDate) == null ? DateTime.Now : DateTime.Parse(articleDate));
                    UpdateItem(itemfound, tries, IsUrlProcessed);

                    //throw;

                    //Console.WriteLine();
                    //Console.WriteLine(url);
                    //Console.WriteLine(e);
                    //Console.WriteLine("--------------------------------------------------------------------------");
                }
                finally
                {
                    //DateTime dtEnd = DateTime.Now;

                    //Console.WriteLine(string.Format("\r Total time taken to process items is {0}", (dtEnd - dtStart).TotalSeconds));

                }
            }
            catch (Exception e)
            {
                //throw;
                //Console.WriteLine();
                //Console.WriteLine(url);
                //Console.WriteLine(e);
                //Console.WriteLine("--------------------------------------------------------------------------");
            }
            finally
            {
                Done((string)id);
            }

        }

        private static void UpdateItem(Mappings itemfound, int tries = 0, bool IsUrlProcessed = true)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            BotDDL botUpdate = new BotDDL();
            BotDDL.ConnectionString = ConnectionString;
            try
            {
                Mappings item = new Mappings
                {
                    SiteName = itemfound.SiteName,
                    IsUrlProcessed = IsUrlProcessed,
                    IsPartProcessed = itemfound.IsPartProcessed,
                    Url = itemfound.Url,
                    UrlMaxTries = tries,
                    PartMaxTries = 0,
                    DateCreated = itemfound.DateCreated,
                    DateModified = itemfound.DateModified,
                    HttpCode = itemfound.HttpCode
                };
                botUpdate.UpdateUrl(item);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
