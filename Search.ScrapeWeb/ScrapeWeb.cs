using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Search.Helpers;
using System.Net;

namespace Search.Scrapper
{
    public class ScrapeWeb
    {
        private StringBuilder sbText = new StringBuilder();
        private string strWebContent;
        private string currentUrl;
        //private string timeInterval = ConfigurationManager.AppSettings["minTimeInterval"].ToString(); //250 milliseconds will make it a 4 calls/ min thereby avoiding overload
        //private TimeSpan timeStart, timeEnd;

        private static DateTime LastCallToSiliconExpert { get; set; } = DateTime.Now;
        public int MinimumWaitTimeBetweenCalls { get; } = 250; // milli seconds
        private object thisLock = new object();

        //public string RawResponse { get; set; }

        public ScrapeWeb()
        {

        }
        /// <summary>
        /// This will take URL parameter, check if the calls are less than 4 calls/ second and then proceed to create a Content object
        /// </summary>
        /// <param name="url">URL to scrape from</param>
        /// <returns></returns>
        public async Task<Content> ReturnArticleObject(string url)
        {
            Content item = new Content();
            try
            {
                LastCallToSiliconExpert = DateTime.Now;
                this.Timer();
                item = await GetDataFromUrl(url);
                if (!string.IsNullOrEmpty(item.RawResponse))
                {
                    strWebContent = item.RawResponse;
                    item.Title = GetTitle();
                    item.Description = GetDescription();
                    item.RawBodyContents = GetContent();
                    item.Contents = HtmlHelper.RemoveMultipleSpaces(string.Join(" ", Regex.Split(HttpUtility.HtmlDecode((HtmlHelper.RemoveHtmlTags(item.RawBodyContents)).Replace(@"""", @"\""").Replace("//", "")), @"(?:\r\n|\n|\r)")));
                    item.PubDate = GetPubDate();
                }
                LastCallToSiliconExpert = DateTime.Now;
                return item;

            }
            catch(InvalidOperationException ioe)
            {
                return null;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void Timer()
        {
            lock (thisLock)
            {
                DateTime now = DateTime.Now;
                TimeSpan timeSpanSinceLastCall = now - LastCallToSiliconExpert;
                /// round down so we don't make more calls becasue of rounding errors
                int milliSecondsSinceLastCall = (int)Math.Floor(timeSpanSinceLastCall.TotalMilliseconds);

                if (milliSecondsSinceLastCall < MinimumWaitTimeBetweenCalls)
                {
                    int pauseInterval = MinimumWaitTimeBetweenCalls - milliSecondsSinceLastCall;
                    Thread.Sleep(pauseInterval);
                }

                LastCallToSiliconExpert = DateTime.Now;
            }
        }

        /// <summary>
        /// Checks if a file exists to write min  time interval in the go. If it does, overwrites the existing value. This is used to calculate and see if the thread needs to slee or not.
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        //private double WriteTimeStamp(double delta = 0)
        //{
        //    double readValueFromFile = 0, minTimeToWait;
        //    try
        //    {
        //        if (!File.Exists(ConfigurationManager.AppSettings["filePath"].ToString() + "lastTimestamp.txt"))
        //        {
        //            File.WriteAllText(ConfigurationManager.AppSettings["filePath"].ToString() + "lastTimestamp.txt", timeInterval.ToString());
        //            double.TryParse(timeInterval, out minTimeToWait);
        //            return minTimeToWait;
        //        }
        //        StreamReader file = new StreamReader(ConfigurationManager.AppSettings["filePath"].ToString() + "lastTimestamp.txt");
        //        string lastValueFromFile = file.ReadLine();
        //        double.TryParse(lastValueFromFile, out readValueFromFile);
        //        file.Close();
        //        File.WriteAllText(ConfigurationManager.AppSettings["filePath"].ToString() + "lastTimestamp.txt", delta.ToString());
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //    return readValueFromFile;
        //}

        /// <summary>
        /// Make a call to this function with URL
        /// </summary>
        /// <param name="Url">Pass in url like http://www.google.com/ </param>
        /// <returns>This return a task</returns>

        //private async Task<Tuple<HttpStatusCode, HttpContent>> GetRequestStatusCode(string Url)
        //{
        //    HttpResponseMessage message = await MakeRequest(Url);
        //    return new Tuple<HttpStatusCode, HttpContent>(message.StatusCode, message.Content);
        //}

        private async Task<HttpResponseMessage> GetRequestStatusCode(string Url)
        {
            HttpResponseMessage message = await MakeRequest(Url);
            return message;
        }

        private async Task<Content> GetDataFromUrl(string Url)
        {
            Content item = new Content();

            try
            {
                Uri uri = new Uri(Url);
                currentUrl = Uri.UriSchemeHttp + Uri.SchemeDelimiter + uri.Authority + "/";

                //Tuple<HttpStatusCode, HttpContent> content = await GetRequestStatusCode(Url);
                HttpResponseMessage content = await GetRequestStatusCode(Url);

                item.Code = content.StatusCode;

                if (content.StatusCode == HttpStatusCode.OK)
                {
                    item.RawResponse = await content.Content.ReadAsStringAsync();
                }
                else
                {
                    item.RawResponse = string.Empty;
                }
                return item;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// This will get the title from the html. 
        /// </summary>
        /// <returns>Title based on meta tags or looking for title tag in the header. If nothing matches it returns empty string</returns>
        private string GetTitle()
        {
            string title = string.Empty;
            //try meta tags and if that help then do the else part 
            if (string.IsNullOrEmpty(strWebContent))
            {
                throw new Exception("Html Content is empty, cannot process this URL.");
            }
            else
            {
                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(strWebContent);

                HtmlNodeCollection nodes = html.DocumentNode.SelectNodes("//meta");

                if (nodes.Count > 0)
                {
                    foreach (var item in nodes)
                    {
                        HtmlAttributeCollection attrCol = item.Attributes;

                        if (attrCol[0].Value == "og:title")
                        {
                            if (!string.IsNullOrEmpty(attrCol[1].Value))
                            {
                                title = HttpUtility.HtmlDecode(attrCol[1].Value);
                                break;
                            }
                        }

                    }
                    if (string.IsNullOrEmpty(title))
                    {
                        title = Regex.Match(strWebContent, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
                    }
                }
                else
                {
                    title = Regex.Match(strWebContent, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
                }
            }
            if (string.IsNullOrEmpty(title))
                return string.Empty;
            return HtmlHelper.RemoveMultipleSpaces(title.Replace("\\r\\n", "").Replace("\n", string.Empty));
        }

        //private async Task<string> MakeRequest(string Url)
        //{
        //    WebRequestHandler handler = new WebRequestHandler();
        //    handler.AllowAutoRedirect = false;
        //    HttpClient client = new HttpClient(handler);
        //    try
        //    {
        //        HttpResponseMessage responseMessage = await client.GetAsync(Url);
        //        if (responseMessage.IsSuccessStatusCode && responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            var content = await responseMessage.Content.ReadAsStringAsync();
        //            return content;
        //        }
        //        return string.Empty;

        //    }
        //    catch (HttpException he)
        //    {
        //        throw he;
        //    }
        //}

        private async Task<HttpResponseMessage> MakeRequest(string Url)
        {
            WebRequestHandler handler = new WebRequestHandler
            {
                AllowAutoRedirect = true
            };
            HttpClient client = new HttpClient(handler);
            try
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                HttpResponseMessage responseMessage = await client.GetAsync(Url);
                return responseMessage;
            }
            catch (HttpException he)
            {
                throw he;
            }
        }

        private string GetContent()
        {
            if (string.IsNullOrEmpty(strWebContent))
                throw new Exception("Html content is empty, cannot process this URL");
            else
            {
                HtmlDocument html = new HtmlDocument
                {
                    OptionOutputAsXml = true
                };
                html.LoadHtml(strWebContent);

                StringBuilder sb = new StringBuilder();

                IEnumerable<HtmlNode> hasDivs;
                HtmlNode foundDiv = null;

                if (currentUrl.Contains("schematics.com"))
                {
                    //schematics has this weird case where multiple project-page-info-text divs are found. To find the right one based on observation, second div has id and exact content.
                    //This code will still fail for other cases on schematics.com because the div is not consistent.

                    hasDivs = html.DocumentNode
                              .Descendants("div")
                              .Where(div => div.HasClass("project-page-info-text"));

                    if (hasDivs.Count() > 1)
                    {
                        foreach (var node in hasDivs)
                        {
                            if (node.ParentNode.Attributes.Count > 0)
                            {
                                //The assumption is that div which as content will have a parent node with a div class project-summary-div
                                HtmlAttributeCollection attrCol = node.ParentNode.Attributes;

                                var locateDiv = (from attrnodes in attrCol
                                                 where attrnodes.Name == "id" && attrnodes.Value == "project-summary-div"
                                                 select attrnodes).ToList();

                                if (locateDiv.Count == 1)
                                {
                                    foundDiv = node;
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (currentUrl.Contains("electronicproducts.com"))
                {
                    //This is written with the assumption that EP will never have more than one div with ep_Body class
                    hasDivs = html.DocumentNode
                              .Descendants("div")
                              .Where(div => div.HasClass("ep_Body"));
                    foundDiv = hasDivs.First();
                }
                else if (currentUrl.Contains("eetimes.com"))
                {
                    //This is written with the assumption that eetimes will never have more than one <article> element
                    hasDivs = html.DocumentNode
                              .Descendants("div")
                              .Where(div => div.HasClass("articleBody"));
                    //.Descendants("articleBody");
                    foundDiv = hasDivs.First();
                }
                else if (currentUrl.Contains("www.edn.com"))
                {
                    // <div class="detail_body">
                    hasDivs = html.DocumentNode
                              .Descendants("div")
                              .Where(div => div.HasClass("entry-content"));
                    if (hasDivs != null && hasDivs.Count() > 0)
                        foundDiv = hasDivs.First();
                    else
                        return string.Empty;
                }
                else if (currentUrl.Contains("www.ebnonline.com"))
                {
                    // <div class="grayshowlinks bigsmall" style="margin-bottom: 14px;">
                    hasDivs = html.DocumentNode
                              .Descendants("div")
                              .Where(div => div.HasClass("entry-content"));
                    if (hasDivs != null && hasDivs.Count() > 0)
                        foundDiv = hasDivs.First();
                    else
                        return string.Empty;
                }
                else if (currentUrl.Contains("www.planetanalog.com"))
                {
                    // <div class="grayshowlinks bigsmall" style="margin-bottom: 14px;">
                    hasDivs = html.DocumentNode
                              .Descendants("div")
                              .Where(div => div.HasClass("entry-content"));
                    foundDiv = hasDivs.First();

                }
                else if (currentUrl.Contains("www.techonline.com"))
                {
                    // <div class="inside hide-for-small">
                    hasDivs = html.DocumentNode
                              .Descendants("div")
                              .Where(div => div.HasClass("inside"));
                    foundDiv = hasDivs.First();
                }
                else if (currentUrl.Contains("www.embedded.com"))
                {
                    // <div class="item full">
                    hasDivs = html.DocumentNode
                              .Descendants("div")
                              .Where(div => div.HasClass("entry-content"));
                    foundDiv = hasDivs.First();
                }
                else if (currentUrl.Contains("eetimes.eu"))
                {
                    //This is written with the assumption that EP will never have more than one div with ep_Body class
                    hasDivs = html.DocumentNode
                              .Descendants("div")
                              .Where(div => div.HasClass("entry-content"));
                    foundDiv = hasDivs.First();
                }
                else if (currentUrl.Contains("www.eeweb.com"))
                {
                    // Blog Content
                    // <div class="blog_entry">
                    hasDivs = GetArticleContent(html, "div", "blog_entry");
                    if (hasDivs.Count() > 0)
                    {
                        foundDiv = hasDivs.First();
                    }
                    else
                    {
                        // News Content
                        // <div class="news_wrap clearfix">
                        hasDivs = GetArticleContent(html, "div", "news_wrap");
                        if (hasDivs.Count() > 0)
                        {
                            foundDiv = hasDivs.First();
                        }
                        else
                        {
                            // Forum Content
                            // <div class="article format">
                            hasDivs = GetArticleContent(html, "div", "article");
                            if (hasDivs.Count() > 0)
                            {
                                foundDiv = hasDivs.First();
                            }
                        }
                    }

                }

                if (foundDiv != null)
                {
                    string decodedData = HttpUtility.HtmlDecode(foundDiv.InnerHtml);

                    return decodedData;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private IEnumerable<HtmlNode> GetArticleContent(HtmlDocument html, string decendants, string hasClass)
        {
            var hasDivs = html.DocumentNode.Descendants(decendants).Where(div => div.HasClass(hasClass));
            return hasDivs;
        }

        private string GetDescription()
        {
            string strDescription = string.Empty;
            try
            {
                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(strWebContent);

                HtmlNodeCollection nodes = html.DocumentNode.SelectNodes("//meta");

                if (nodes.Count > 0)
                {
                    foreach (var item in nodes)
                    {
                        HtmlAttributeCollection attrCol = item.Attributes;

                        if (attrCol[0].Value == "og:description")
                        {
                            if (!string.IsNullOrEmpty(attrCol[1].Value))
                            {
                                strDescription = attrCol[1].Value;
                                break;
                            }
                        }
                        else if (attrCol[0].Value == "description")
                        {
                            if (!string.IsNullOrEmpty(attrCol[1].Value))
                            {
                                strDescription = attrCol[1].Value;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            if (!string.IsNullOrEmpty(strDescription))
                HtmlHelper.RemoveMultipleSpaces(strDescription.Trim());
            return strDescription;
        }

        private string GetPubDate()
        {
            string pubDate = string.Empty;

            if (string.IsNullOrEmpty(strWebContent))
                throw new Exception("Html content is empty, cannot process this URL");
            else
            {
                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(strWebContent);

                HtmlNodeCollection nodes = html.DocumentNode.SelectNodes("//meta");

                if (nodes.Count > 0)
                {
                    foreach (var item in nodes)
                    {
                        HtmlAttributeCollection attrCol = item.Attributes;

                        if (attrCol[0].Value == "article:published_time")
                        {
                            if (!string.IsNullOrEmpty(attrCol[1].Value))
                            {
                                pubDate = attrCol[1].Value;
                                break;
                            }
                        }
                        else if (attrCol[0].Value == "created")
                        {
                            if (!string.IsNullOrEmpty(attrCol[1].Value))
                            {
                                pubDate = attrCol[1].Value;
                                break;
                            }
                        }

                    }
                }
            }
            return pubDate;
        }

        public List<string> GetAnchorTags(string Content, string Url)
        {
            if (string.IsNullOrEmpty(Content))
            {
                return new List<string>();
            }
            else
            {

                string baseDomain = GetDomain(Url);
                List<string> list = new List<string>();
                Regex regex = new Regex("<a[^>]*? href=\"(?<url>[^\"]+)\"[^>]*?>(?<text>.*?)</a>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

                var matches = regex.Matches(Content);

                foreach (Match m in matches)
                {
                    string hrefUrl = m.Groups["url"].Value;
                    string anchorText = m.Groups["text"].Value;

                    // check if the URL validates
                    // we're only going to process a full URL with http:// or https://
                    Regex regexUrlValidator = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&amp;=]*)?", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    var urlMatches = regexUrlValidator.Matches(hrefUrl);
                    if (urlMatches.Count > 0 && !hrefUrl.Contains(baseDomain))
                    {
                        list.Add(hrefUrl);
                    }
                    //list.Add(m.Groups["url"].Value);
                    //Console.WriteLine("URL: " + m.Groups["url"].Value + " -- Text = " + m.Groups["text"].Value);
                }

                //Regex r = new Regex("<a[^>]*? href=\"(?<url>[^\"]+)\"[^>]*?>(?<text>.*?)</a>", RegexOptions.IgnoreCase);
                //MatchCollection m = r.Matches(Content);

                //foreach (Match item in m)
                //{
                //    list.Add(item.Groups["url"].Value);
                //}
                return list;
            }
        }

        private static string GetDomain(string url)
        {
            string domain = null;
            string[] parts = url.Split('/');
            if (parts.Count() >= 3)
            {
                domain = parts[2];
            }
            return domain;
        }
    }
}
