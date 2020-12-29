using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Search.MediaStore.DDL;
using Search.MediaStore;
using System.Configuration;
using System.Net.Http;
using System.Xml;
using Search.Helpers;
using HtmlAgilityPack;
using Search.Scrapper;
using Search.ElasticSearchMedia;
using Search.MediaStore.DDL.Helpers;
using Search.API.Classes;
using System.Net;
using Search.ElasticSearchMedia.Interfaces;
using Ninject;
using System.Reflection;
using System.Threading;
using Search.ElasticSearchMedia.Implementations;

namespace Search.Sitemap.Bot
{
    class Program
    {
        //private static const long UnixEpochSeconds = UnixEpochTicks / TimeSpan.TicksPerSecond; // 62,135,596,800
        private static ISearchApi searchApi;

        public static List<string> properties = new List<string>();

        static void Main(string[] args)
        {

            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            searchApi = kernel.Get<ISearchApi>();

            bool bStart = true;
            string appPath = Directory.GetCurrentDirectory();

            while (bStart)
            {
                try
                {
                    properties = ReadFiles(appPath);
                }
                catch (Exception readEx)
                {
                    Console.WriteLine("Unable to read the file, please check the path & look for properties.txt file");
                    return;
                }

                string ConnectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

                BotDDL bot = new BotDDL();
                BotDDL.ConnectionString = ConnectionString;
                long currentFile = 1;

                foreach (var property in properties)
                {
                    //This will return items from sitemap index files. Each line from that will point to another set of file which contains urls
                    //await ReadSiteMapIndexFile(property).Wait();

                    try
                    {
                        Task<string> findFile = Task.Run(() => ReadSiteMapIndexFile(property));
                        findFile.Wait();
                        if (findFile.Result != null && !string.IsNullOrEmpty(findFile.Result.ToString()))
                        {
                            List<string> yearlyItems = ParseSitemapIndexFile(property + "/sitemap.xml");
                            List<string> Urls = new List<string>();
                            string strStartYear = "", strEndYear = "";

                            try
                            {
                                strStartYear = ConfigurationManager.AppSettings["startYear"].ToString();
                                strEndYear = ConfigurationManager.AppSettings["endYear"].ToString();
                            }
                            catch (Exception e)
                            {
                                strStartYear = strStartYear == "" ? DateTime.Now.Year.ToString() : ConfigurationManager.AppSettings["startYear"].ToString();
                                strEndYear = DateTime.Now.Year.ToString();
                            }

                            if (string.IsNullOrEmpty(strStartYear))
                            {
                                strStartYear = DateTime.Now.Year.ToString();
                            }

                            if (string.IsNullOrEmpty(strEndYear))
                            {
                                strEndYear = DateTime.Now.Year.ToString();
                            }

                            if (property.ToLower().Contains("electronicproducts"))
                            {
                                foreach (var eachYear in yearlyItems)
                                {
                                    try
                                    {
                                        Uri uri = new Uri(eachYear);
                                        if (Int32.Parse(uri.Segments[1].Substring(0, 4)) < Int32.Parse(strStartYear))
                                        {
                                            continue;
                                        }
                                        else if (Int32.Parse(uri.Segments[1].Substring(0, 4)) > Int32.Parse(strEndYear))
                                        {
                                            continue;
                                        }
                                        else if (Int32.Parse(uri.Segments[1].Substring(0, 4)) >= Int32.Parse(strStartYear) && (Int32.Parse(uri.Segments[1].Substring(0, 4)) <= Int32.Parse(strEndYear)))
                                        {
                                            List<XmlNode> articleItems = new List<XmlNode>();
                                            articleItems = ParseSitemapFile(eachYear);
                                            foreach (var article in articleItems)
                                            {
                                                Console.Write("\r{0} item(s) processed for {1}", currentFile++, property);

                                                Mappings i = new Mappings();
                                                try
                                                {
                                                    //Rules to clean up the URL
                                                    if (article.InnerText.Contains("electronicproducts") && (article.InnerText.ToLower().Contains("/companies/") || article.InnerText.ToLower().Contains("/videos/") || article.InnerText.ToLower().Contains("/reference_design/") || article.InnerText.ToLower().Contains("/company_community/")))
                                                    {
                                                        continue;
                                                    }

                                                    if (bot.CreateMapping(article))
                                                    {
                                                        Urls.Add(XmlHelpers.GetXmlNodeValue(article, "loc")); //i.Url
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine(e.Message);
                                                }
                                            }
                                        }
                                        else if (Int32.Parse(uri.Segments[1].Substring(0, 4)) == Int32.Parse(strStartYear) && (Int32.Parse(uri.Segments[1].Substring(0, 4)) == Int32.Parse(strEndYear)))
                                        {
                                            List<XmlNode> articleItems = new List<XmlNode>();
                                            articleItems = ParseSitemapFile(eachYear);
                                            foreach (var article in articleItems)
                                            {
                                                Console.Write("\r{0} item processed  ", currentFile++);

                                                Mappings i = new Mappings();
                                                try
                                                {
                                                    //Rules to clean up the URL
                                                    if (article.InnerText.Contains("electronicproducts") && (article.InnerText.ToLower().Contains("/companies/") || article.InnerText.ToLower().Contains("/videos/") || article.InnerText.ToLower().Contains("/reference_design/") || article.InnerText.ToLower().Contains("/company_community/")))
                                                    {
                                                        continue;
                                                    }

                                                    if (bot.CreateMapping(article))
                                                    {
                                                        Urls.Add(XmlHelpers.GetXmlNodeValue(article, "loc")); //i.Url
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine(e.Message);
                                                }
                                            }
                                        }
                                        Console.WriteLine("\r Done processing items. Total " + currentFile + " items processed for the year " + eachYear);
                                    }
                                    catch (Exception yearEx)
                                    {
                                        Console.WriteLine("Error processing " + eachYear + " for " + property + " Error details are : " + yearEx);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var eachYear in yearlyItems)
                                {
                                    try
                                    {
                                        Uri uri = new Uri(eachYear);
                                        int startYearFromurl = 0;
                                        //sitemap-posttype-post.2014.xml
                                        if (uri.Segments[1].Contains("posttype"))
                                        {
                                            string[] yearSplit = uri.Segments[1].ToString().Split('.');
                                            if (yearSplit.Length == 3)
                                            {
                                                Int32.TryParse(yearSplit[1], out startYearFromurl);
                                            }
                                        }

                                        if (startYearFromurl < Int32.Parse(strStartYear))
                                        {
                                            continue;
                                        }
                                        else if (startYearFromurl > Int32.Parse(strEndYear))
                                        {
                                            continue;
                                        }
                                        else if (startYearFromurl >= Int32.Parse(strStartYear) && (startYearFromurl <= Int32.Parse(strEndYear)))
                                        {
                                            List<XmlNode> articleItems = new List<XmlNode>();
                                            articleItems = ParseSitemapFile(eachYear);
                                            foreach (var article in articleItems)
                                            {
                                                Console.Write("\r{0} item(s) processed for {1}", currentFile++, property);

                                                Mappings i = new Mappings();
                                                try
                                                {
                                                    
                                                    if (bot.CreateMapping(article))
                                                    {
                                                        Urls.Add(XmlHelpers.GetXmlNodeValue(article, "loc")); //i.Url
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine(e.Message);
                                                }
                                            }
                                        }
                                        else if (startYearFromurl == Int32.Parse(strStartYear) && (startYearFromurl == Int32.Parse(strEndYear)))
                                        {
                                            List<XmlNode> articleItems = new List<XmlNode>();
                                            articleItems = ParseSitemapFile(eachYear);
                                            foreach (var article in articleItems)
                                            {
                                                Console.Write("\r{0} item processed  ", currentFile++);

                                                Mappings i = new Mappings();
                                                try
                                                {
                                                    if (bot.CreateMapping(article))
                                                    {
                                                        Urls.Add(XmlHelpers.GetXmlNodeValue(article, "loc")); //i.Url
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine(e.Message);
                                                }
                                            }
                                        }
                                        Console.WriteLine("\r Done processing items. Total " + currentFile + " items processed for the year " + eachYear);
                                    }
                                    catch (Exception yearEx)
                                    {
                                        Console.WriteLine("Error processing " + eachYear + " for " + property + " Error details are : " + yearEx);
                                    }
                                }
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Error processing file, Error details are " + e.StackTrace);
                    }
                }
                Console.WriteLine("\rSleeping for 5 mins at " + DateTime.Now);
                Thread.Sleep(300000);
                Console.WriteLine("\rWaking up from 5 min sleep at " + DateTime.Now);
            }
        }

        private static List<string> ReadFiles(string appPath)
        {
            List<string> property = new List<string>();
            try
            {
                DirectoryInfo di = new DirectoryInfo(appPath);
                FileInfo fi = new FileInfo(appPath + @"\properties.txt"); //di.GetFiles(@"properties.txt");
                using (StreamReader sr = fi.OpenText())
                {
                    string line = string.Empty;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.EndsWith("/"))
                        {
                            property.Add(line.Substring(0, (line.Length - 1)));
                        }
                        else
                        {
                            property.Add(line);
                        }
                    }
                }
                return property;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// Make this function generic for all type of date functions
        /// </summary>
        /// <param name="property">Date</param>
        /// <returns></returns>
        /// 

        private static async Task<string> ReadSiteMapIndexFile(string property)
        {
            //File to look for is sitemap.xml
            HttpClient client = new HttpClient();
            HttpResponseMessage response;

            try
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                response = await client.GetAsync(property + "/sitemap.xml");
            }
            catch (HttpRequestException he)
            {
                throw he;
            }
            catch (Exception e)
            {
                throw e;
            }
            return response.StatusCode.ToString();
        }

        private static List<string> ParseSitemapIndexFile(string url)
        {
            XmlDocument rssXmlDoc = new XmlDocument();

            // Load the Sitemap file from the Sitemap URL
            rssXmlDoc.Load(url);

            StringBuilder sitemapContent = new StringBuilder();

            List<string> indexItems = new List<string>();

            // Iterate through the top level nodes and find the "urlset" node. 
            foreach (XmlNode topNode in rssXmlDoc.ChildNodes)
            {
                if (topNode.Name.ToLower() == "sitemapindex")
                {
                    // Use the Namespace Manager, so that we can fetch nodes using the namespace
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(rssXmlDoc.NameTable);
                    nsmgr.AddNamespace("ns", topNode.NamespaceURI);

                    // Get all URL nodes and iterate through it.
                    XmlNodeList urlNodes = topNode.ChildNodes;
                    foreach (XmlNode urlNode in urlNodes)
                    {
                        // Get the "loc" node and retrieve the inner text.
                        XmlNode locNode = urlNode.SelectSingleNode("ns:loc", nsmgr);
                        string link = locNode != null ? locNode.InnerText : "";
                        indexItems.Add(link);

                        // Add to our string builder.
                        //sitemapContent.Append(link + "");
                    }
                }
            }

            return indexItems; //sitemapContent.ToString();
        }

        private static List<XmlNode> ParseSitemapFile(string url)
        {
            XmlDocument rssXmlDoc = new XmlDocument();

            List<XmlNode> articleList = new List<XmlNode>();

            // Load the Sitemap file from the Sitemap URL
            rssXmlDoc.Load(url);

            StringBuilder sitemapContent = new StringBuilder();

            // Iterate through the top level nodes and find the "urlset" node. 
            foreach (XmlNode topNode in rssXmlDoc.ChildNodes)
            {
                if (topNode.Name.ToLower() == "urlset")
                {
                    // Use the Namespace Manager, so that we can fetch nodes using the namespace
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(rssXmlDoc.NameTable);
                    nsmgr.AddNamespace("ns", topNode.NamespaceURI);

                    // Get all URL nodes and iterate through it.
                    XmlNodeList urlNodes = topNode.ChildNodes;
                    foreach (XmlNode urlNode in urlNodes)
                    {
                        // Get the "loc" node and retrieve the inner text.
                        XmlNode locNode = urlNode.SelectSingleNode("ns:loc", nsmgr);
                        string link = locNode != null ? locNode.InnerText : "";

                        // Add to our string builder.
                        //sitemapContent.Append(link + "");

                        articleList.Add(urlNode);
                    }
                }
            }

            return articleList; // sitemapContent.ToString();
        }

        public static async Task ProcessUrls(List<string> urls)
        {
            ScrapeWeb bot = new ScrapeWeb();
            int currentCount = 1;

            string ConnectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            BotDDL.ConnectionString = ConnectionString;
            BotDDL botUpdate = new BotDDL();

            Content obj;
            foreach (string url in urls)
            {
                // Console.WriteLine($"Scraping {url}. . .  \n");
                Articles article = new Articles();

                Mappings itemUpdate = botUpdate.GetItemByUrl(url);

                try
                {
                    obj = await bot.ReturnArticleObject(url);

                    if (obj.Code != System.Net.HttpStatusCode.OK)
                    {
                        itemUpdate.HttpCode = obj.Code;
                        UpdateMappingItem(itemUpdate, (itemUpdate.UrlMaxTries + 1), false);
                    }
                    else
                    {

                        string title = obj.Title;
                        string content = obj.Contents;
                        string description = obj.Description;

                        article.Site = url.GetSite();
                        article.Content = content;
                        article.Title = title;
                        article.Url = url.ToLower();
                        article.Description = description;
                        string strThumbNail = HtmlHelper.GetImageUrl(url, obj.RawResponse);
                        if (!string.IsNullOrEmpty(strThumbNail))
                        {
                            //This condition needs to be added to remove ?n=<number> from EP thumbnails
                            if (strThumbNail.Contains("?"))
                            {
                                article.ImageUrl = strThumbNail.Substring(0, strThumbNail.IndexOf("?")).Replace("http:", "https:");
                            }
                        }
                        else
                        {
                            article.ImageUrl = string.IsNullOrEmpty(strThumbNail) ? article.Url.GetDefaultImageUrls() : strThumbNail.Replace("http:", "https:");
                        }


                        // DateTime.ParseExact(_Modified, dateformat, CultureInfo.InvariantCulture);
                        string articleDate = HtmlHelper.GetPublishDate(url, obj.RawResponse);
                        if (string.IsNullOrEmpty(articleDate))
                        {
                            article.Pubdate = DateTime.Now;
                        }
                        else
                        {
                            article.Pubdate = DateTime.Parse(articleDate);
                        }

                        var client = new Index(searchApi);
                        var result = client.Upsert(article);
                        itemUpdate.HttpCode = obj.Code;
                        if (result)
                        {
                            itemUpdate.DateModified = DateTime.Parse(articleDate);
                            UpdateMappingItem(itemUpdate);
                        }
                        else
                        {
                            int tries = itemUpdate.UrlMaxTries + 1;
                            bool IsUrlProcessed = false;
                            UpdateMappingItem(itemUpdate, tries, IsUrlProcessed);
                        }

                        //string title = obj.Title;
                        //string content = obj.Contents;
                        //string description = obj.Description;

                        //article.Content = content;
                        //article.Title = title;
                        //article.Url = url;

                        //article.Pubdate = url.Contains("edn.com") ? DateTime.Parse(obj.PubDate) : (string.IsNullOrEmpty(HtmlHelper.GetPublishDate(url, obj.RawResponse)) ? DateTime.Now : DateTime.Parse(HtmlHelper.GetPublishDate(url, obj.RawResponse)));
                        //article.Description = description;

                        //if (article.Pubdate != null)
                        //{
                        //    itemUpdate.HttpCode = obj.Code;
                        //    var client = new Index();
                        //    var result = client.Upsert(article);
                        //    if (result)
                        //    {
                        //        itemUpdate.DateModified = url.Contains("edn.com") ? DateTime.Parse(obj.PubDate) : article.Pubdate;
                        //        UpdateMappingItem(itemUpdate);
                        //    }
                        //    else
                        //    {
                        //        UpdateMappingItem(itemUpdate, (itemUpdate.UrlMaxTries + 1), false);
                        //    }
                        //    double currentPercentage = (int)Math.Round((double)(100 * currentCount) / urls.Count);

                        //    Console.Write(string.Format("\r{0:0.###} % completed", currentPercentage));
                        //}
                        //else
                        //{
                        //    Console.WriteLine($" Skipping ");
                        //}
                    }
                }
                catch (Exception e)
                {

                    UpdateMappingItem(itemUpdate, itemUpdate.UrlMaxTries + 1, false);
                    Console.WriteLine();
                    Console.WriteLine(url);
                    Console.WriteLine(e);
                    Console.WriteLine("--------------------------------------------------------------------------");
                }
                currentCount += 1;
            }

            Console.WriteLine("Finished scraping . . . press enter to query");

        }

        public static void Compress(FileInfo fileToCompress)
        {
            using (FileStream originalFileStream = fileToCompress.OpenRead())
            {
                if ((File.GetAttributes(fileToCompress.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
                {
                    using (FileStream compressedFileStream = File.Create(fileToCompress.FullName + ".gz"))
                    {
                        using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                        {
                            originalFileStream.CopyTo(compressionStream);
                            Console.WriteLine("Compressed {0} from {1} to {2} bytes.",
                                fileToCompress.Name, fileToCompress.Length.ToString(), compressedFileStream.Length.ToString());
                        }
                    }
                }
            }
        }

        //public static async Task Decompress(FileInfo fileToDecompress)
        //{
        //    List<string> urls = new List<string>();
        //    using (FileStream originalFileStream = fileToDecompress.OpenRead())
        //    {
        //        string currentFileName = fileToDecompress.FullName;
        //        string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

        //        using (FileStream decompressedFileStream = File.Create(newFileName))
        //        {
        //            using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
        //            {
        //                decompressionStream.CopyTo(decompressedFileStream);
        //                //Read the file and pass the urls to the URLs var
        //                await ProcessUrls(urls);
        //                //Console.WriteLine("Decompressed: {0}", fileToDecompress.Name);
        //            }
        //        }
        //    }
        //}

        public static double ToUnixTimeSeconds(DateTime currentDate)
        {
            // Truncate sub-second precision before offsetting by the Unix Epoch to avoid
            // the last digit being off by one for dates that result in negative Unix times.
            //
            // For example, consider the DateTimeOffset 12/31/1969 12:59:59.001 +0
            //   ticks            = 621355967990010000
            //   ticksFromEpoch   = ticks - UnixEpochTicks                   = -9990000
            //   secondsFromEpoch = ticksFromEpoch / TimeSpan.TicksPerSecond = 0
            //
            // Notice that secondsFromEpoch is rounded *up* by the truncation induced by integer division,
            // whereas we actually always want to round *down* when converting to Unix time. This happens
            // automatically for positive Unix time values. Now the example becomes:
            //   seconds          = ticks / TimeSpan.TicksPerSecond = 62135596799
            //   secondsFromEpoch = seconds - UnixEpochSeconds      = -1
            //
            // In other words, we want to consistently round toward the time 1/1/0001 00:00:00,
            // rather than toward the Unix Epoch (1/1/1970 00:00:00).

            //DateTime UtcDateTime = currentDate;
            //long seconds = UtcDateTime.Ticks / TimeSpan.TicksPerSecond;
            //return seconds - UnixEpochSeconds;

            var dateTime = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, currentDate.Hour, currentDate.Minute, currentDate.Second, DateTimeKind.Local);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return ((dateTime.ToUniversalTime() - epoch).TotalSeconds);

        }

        private static void UpdateMappingItem(Mappings item, int UrlMaxTries = 0, bool IsUrlProcessed = true) //Mappings itemfound, int tries = 0, bool IsUrlProcessed = true
        {
            /* Update database to indicate that the part was processed*/

            BotDDL bot = new BotDDL();
            BotDDL.ConnectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            try
            {
                Mappings updatedItem = new Mappings
                {
                    Id = item.Id,
                    SiteName = item.SiteName,
                    Url = item.Url,
                    IsUrlProcessed = IsUrlProcessed,
                    IsPartProcessed = false,
                    DateCreated = item.DateCreated,
                    DateModified = item.DateModified,
                    PartMaxTries = item.PartMaxTries,
                    UrlMaxTries = UrlMaxTries,
                    HttpCode = item.HttpCode,
                    UpdatedUrl = true
                };
                bot.UpdateUrl(updatedItem);
                /*End of update*/
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
