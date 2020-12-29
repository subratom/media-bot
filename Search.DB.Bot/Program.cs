using Ninject;
using Search.DB.Bot.Classes;
using Search.ElasticSearchMedia;
using Search.ElasticSearchMedia.Implementations;
using Search.ElasticSearchMedia.Interfaces;
using Search.MediaStore.DDL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Search.DB.Bot
{
    class Program
    {

        private static readonly Queue<Task> WaitingTasks = new Queue<Task>();
        private static readonly Dictionary<string, Task> RunningTasks = new Dictionary<string, Task>();
        public static int MaxRunningTasks =Int32.Parse(ConfigurationManager.AppSettings["MaxThreads"].ToString()) ; // vary this to dynamically throttle launching new tasks 
        private static int i = 0;

        private static ISearchApi _searchApi;

        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            _searchApi = kernel.Get<ISearchApi>();

            Console.WriteLine("Starting process");

            while (true)
            {
                try
                {
                    string ConnectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
                    BotDDL bot = new BotDDL();
                    BotDDL.ConnectionString = ConnectionString;

                    List<Mappings> items = new List<Mappings>();
#if DEBUG
                    //items = bot.ReprocessItems("Embedded", 20);
#endif
                    items = bot.GetUrlsToProcess(Int32.Parse(ConfigurationManager.AppSettings["MaxItems"].ToString()));
                    Process(items);

                    Thread.Sleep(120000); //2 min wait
                    items = new List<Mappings>();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }


        private static void DeleteEntries(string Url)
        {
            SearchHits items = _searchApi.Url(Url.ToLower());
            foreach (var item in items.Hits)
            {
                Console.WriteLine(string.Format("\ritem is {0} and url is {1}", item.Source.Site, item.Source.Url));
                var client = new Index(_searchApi);
                client.DeleteDocument(item.Id);
            }
        }

        private static void ResetEntries()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            BotDDL botReset = new BotDDL();
            try
            {
                botReset.ResetProcessedUrls();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }//ResetProcessedUrls
        }

        private static void Process(List<Mappings> items)
        {
            try
            {

                if (items != null && items.Count > 0)
                {
                    var tokenSource = new CancellationTokenSource();
                    var token = tokenSource.Token;
                    Worker.Done = new Worker.DoneDelegate(WorkerDone);

                    foreach (var item in items)
                    {
                        WaitingTasks.Enqueue(new Task(id => new Worker().DoWork((string)id, item.Url, token), item.Url, token));
                        i++;
                    }
                    LaunchTasks();
                }
                else
                {
                    Console.WriteLine("\rNo items found. Process  will go to sleep for 10 mins. Process stopped at " + DateTime.Now);
                    Thread.Sleep(600000);
                }
                i = 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
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

        static async void LaunchTasks()
        {
            try
            {
                while ((WaitingTasks.Count > 0) || (RunningTasks.Count > 0))
                {
                    // launch tasks when there's room
                    while ((WaitingTasks.Count > 0) && (RunningTasks.Count < MaxRunningTasks))
                    {
                        Task task = WaitingTasks.Dequeue();
                        if (!RunningTasks.ContainsKey((string)task.AsyncState))
                        {
                            lock (RunningTasks) RunningTasks.Add((string)task.AsyncState, task);
                            task.Start();
                            Console.WriteLine("\r Processing " + (string)task.AsyncState.ToString());
                        }
                    }
                    UpdateConsole();
                    await Task.Delay(15000); // wait before checking again
                }
                UpdateConsole();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        static void UpdateConsole()
        {
            Console.Write(string.Format("\rwaiting: {0,3:##0}  running: {1,3:##0} ", WaitingTasks.Count, RunningTasks.Count));
        }

        static void WorkerDone(string id)
        {
            lock (RunningTasks) RunningTasks.Remove(id);
        }
    }
}
