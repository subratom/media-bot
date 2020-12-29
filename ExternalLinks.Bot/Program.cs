using Search.API.Classes;
using Search.Helpers;
using Search.MediaStore.DDL;
using Search.Scrapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace ExternalLinks.Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Beginning Process To Look up External Links");
            Process();
        }

        private static void Process()
        {


            string ConnectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            BotDDL bot = new BotDDL();

            long currentCount = 0;

            while (true)
            {
                try
                {
                    List<string> urls = new List<string>();
                    List<Mappings> items = bot.GetUrlsToProcessExternalLinks(100);
                    if (items != null)
                    {
                        currentCount += items.Count;
                        Console.WriteLine($"\r {currentCount} processed so far");

                        foreach (var item in items)
                        {
                            urls.Add(item.Url);
                        }

                        ProcessUrls(urls).Wait();
                        Console.WriteLine("\rSleeping for 5 mins at " + DateTime.Now);
                        Thread.Sleep(300000);
                        Console.WriteLine("\rWaking up from 5 min sleep at " + DateTime.Now);
                    }
                    else
                    {
                        Console.WriteLine("No items found. Process  will go to sleep for 10 mins. Process stopped at " + DateTime.Now);
                        Thread.Sleep(600000);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static async Task ProcessUrls(List<string> urls)
        {
            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
                BotDDL botInsert = new BotDDL();
                int currentUrl = 1;
                ScrapeWeb bot = new ScrapeWeb();
                Content obj;

                foreach (string url in urls)
                {
                    try
                    {
                        //Don't run this code. This will fail as the SP looks for bit values and current code is modified to look for 
                        // int values. After the initial pass there needs to be a change so that proper int values are inserted.

                        obj = await bot.ReturnArticleObject(url);
                        if (obj != null && !string.IsNullOrEmpty(obj.Contents) && obj.Code == HttpStatusCode.OK)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\r Processing :" + url);
                            Console.ResetColor();

                            List<string> anchorItems = bot.GetAnchorTags(obj.RawBodyContents, url);

                            if (anchorItems.Count > 0)
                            {
                                ExternalLinkParent parentItem = CreateParentObject(url);
                                try
                                {
                                    bool success = botInsert.InsertExternalParentLinks(parentItem, anchorItems);
                                    if (!success)
                                    {
                                        botInsert.UpdateExternalUrlFlag(url, true);
                                        //botInsert.UpdateExternalUrlFlag(url, (int)ItemStatusEnum.Failed);
                                        Console.WriteLine($"{0} was unsuccessfully processed");
                                    }
                                    else
                                    {
                                        botInsert.UpdateExternalUrlFlag(url, true);
                                        //botInsert.UpdateExternalUrlFlag(url, (int)ItemStatusEnum.Processed);
                                    }
                                }
                                catch (Exception parentException)
                                {

                                    botInsert.UpdateExternalUrlFlag(url, true);
                                    //botInsert.UpdateExternalUrlFlag(url, (int)ItemStatusEnum.Failed);
                                    Console.WriteLine();
                                    Console.WriteLine(url);
                                    Console.WriteLine(parentException);
                                    Console.WriteLine("--------------------------------------------------------------------------");
                                }

                                //Insert in parent table 
                            }
                            else
                            {
                                botInsert.UpdateExternalUrlFlag(url, true);
                                //botInsert.UpdateExternalUrlFlag(url, (int)ItemStatusEnum.Processed);
                            }
                        }
                        else
                        {
                            botInsert.UpdateExternalUrlFlag(url, true);
                            //botInsert.UpdateExternalUrlFlag(url, (int)ItemStatusEnum.Skip);
                        }
                    }
                    catch (Exception e)
                    {
                        //botInsert.UpdateExternalUrlFlag(url, (int)ItemStatusEnum.Skip);
                        botInsert.UpdateExternalUrlFlag(url, true);
                        Console.WriteLine();
                        Console.WriteLine(url);
                        Console.WriteLine(e);
                        Console.WriteLine("--------------------------------------------------------------------------");
                    }

                    double currentPercentage = (int)Math.Round((double)(100 * currentUrl) / urls.Count);
                    currentUrl++;
                    Console.Write(string.Format("\r{0:0.###} % completed", currentPercentage));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static ExternalLinkParent CreateParentObject(string url)
        {
            ExternalLinkParent parentItem = new ExternalLinkParent
            {
                SiteName = url.GetSiteName(),
                SiteUrl = url,
                DateCreated = DateTime.Now.Date,
                IsProcessed = false
            };
            return parentItem;
        }
    }
}
