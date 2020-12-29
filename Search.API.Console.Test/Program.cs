using Search.API.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Search.Helpers;
using System.IO;
using Search.API.Console.Test.Classes;

namespace Search.API.Console.Test
{
    class Program
    {


        static void Main()
        {
            //ProcessUrls().Wait();

            //string url = "https://www.eetimes.com/email.asp?url=https%3A%2F%2Fwww.eetimes.com%2Fdocument.asp%3Fdoc_id%3D1333193%26page_number%3D2&title=Taiwan%20Has%20%27Big%20Data-Free%27%20Social%20Platform%20%7C%20EE%20Times";
            //string output = url.CleanUrl();

            //return;

            ReadJsonFile();

        }

        private static void ReadJsonFile()
        {
            string filePath = Directory.GetCurrentDirectory() + @"\media.json";

            List<ESearchContainer> _items = new List<ESearchContainer>();

            using (StreamReader sr = new StreamReader(filePath))
            {
                using (JsonTextReader reader = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer();
                    ESearchContainer searchContainerItem = new ESearchContainer();
                    while(reader.Read())
                    {
                        searchContainerItem = serializer.Deserialize<ESearchContainer>(reader);
}
                }
            }
            //throw new NotImplementedException();
        }

        //static async Task ProcessUrls()
        //{
        //    List<string> blogUrls = new List<string>()
        //    {
        //        "https://www.eeweb.com/blog/aarone_joles/eclipse-led-and-pov-cameras",
        //        "https://www.eeweb.com/blog/aaron_clarke/featured-engineer43",
        //        "https://www.eeweb.com/blog/aaron_franklin/featured-engineer131",
        //        "https://www.eeweb.com/blog/aaron_franklin/replacing-silicon-with-carbon-nanotubes-why-its-still-worth-considering",
        //        "https://www.eeweb.com/blog/abby_white/launch-x431-diagnostic",
        //        "https://www.eeweb.com/blog/abdullah_omar_naasif/pic-microcontroller-tutorial-for-beginners",
        //        "https://www.eeweb.com/blog/abey_francis/industry-and-automation-systems",
        //        "https://www.eeweb.com/blog/abolfazl_razi/multiple-access-for-passive-sensors",
        //        "https://www.eeweb.com/blog/ab_ahmad/p-c-battery",
        //        "https://www.eeweb.com/blog/adaeze_ohiaeri/the-future-and-electronics",
        //        "https://www.eeweb.com/blog/adam_c/an-oscilloscope-in-your-pocket",
        //        "https://www.eeweb.com/blog/adam_fabio/featured-engineer48",
        //        "https://www.eeweb.com/blog/adam_jackson/bike-lights-a-buyers-guide",
        //        "https://www.eeweb.com/blog/adam_taylor_2/make-something-awesome-with-the-99-fpga-based-arty-development-board",
        //        "https://www.eeweb.com/blog/adam_taylor_2/the-benefits-of-hw-sw-co-simulation-for-zynq-based-designs",
        //        "https://www.eeweb.com/blog/adam_taylor_2/revision-amazing-abstractions",
        //        "https://www.eeweb.com/blog/admin/are-you-gaining-what-you-should-from-your-antenna",
        //        "https://www.eeweb.com/blog/admin/measurement-uncertainty-and-cispr-11",
        //        "https://www.eeweb.com/blog/adolfo_garcia/saving-around-the-house-part-i",
        //        "https://www.eeweb.com/blog/adolfo_garcia/redefining-a-new-state-of-the-art-in-microampere-current-sense-amplifiers",
        //        "https://www.eeweb.com/blog/adolfo_garcia/quickly-prototyping-smt-components-what-methods-are-best",
        //        "https://www.eeweb.com/blog/adolfo_garcia/the-engineering-interview-interesting-and-challenging-questions-part-i",
        //        "https://www.eeweb.com/blog/adolfo_garcia/2012-a-great-year-considering",
        //        "https://www.eeweb.com/blog/adolfo_garcia/balanced-audio-line-receivers-and-the-physics-of-fields",
        //        "https://www.eeweb.com/blog/adolfo_garcia/electronics-engineering-and-the-art-of-automatic-transmission-repair",
        //        "https://www.eeweb.com/blog/avago_technologies/avago-optocouplers-in-intrinsic-safety-applications"
        //    };

