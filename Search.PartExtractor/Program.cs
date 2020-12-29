using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Search.MediaStore;
using Search.MediaStore.DDL;
using System.Configuration;
using System.Threading;
using System.IO;
using Search.ElasticSearchMedia;
using Search.PartExtractor;
using Search.API.Classes;
using Ninject;
using Search.ElasticSearchMedia.Interfaces;
using Search.ElasticSearchMedia.Implementations;

namespace Search.PartExtractor
{
    class Program
    {

        private static ISearchApi searchApi;
        private static void UpdateMappingItem(Mappings item, int PartTried = 0, bool IsPartProcessed = true)
        {
            /* Update database to indicate that the part was processed*/

            BotDDL bot = new BotDDL();
            BotDDL.ConnectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            Mappings mappings = new Mappings();
            Mappings updatedItem = mappings;
            updatedItem.Id = item.Id;
            updatedItem.SiteName = item.SiteName;
            updatedItem.Url = item.Url;
            updatedItem.IsUrlProcessed = item.IsUrlProcessed;
            updatedItem.IsPartProcessed = IsPartProcessed;
            updatedItem.DateCreated = item.DateCreated;
            updatedItem.DateModified = item.DateModified;
            updatedItem.PartMaxTries = PartTried;
            updatedItem.UrlMaxTries = item.UrlMaxTries;
            updatedItem.HttpCode = item.HttpCode;
            bot.UpdateUrl(updatedItem);
            /*End of update*/
        }

        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Load(System.Reflection.Assembly.GetExecutingAssembly());
            searchApi = kernel.Get<ISearchApi>();

            BotDDL.ConnectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            BotDDL bot = new BotDDL();

            string appPath = Directory.GetCurrentDirectory();

            System.IO.StreamWriter candidatePartNumbersFile = new System.IO.StreamWriter(appPath + @"\candidates.txt", true);

            // logging all vetted part numbers from SE
            System.IO.StreamWriter seVettedPartNumbersFile = new System.IO.StreamWriter(appPath + @"\vetted.txt", true);

            while (true)
            {
                try
                {
                    List<Mappings> items = bot.GetPartsToProcess(100);
                    if (items == null)
                    {
                        Console.WriteLine("Null items retrieved.");
                    }
                    else if (items.Count == 0)
                    {
                        Console.WriteLine(string.Format("\r No items found at {0}, app sleeping for 15 mins ", DateTime.Now));
                        Thread.Sleep(900000);
                        Console.Clear();
                        Console.WriteLine(string.Format("\r Waking up from 15 mins sleep at {0}", DateTime.Now));
                    }
                    else
                    {
                        int currentUrl = 1;

                        foreach (var item in items)
                        {
                            try
                            {
                                Console.WriteLine($"+-----------------------------------------------------------------------+");

                                SearchApi search = new SearchApi();
                                var searchResult = search.Url(item.Url.ToLower(), 25, 0);
                                //totalResults = searchResult.Total;

                                if (searchResult.TotalItems == 0)
                                {
                                    UpdateMappingItem(item, (item.PartMaxTries + 1), false);
                                }
                                else
                                {
                                    var documents = searchResult;

                                    foreach (var document in documents.Hits)
                                    {
                                        Articles article = document.Source;
                                        string content = article.Content;
                                        List<string> parts = RuleMatcher.Match(content);
                                        Console.WriteLine(article.Title);

                                        if (parts.Count == 0)
                                        {
                                            Console.WriteLine("no parts found");
                                            UpdateMappingItem(item, item.PartMaxTries + 1);
                                        }
                                        else
                                        {

                                            Console.WriteLine("--- Before SE Check---------------------------------------------------");
                                            foreach (var part in parts)
                                            {
                                                Console.WriteLine(part);
                                                candidatePartNumbersFile.WriteLine(part);
                                                candidatePartNumbersFile.Flush();
                                            }

                                            CleanPartsList(parts).Wait();

                                            Console.WriteLine("+-- After SE Check---------------------------------------------------");
                                            foreach (var part in parts)
                                            {
                                                Console.WriteLine($"| {part}");
                                                seVettedPartNumbersFile.WriteLine(part);
                                                seVettedPartNumbersFile.Flush();
                                            }
                                            Console.WriteLine("+--------------------------------------------------------------------");

                                            Index updatePartInfo = new Index(searchApi);
                                            if (parts.Count > 0)
                                            {
                                                article.PartNumbers = parts.ToArray();
                                                updatePartInfo.Upsert(article);
                                            }
                                            else
                                            {
                                                article.PartNumbers = null;
                                                updatePartInfo.ClearParts(article);
                                            }
                                            UpdateMappingItem(item);
                                        }

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                UpdateMappingItem(item, (item.PartMaxTries + 1), false);
                                Console.WriteLine(ex);
                            }

                            Console.WriteLine($"+-----------------------------------------------------------------------+");
                            Console.WriteLine();

                            double currentPercentage = Math.Round((double)(100 * currentUrl) / items.Count);
                            Console.WriteLine(string.Format("\r{0:0.##}% processed ", currentPercentage));
                            currentUrl += 1;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(" Error occured. Here are the details" + e.StackTrace);
                }

            }

        }

        private static async Task CleanPartsList(List<string> partsList)
        {
            bool result;

            SiliconExpertAPI se = new SiliconExpertAPI();
            if (se.LoginStatus == true)
            {
                int count = partsList.Count;
                int i = 0;
                while (i < count)
                {
                    result = await se.ValidatePart(partsList[i]);
                    if (result == false)
                    {
                        partsList.RemoveAt(i);
                        count--;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        private static async Task TestAPI()
        {
            SiliconExpertAPI se = new SiliconExpertAPI();
            if (se.LoginStatus == true)
            {
                string[] parts = new string[] { "X1Y1Z1", "RS-232", "BAV99", "BAV-99", "BAV99T", "BAV99XXXJJ", "LM317", "ABC123", "THIS-THING" };

                bool result = false;

                foreach (string part in parts)
                {
                    result = await se.ValidatePart(part);
                    Console.WriteLine($"{part} validation result {result}");
                }
            }
        }
    }
}