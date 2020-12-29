using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;

namespace Search.PartExtractor
{
    public class SiliconExpertAPI
    {
        private static DateTime LastCallToSiliconExpert { get; set; } = DateTime.Now;
        public int MinimumWaitTimeBetweenCalls { get; } = 250; // milli seconds
        private object thisLock = new object();
        public bool LoginStatus { get; private set; } = false;
        private HttpClient client = new HttpClient();
        private readonly string loginUrl = "http://52.8.69.170:8001/SearchService/login/authenticateUser?login=ac_offline&apiKey=Ab8MK21O0q";
        private readonly string partUrlPrefix = "http://52.8.69.170:8001/SearchService/free/search/partsearch?partNumber=";

        public SiliconExpertAPI()
        {
            // required by SiliconExpert
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Login().Wait();
        }

        public async Task Login()
        {
            // need to call before every SE API call
            this.Timer();

            HttpResponseMessage responseMessage = await client.GetAsync(this.loginUrl);

            // this needs to be updated after every call to the SE API
            LastCallToSiliconExpert = DateTime.Now;

            if (responseMessage.IsSuccessStatusCode)
            {
                var result = await responseMessage.Content.ReadAsStringAsync();

                dynamic results = JsonConvert.DeserializeObject<dynamic>(result);

                var success = results.Status.Success;
                if (success == "true")
                {
                    LoginStatus = true;
                }
            }
        }

        public async Task<bool> ValidatePart(string partNumber)
        {
            bool bResult = false;
            string partNumberUrl = $"{partUrlPrefix}{partNumber}";

            // need to call before every SE API call
            this.Timer();

            HttpResponseMessage responseMessage = await client.GetAsync(partNumberUrl);

            // this needs to be updated after every call to the SE API
            LastCallToSiliconExpert = DateTime.Now;

            if (responseMessage.IsSuccessStatusCode)
            {
                var result = await responseMessage.Content.ReadAsStringAsync();

                dynamic results = JsonConvert.DeserializeObject<dynamic>(result);

                var success = results.Status.Success;
                var code = results.Status.Code;
                if (success == "true" && code != "3")
                {
                    // if the part number is only numeric we must have an exact match from SE
                    // if it's not numeric then we will do a begins with match case insensitive

                    // check first part number in result
                    try
                    {
                        string firstPart = results.Result[0].PartNumber;

                        // does the part number only contain digits
                        string onlyNumbers = @"^([\d]{4,})";
                        Regex numberMatch = new Regex(onlyNumbers);
                        if (numberMatch.IsMatch(firstPart))
                        {
                            if (firstPart == partNumber)
                            {
                                // we found the part
                                bResult = true;
                            }
                        }
                        // we need to be case insensitive
                        else if (firstPart.ToUpper().StartsWith(partNumber.ToUpper()) == true)
                        {
                            // we found the part
                            bResult = true;
                        }
                    }
                    catch
                    {
                        //int breaker = 1;
                    }
                }
            }

            return bResult;
        }

        private void Timer()
        {
            lock (thisLock)
            {
                DateTime now = DateTime.Now;
                TimeSpan timeSpanSinceLastCall = now - LastCallToSiliconExpert;
                // round down so we don't make more calls becasue of rounding errors
                int milliSecondsSinceLastCall = (int)Math.Floor(timeSpanSinceLastCall.TotalMilliseconds);

                if (milliSecondsSinceLastCall < MinimumWaitTimeBetweenCalls)
                {
                    int pauseInterval = MinimumWaitTimeBetweenCalls - milliSecondsSinceLastCall;
                    // Console.WriteLine($"Pausing {pauseInterval}");
                    System.Threading.Thread.Sleep(pauseInterval);
                }

                // LastCallToSiliconExpert = DateTime.Now;
            }
        }

        public void TestTimer()
        {
            lock (thisLock)
            {
                DateTime now = DateTime.Now;
                TimeSpan timeSpanSinceLastCall = now - LastCallToSiliconExpert;
                // round down so we don't make more calls becasue of rounding errors
                int milliSecondsSinceLastCall = (int)Math.Floor(timeSpanSinceLastCall.TotalMilliseconds);

                if (milliSecondsSinceLastCall < MinimumWaitTimeBetweenCalls)
                {
                    int pauseInterval = MinimumWaitTimeBetweenCalls - milliSecondsSinceLastCall;
                    //Console.WriteLine($"--> pausing {pauseInterval} milliseconds");
                    System.Threading.Thread.Sleep(pauseInterval);
                }
                else
                {
                    //Console.WriteLine($"--> no need to wait");
                }
                LastCallToSiliconExpert = DateTime.Now;
            }
        }
    }
}