        //    List<string> newsUrls = new List<string>()
        //    {
        //        "https://www.eeweb.com/news/4-channel-with-mic-amplifier-and-pll",
        //        "https://www.eeweb.com/news/electronic-compass-with-hall-sensor-technology",
        //        "https://www.eeweb.com/blog/aarone_joles/eclipse-led-and-pov-cameras",
        //        "https://www.eeweb.com/company-news/allegro_microsystems/3-a-synchronous-buck-regulator-ic",
        //        "https://www.eeweb.com/news/high-performance-high-frequency-drmos-device",
        //        "https://www.eeweb.com/electronics-forum/electrical-legislation-standards-codes-quiz",
        //        "https://www.eeweb.com/electronics-forum/how-can-i-locate-a-wireless-sensor-network-root-node-base-station",
        //        "https://www.eeweb.com/electronics-forum/directory-on-general-electric",
        //        "https://www.eeweb.com/electronics-forum/smt-500-ton-press-machine",
        //        "https://www.eeweb.com/electronics-forum/battery-electric-vehicle-power-range",
        //        "https://www.eeweb.com/electronics-forum/regarding-smps",
        //        "https://www.eeweb.com/electronics-forum/i-am-doing-project-for-my-home-and-need-help",
        //        "https://www.eeweb.com/electronics-forum/need-help-to-design-avr",
        //        "https://www.eeweb.com/electronics-forum/electric-vehicle-on-board-chargers",
        //        "https://www.eeweb.com/electronics-forum/looking-for-projects",
        //        "https://www.eeweb.com/electronics-forum/atr-72-600-anf-boeing-737",
        //        "https://www.eeweb.com/electronics-forum/help-transistor-transformer-something-i-know-nothing",
        //        "https://www.eeweb.com/electronics-forum/delay-timer-relay-control-connection-problem",
        //        "https://www.eeweb.com/electronics-forum/need-help-with-wire-gauge-for-streetlight-design",
        //        "https://www.eeweb.com/electronics-forum/measuring-the-current-draw-of-multiple-servos",
        //        "https://www.eeweb.com/electronics-forum/is-lm35-is-sufficient-as-battery-temperature-sensor",
        //        "https://www.eeweb.com/electronics-forum/dc-dc-power-supply-design-in-automotive",
        //        "https://www.eeweb.com/electronics-forum/on-board-charger-for-electric-vehicles",
        //        "https://www.eeweb.com/electronics-forum/inertia-of-power-system",
        //        "https://www.eeweb.com/electronics-forum/cat-6a-tcl-failures",
        //        "https://www.eeweb.com/electronics-forum/rexroth-rd52-1-34n-110-l-v1-fw-inverter-drive",
        //        "https://www.eeweb.com/electronics-forum/data-signals-of-ssi-encoder",
        //        "https://www.eeweb.com/toolbox/resistor-tables"
        //    };

        //    foreach (var url in newsUrls)
        //    {
        //        await ProcessUrl(url);
        //    }
        //}

        static async Task ProcessUrl(string url)
        {
            List<string> externalLinks = new List<string>();
            Search.Scrapper.ScrapeWeb bot = new Scrapper.ScrapeWeb();
            var content = await bot.ReturnArticleObject(url);
            System.Console.WriteLine(url);
            System.Console.WriteLine("--------------------------------------------------------------------------------------------------");

            externalLinks = bot.GetAnchorTags(content.RawBodyContents, url);
            if (content.RawBodyContents == null)
            {
                System.Console.WriteLine("No HTML recovered");
            }

            foreach (var externalLink in externalLinks)
            {
                System.Console.WriteLine(externalLink);
            }
            System.Console.WriteLine();
        }

        //static HttpClient client = new HttpClient();
        //static void Main(string[] args)
        //{
        //    //RunAsync().Wait();

        //    Task.Run(() => RunAsync()).Wait();
        //}

        //public static async void RunAsync()
        //{
        //    client.BaseAddress = new Uri("https://search.aspencore.com/api/");
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        //    PartsQuery q = new PartsQuery() { PartFamily = "lm317", AdditionalKeywords = null, MaxCount = 5 };
        //    string d = await GetResults(q);
        //}

        //private static async Task<string> GetResults(PartsQuery q)
        //{
        //    string data;
        //    StringContent s = new StringContent(JsonConvert.SerializeObject(q), Encoding.Unicode, "application/json");
        //    HttpResponseMessage message = await client.PostAsync("api/v1/searchresults", s);
        //    message.EnsureSuccessStatusCode();
        //    data = await message.Content.ReadAsStringAsync();
        //    return data;
        //}
    }
}
