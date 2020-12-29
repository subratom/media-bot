using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Search.API.Classes;
using System.Configuration;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;
using HtmlAgilityPack;

namespace Search.Helpers
{
    public static class HtmlHelper
    {
        public static string RemoveHtmlTags(String input)
        {
            return Regex.Replace(RemoveScriptTags(input), @"<[^>]+>|&nbsp;", " ").Trim();
        }

        private static string RemoveScriptTags(string input)
        {
            Regex rRemScript = new Regex(@"<script[^>]*>[\s\S]*?</script>");
            return rRemScript.Replace(input, "");
        }

        public static string RemoveMultipleSpaces(String input)
        {
            return Regex.Replace(input, @"\s{2,}", " ").Trim();
        }

        public static Boolean HasClass(this HtmlNode element, String className)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (string.IsNullOrWhiteSpace(className)) throw new ArgumentNullException(nameof(className));
            if (element.NodeType != HtmlNodeType.Element) return false;

            HtmlAttribute classAttrib = element.Attributes["class"];
            if (classAttrib == null) return false;

            Boolean hasClass = CheapClassListContains(classAttrib.Value, className, StringComparison.Ordinal);
            return hasClass;
        }

        public static Boolean HasId(this HtmlNode element, String idName)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (string.IsNullOrWhiteSpace(idName)) throw new ArgumentNullException(nameof(idName));
            if (element.NodeType != HtmlNodeType.Element) return false;

            HtmlAttribute classAttrib = element.Attributes["id"];
            if (classAttrib == null) return false;

            Boolean hasId = CheapClassListContains(classAttrib.Value, idName, StringComparison.Ordinal);
            return hasId;
        }

        /// <summary>Performs optionally-whitespace-padded string search without new string allocations.</summary>
        /// <remarks>A regex might also work, but constructing a new regex every time this method is called would be expensive.</remarks>
        private static Boolean CheapClassListContains(String haystack, String needle, StringComparison comparison)
        {
            if (String.Equals(haystack, needle, comparison)) return true;
            Int32 idx = 0;
            while (idx + needle.Length <= haystack.Length)
            {
                idx = haystack.IndexOf(needle, idx, comparison);
                if (idx == -1) return false;

                Int32 end = idx + needle.Length;

                // Needle must be enclosed in whitespace or be at the start/end of string
                Boolean validStart = idx == 0 || Char.IsWhiteSpace(haystack[idx - 1]);
                Boolean validEnd = end == haystack.Length || Char.IsWhiteSpace(haystack[end]);
                if (validStart && validEnd) return true;

                idx++;
            }
            return false;
        }

        public static string GetHtmlNode()
        {


            return string.Empty;
        }
        public static string GetAuthor(string site, string Html)
        {
            HtmlDocument document = new HtmlDocument();

            if (string.IsNullOrEmpty(Html) || string.IsNullOrEmpty(site))
                return string.Empty;

            document.LoadHtml(Html);

            HtmlNodeCollection htmlNodes = document.DocumentNode.SelectNodes("//meta");
            if (htmlNodes != null)
            {
                foreach (var metaNode in htmlNodes)
                {
                    if (metaNode.GetAttributeValue("name", "") == "author")
                    {
                        return HttpUtility.HtmlDecode(metaNode.GetAttributeValue("content", ""));
                    }
                }
            }
            return string.Empty;
        }

        public static string GetPublishDate(string site, string Html)
        {
            HtmlDocument document = new HtmlDocument();

            if (string.IsNullOrEmpty(Html) || string.IsNullOrEmpty(site))
                return string.Empty;

            document.LoadHtml(Html);

            HtmlNodeCollection htmlNodes = document.DocumentNode.SelectNodes("//meta");
            if (htmlNodes != null)
            {
                foreach (var metaNode in htmlNodes)
                {
                    if (metaNode.GetAttributeValue("name", "") == "created")
                    {
                        return metaNode.GetAttributeValue("content", "");
                    }
                }
            }

            //eeweb
            HtmlNodeCollection collection = document.DocumentNode.SelectNodes("//div[@class='posted_date']");
            if (collection != null)
            {
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@class='posted_date']"))
                {
                    if (node.ChildNodes.Count == 1)
                    {
                        return node.InnerText;
                    }
                }
            }
            return string.Empty;
        }
        public static string GetImageUrl(string site, string Html)
        {
            HtmlDocument document = new HtmlDocument();

            if (string.IsNullOrEmpty(Html) || string.IsNullOrEmpty(site))
                return string.Empty;

            document.LoadHtml(Html);

            HtmlNodeCollection htmlNodes = document.DocumentNode.SelectNodes("//meta");
            if (htmlNodes != null)
            {
                foreach (var metaNode in htmlNodes)
                {
                    if (metaNode.GetAttributeValue("property", "") == "og:image")
                    {
                        return metaNode.GetAttributeValue("content", "");
                    }
                }
            }
            return string.Empty;
        }

        public static string GetSite(this string Url)
        {
            string siteName = string.Empty;

            if (Url.Contains("eetimes.com"))
            {
                siteName = "EETimes";
            }
            else if (Url.Contains("electronicproducts.com"))
            {
                siteName = "Electronic Products";
            }
            else if (Url.Contains("edn.com"))
            {
                siteName = "EDN";
            }
            else if (Url.Contains("embedded.com"))
            {
                siteName = "Embedded";
            }
            else if (Url.Contains("planetanalog.com"))
            {
                siteName = "Planet Analog";
            }
            else if (Url.Contains("ebnonline.com"))
            {
                siteName = "EBN Online";
            }
            else if (Url.Contains("techonline.com"))
            {
                siteName = "TechOnline";
            }
            else if (Url.Contains("eeweb.com"))
            {
                siteName = "EEWeb";
            }
            return siteName;
        }

        public static string GetAuthorImage(string authorName)
        {
            //this will read the config file and
            string authorImageFile = @"";

            GenericHelpers getList = new GenericHelpers();
            try
            {
                string authorXml = ReadAuthorXml();

                var authors = DeserializeData(authorXml);

                //MemoryStream doc = GetConfigDocument();

                //List<Author> authors = getList.DeserializeObject<List<Author>>(doc, "authors");

                var authorLookup = (from a in authors.AuthorList
                                    where a.Name == authorName
                                    select a).ToList();

                if (authorLookup.Count > 0)
                {
                    authorImageFile = authorLookup[0].Url;
                    if (string.IsNullOrEmpty(authorImageFile))
                        authorImageFile = @"https://rnd.aspencore.com/hulk-media/generic-author-images/generic-user-male.gif";
                }
                else
                {
                    authorImageFile = @"";
                }

                return authorImageFile; //Decide where we want to host this image, also how do we decide if the author is male or female.
            }
            catch (Exception e)
            {
                return @"";
                //throw e;
            }
        }

        private static Authors DeserializeData(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Authors), new XmlRootAttribute() { ElementName = "authors" });
            using (StringReader reader = new StringReader(xml))
            {
                var deserialized = (Authors)serializer.Deserialize(reader);
                return deserialized;
            }
        }

        private static MemoryStream GetConfigDocument()
        {

            MemoryStream ms = new MemoryStream();
            using (FileStream file = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"\Config\" + ConfigurationManager.AppSettings["authorconfig"].ToString(), FileMode.Open, FileAccess.Read))
            {
                file.CopyTo(ms);

                return ms;
            }
        }


        private static string ReadAuthorXml()
        {

            return File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Config\" + ConfigurationManager.AppSettings["authorconfig"].ToString());
        }

        public static string GetImageForDisplay(this string Url, string currentImageUrl)
        {
            string ImageUrl = string.Empty;

            if (Url.Contains("eetimes.com"))
            {
                if (currentImageUrl.Equals("https://m.eet.com/images/eetimes/eet_fb_icon3.jpg"))
                    ImageUrl = "https://rnd.aspencore.com/elasticsearch/EET-650x366.jpg";
                else
                    ImageUrl = currentImageUrl;
            }
            else if (Url.Contains("electronicproducts.com"))
            {
                if (currentImageUrl.Equals("https://www.electronicproducts.com/uploadedImages/thumb_noimage.jpg"))
                    ImageUrl = "https://rnd.aspencore.com/elasticsearch/EP-650x366.jpg";
                else if (currentImageUrl.Equals("https://www.electronicproducts.com"))
                    ImageUrl = "https://rnd.aspencore.com/elasticsearch/EP-650x366.jpg";
                else
                    ImageUrl = currentImageUrl;
            }
            else if (Url.Contains("edn.com"))
            {
                if (currentImageUrl.Equals("https://m.eet.com/images/edn/images/icons/contentitem-default.png"))
                    ImageUrl = "https://rnd.aspencore.com/elasticsearch/EDN-650x366.jpg";
                else
                    ImageUrl = currentImageUrl;
            }
            else if (Url.Contains("embedded.com"))
            {
                if (currentImageUrl.Equals("https://m.eet.com/images/edn/images/icons/contentitem-default.png"))
                    ImageUrl = "https://rnd.aspencore.com/elasticsearch/embedded-650x366.jpg";
                else
                    ImageUrl = currentImageUrl;
            }
            else if (Url.Contains("planetanalog.com"))
            {
                if (currentImageUrl.Equals("https://m.eet.com/images/planetanalog/Planet-Analog_CAP_top_600x122.gif"))
                    ImageUrl = "https://rnd.aspencore.com/elasticsearch/planetanalog-650x366.jpg";
                else
                    ImageUrl = currentImageUrl;
            }
            else if (Url.Contains("ebnonline.com"))
            {
                if (currentImageUrl.Equals("http://img.lightreading.com/ebnonline/EBN_top_logo.gif"))
                    ImageUrl = "https://rnd.aspencore.com/elasticsearch/EBN-650x366.jpg";
                else
                    ImageUrl = currentImageUrl;
            }
            else if (Url.Contains("techonline.com"))
            {
                ImageUrl = "https://rnd.aspencore.com/elasticsearch/TOL-650x366.jpg";
            }
            return ImageUrl;
        }

        public static string GetDefaultImageUrls(this string Url)
        {
            string ImageUrl = string.Empty;

            if (Url.Contains("eetimes.com"))
            {
                ImageUrl = "https://rnd.aspencore.com/elasticsearch/EET-650x366.jpg";
            }
            else if (Url.Contains("electronicproducts.com"))
            {
                ImageUrl = "https://rnd.aspencore.com/elasticsearch/EP-650x366.jpg";
            }
            else if (Url.Contains("edn.com"))
            {
                ImageUrl = "https://rnd.aspencore.com/elasticsearch/EDN-650x366.jpg";
            }
            else if (Url.Contains("embedded.com"))
            {
                ImageUrl = "https://rnd.aspencore.com/elasticsearch/embedded-650x366.jpg";
            }
            else if (Url.Contains("planetanalog.com"))
            {
                ImageUrl = "https://rnd.aspencore.com/elasticsearch/planetanalog-650x366.jpg";
            }
            else if (Url.Contains("ebnonline.com"))
            {
                ImageUrl = "https://rnd.aspencore.com/elasticsearch/EBN-650x366.jpg";
            }
            else if (Url.Contains("techonline.com"))
            {
                ImageUrl = "https://rnd.aspencore.com/elasticsearch/TOL-650x366.jpg";
            }
            return ImageUrl;
        }

        private static bool IsUrlEncoded(string text)
        {
            return (HttpUtility.UrlEncode(text) != text);
        }

        private static string UrlEncode(string Url)
        {
            return HttpUtility.UrlEncode(Url);
        }

        private static string UrlDecode(string Url)
        {
            return HttpUtility.UrlDecode(Url);
        }

        public static string GetUrl(string Url)
        {
            if (IsUrlEncoded(Url))
            {
                return UrlDecode(Url);
            }
            else
            {
                return Url;
            }
        }

        public static string CleanUrl(this String Url)
        {
            //List the rules to clean the url. 
            string currentSite = Url.GetSite();
            string cleanedUrl = string.Empty;
            switch (currentSite)
            {
                case "Electronic Products":
                    cleanedUrl = RemoveParameters(Url);
                    break;
                case "EETimes":
                    cleanedUrl = CleanEETUrl(Url);
                    break;
                case "EDN":
                    cleanedUrl = RemoveParameters(Url);
                    break;
                case "Embedded":
                    cleanedUrl = RemoveParameters(Url);
                    break;
                case "Planet Analog":
                    cleanedUrl = CleanPAUrl(Url);
                    break;
                default:
                    break;
            }
            return cleanedUrl;
        }

        private static string CleanPAUrl(string url)
        {
            string[] exclusion = new string[] { "email.asp", "print", "url", "title" };

            Dictionary<string, string> keyValuePairs = GetParams(url);
            string cleanedUrl = RemoveParameters(url);

            for (int i = 0; i < exclusion.Length; i++)
            {
                if (keyValuePairs.ContainsKey(exclusion[i]))
                    return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("?");
            foreach (var item in keyValuePairs)
            {
                if (item.Key != "page_number" && item.Key != "utm_source" && item.Key != "utm_medium" && item.Key != "piddl_msgid")
                {
                    sb.AppendFormat("{0}={1}", item.Key, item.Value);
                    sb.Append("&");
                }
            }
            return cleanedUrl + sb.ToString().Substring(0, sb.ToString().Length - 1);
        }

        private static string CleanEETUrl(string url)
        {

            string[] exclusion = new string[] { "email.asp", "print", "url", "title" };

            Dictionary<string, string> keyValuePairs = GetParams(url);
            string cleanedUrl = RemoveParameters(url);

            for (int i = 0; i < exclusion.Length; i++)
            {
                if (keyValuePairs.ContainsKey(exclusion[i]))
                    return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("?");
            foreach (var item in keyValuePairs)
            {
                if (item.Key != "page_number" && item.Key != "utm_source" && item.Key != "utm_medium" && item.Key != "piddl_msgid")
                {
                    sb.AppendFormat("{0}={1}", item.Key, item.Value);
                    sb.Append("&");
                }
            }
            return cleanedUrl + sb.ToString().Substring(0, sb.ToString().Length - 1);
        }

        private static Dictionary<string, string> GetParams(string uri)
        {
            var matches = Regex.Matches(uri, @"[\?&](([^&=]+)=([^&=#]*))", RegexOptions.Compiled);
            return matches.Cast<Match>().ToDictionary(
                m => Uri.UnescapeDataString(m.Groups[2].Value),
                m => Uri.UnescapeDataString(m.Groups[3].Value)
            );
        }

        private static string[] BreakURLStructure(this String URL)
        {
            string[] parts = new string[6];

            string regexPattern = @"^(?<s1>(?<s0>[^:/\?#]+):)?(?<a1>"
                                  + @"//(?<a0>[^/\?#]*))?(?<p0>[^\?#]*)"
                                  + @"(?<q1>\?(?<q0>[^#]*))?"
                                  + @"(?<f1>#(?<f0>.*))?";

            Regex linkParser = new Regex(regexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

            Match m = linkParser.Match(URL);

            if (m.Groups.Count > 0)
            {
                if (!string.IsNullOrEmpty(m.Groups["s1"].Value))
                {
                    parts[0] = m.Groups["s1"].Value;
                }
                else
                    return null;

                if (!string.IsNullOrEmpty(m.Groups["a1"].Value))
                {
                    parts[1] = m.Groups["a1"].Value;
                }
                if (!string.IsNullOrEmpty(m.Groups["p0"].Value))
                {
                    if (m.Groups["p0"].Value.Contains("&") && !m.Groups["p0"].Value.Contains("?"))
                    {
                        int indexAmpersand = m.Groups["p0"].Value.IndexOf("&");
                        if (indexAmpersand > 0)
                        {
                            parts[2] = m.Groups["p0"].Value.Substring(0, indexAmpersand);
                            parts[3] = "?";
                            parts[4] = m.Groups["p0"].Value.Substring(indexAmpersand);
                        }
                    }
                    else
                        parts[2] = m.Groups["p0"].Value;
                }
                if (!string.IsNullOrEmpty(m.Groups["q1"].Value))
                {
                    parts[3] = "?";
                }
                if (m.Groups["q1"].Value.IndexOf("http://") > 0 || m.Groups["q1"].Value.IndexOf("https://") > 0)
                {
                    if (!string.IsNullOrEmpty(m.Groups["q1"].Value))
                    {
                        parts[4] = m.Groups["q1"].Value.Substring(1);
                    }

                    if (!string.IsNullOrEmpty(m.Groups["f1"].Value))
                    {
                        parts[5] += m.Groups["f1"].Value;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(m.Groups["q1"].Value) && m.Groups["q1"].Value.ToString().Length > 0)
                        parts[4] = m.Groups["q1"].Value.Substring(1);

                    if (!string.IsNullOrEmpty(m.Groups["f1"].Value))
                    {
                        parts[5] += m.Groups["f1"].Value;
                    }
                }

                return parts;
            }
            return null;
        }

        //private static string MatchAndReturn(this String URL, string TrackerId)
        //{
        //    string regEx = "(&|&amp;|\\?)(acctid)(=)(\\d+)";

        //    //string re1 = "/^(&amp;|\\?)"; // Any Single Character 1
        //    //string re2 = "(acctid)"; // "((?:[a-z][a-z]+))";   // Word 1
        //    //string re3 = "(=)"; // Any Single Character 2
        //    //string re4 = "(\\d+)/";  // Integer Number 1
        //    //re1 + re2 + re3 + re4

        //    Regex r = new Regex(regEx, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        //    Match m = r.Match(URL);
        //    if (m.Success)
        //    {
        //        String c1 = m.Groups[1].ToString();
        //        String word1 = m.Groups[2].ToString();
        //        String c2 = m.Groups[3].ToString();
        //        String int1 = m.Groups[4].ToString();

        //        //return r.Replace(instanceData, string.Empty);

        //        //return "(" + c1.ToString() + ")" + "(" + word1.ToString() + ")" + "(" + c2.ToString() + ")" + "(" + int1.ToString() + ")";
        //        return int1.ToString();
        //    }
        //    return string.Empty;
        //}

        private static string RemoveParameters(string url)
        {
            //Handle only EP for now
            Uri uri = new Uri(url);

            UrlBuilder buildUrl = new UrlBuilder
            {
                Scheme = uri.Scheme,
                Host = uri.Host,
                Port = uri.Port.ToString(),
                Path = uri.AbsolutePath
            };

            return buildUrl.ToString();
        }
    }


    public static class WordExtenions
    {

        public static string ReturnReplacedString(String input, string replaceWord, string replacemenetWord)
        {
            return input.ToLower().Replace(replaceWord.ToLower(), replacemenetWord);
        }
    }

    //static class StopwordTool
    //{
    //    /// <summary>
    //    /// Words we want to remove.
    //    /// </summary>
    //    static Dictionary<string, bool> _stops = new Dictionary<string, bool>
    //{
    //    { "a", true },
    //    { "about", true },
    //    { "above", true },
    //    { "across", true },
    //    { "after", true },
    //    { "afterwards", true },
    //    { "again", true },
    //    { "against", true },
    //    { "all", true },
    //    { "almost", true },
    //    { "alone", true },
    //    { "along", true },
    //    { "already", true },
    //    { "also", true },
    //    { "although", true },
    //    { "always", true },
    //    { "am", true },
    //    { "among", true },
    //    { "amongst", true },
    //    { "amount", true },
    //    { "an", true },
    //    { "and", true },
    //    { "another", true },
    //    { "any", true },
    //    { "anyhow", true },
    //    { "anyone", true },
    //    { "anything", true },
    //    { "anyway", true },
    //    { "anywhere", true },
    //    { "are", true },
    //    { "around", true },
    //    { "as", true },
    //    { "at", true },
    //    { "back", true },
    //    { "be", true },
    //    { "became", true },
    //    { "because", true },
    //    { "become", true },
    //    { "becomes", true },
    //    { "becoming", true },
    //    { "been", true },
    //    { "before", true },
    //    { "beforehand", true },
    //    { "behind", true },
    //    { "being", true },
    //    { "below", true },
    //    { "beside", true },
    //    { "besides", true },
    //    { "between", true },
    //    { "beyond", true },
    //    { "bill", true },
    //    { "both", true },
    //    { "bottom", true },
    //    { "but", true },
    //    { "by", true },
    //    { "call", true },
    //    { "can", true },
    //    { "cannot", true },
    //    { "cant", true },
    //    { "co", true },
    //    { "computer", true },
    //    { "con", true },
    //    { "could", true },
    //    { "couldnt", true },
    //    { "cry", true },
    //    { "de", true },
    //    { "describe", true },
    //    { "detail", true },
    //    { "do", true },
    //    { "done", true },
    //    { "down", true },
    //    { "due", true },
    //    { "during", true },
    //    { "each", true },
    //    { "eg", true },
    //    { "eight", true },
    //    { "either", true },
    //    { "eleven", true },
    //    { "else", true },
    //    { "elsewhere", true },
    //    { "empty", true },
    //    { "enough", true },
    //    { "etc", true },
    //    { "even", true },
    //    { "ever", true },
    //    { "every", true },
    //    { "everyone", true },
    //    { "everything", true },
    //    { "everywhere", true },
    //    { "except", true },
    //    { "few", true },
    //    { "fifteen", true },
    //    { "fify", true },
    //    { "fill", true },
    //    { "find", true },
    //    { "fire", true },
    //    { "first", true },
    //    { "five", true },
    //    { "for", true },
    //    { "former", true },
    //    { "formerly", true },
    //    { "forty", true },
    //    { "found", true },
    //    { "four", true },
    //    { "from", true },
    //    { "front", true },
    //    { "full", true },
    //    { "further", true },
    //    { "get", true },
    //    { "give", true },
    //    { "go", true },
    //    { "had", true },
    //    { "has", true },
    //    { "have", true },
    //    { "he", true },
    //    { "hence", true },
    //    { "her", true },
    //    { "here", true },
    //    { "hereafter", true },
    //    { "hereby", true },
    //    { "herein", true },
    //    { "hereupon", true },
    //    { "hers", true },
    //    { "herself", true },
    //    { "him", true },
    //    { "himself", true },
    //    { "his", true },
    //    { "how", true },
    //    { "however", true },
    //    { "hundred", true },
    //    { "i", true },
    //    { "ie", true },
    //    { "if", true },
    //    { "in", true },
    //    { "inc", true },
    //    { "indeed", true },
    //    { "interest", true },
    //    { "into", true },
    //    { "is", true },
    //    { "it", true },
    //    { "its", true },
    //    { "itself", true },
    //    { "keep", true },
    //    { "last", true },
    //    { "latter", true },
    //    { "latterly", true },
    //    { "least", true },
    //    { "less", true },
    //    { "ltd", true },
    //    { "made", true },
    //    { "many", true },
    //    { "may", true },
    //    { "me", true },
    //    { "meanwhile", true },
    //    { "might", true },
    //    { "mill", true },
    //    { "mine", true },
    //    { "more", true },
    //    { "moreover", true },
    //    { "most", true },
    //    { "mostly", true },
    //    { "move", true },
    //    { "much", true },
    //    { "must", true },
    //    { "my", true },
    //    { "myself", true },
    //    { "name", true },
    //    { "namely", true },
    //    { "neither", true },
    //    { "never", true },
    //    { "nevertheless", true },
    //    { "next", true },
    //    { "nine", true },
    //    { "no", true },
    //    { "nobody", true },
    //    { "none", true },
    //    { "nor", true },
    //    { "not", true },
    //    { "nothing", true },
    //    { "now", true },
    //    { "nowhere", true },
    //    { "of", true },
    //    { "off", true },
    //    { "often", true },
    //    { "on", true },
    //    { "once", true },
    //    { "one", true },
    //    { "only", true },
    //    { "onto", true },
    //    { "or", true },
    //    { "other", true },
    //    { "others", true },
    //    { "otherwise", true },
    //    { "our", true },
    //    { "ours", true },
    //    { "ourselves", true },
    //    { "out", true },
    //    { "over", true },
    //    { "own", true },
    //    { "part", true },
    //    { "per", true },
    //    { "perhaps", true },
    //    { "please", true },
    //    { "put", true },
    //    { "rather", true },
    //    { "re", true },
    //    { "same", true },
    //    { "see", true },
    //    { "seem", true },
    //    { "seemed", true },
    //    { "seeming", true },
    //    { "seems", true },
    //    { "serious", true },
    //    { "several", true },
    //    { "she", true },
    //    { "should", true },
    //    { "show", true },
    //    { "side", true },
    //    { "since", true },
    //    { "sincere", true },
    //    { "six", true },
    //    { "sixty", true },
    //    { "so", true },
    //    { "some", true },
    //    { "somehow", true },
    //    { "someone", true },
    //    { "something", true },
    //    { "sometime", true },
    //    { "sometimes", true },
    //    { "somewhere", true },
    //    { "still", true },
    //    { "such", true },
    //    { "system", true },
    //    { "take", true },
    //    { "ten", true },
    //    { "than", true },
    //    { "that", true },
    //    { "the", true },
    //    { "their", true },
    //    { "them", true },
    //    { "themselves", true },
    //    { "then", true },
    //    { "thence", true },
    //    { "there", true },
    //    { "thereafter", true },
    //    { "thereby", true },
    //    { "therefore", true },
    //    { "therein", true },
    //    { "thereupon", true },
    //    { "these", true },
    //    { "they", true },
    //    { "thick", true },
    //    { "thin", true },
    //    { "third", true },
    //    { "this", true },
    //    { "those", true },
    //    { "though", true },
    //    { "three", true },
    //    { "through", true },
    //    { "throughout", true },
    //    { "thru", true },
    //    { "thus", true },
    //    { "to", true },
    //    { "together", true },
    //    { "too", true },
    //    { "top", true },
    //    { "toward", true },
    //    { "towards", true },
    //    { "twelve", true },
    //    { "twenty", true },
    //    { "two", true },
    //    { "un", true },
    //    { "under", true },
    //    { "until", true },
    //    { "up", true },
    //    { "upon", true },
    //    { "us", true },
    //    { "very", true },
    //    { "via", true },
    //    { "was", true },
    //    { "we", true },
    //    { "well", true },
    //    { "were", true },
    //    { "what", true },
    //    { "whatever", true },
    //    { "when", true },
    //    { "whence", true },
    //    { "whenever", true },
    //    { "where", true },
    //    { "whereafter", true },
    //    { "whereas", true },
    //    { "whereby", true },
    //    { "wherein", true },
    //    { "whereupon", true },
    //    { "wherever", true },
    //    { "whether", true },
    //    { "which", true },
    //    { "while", true },
    //    { "whither", true },
    //    { "who", true },
    //    { "whoever", true },
    //    { "whole", true },
    //    { "whom", true },
    //    { "whose", true },
    //    { "why", true },
    //    { "will", true },
    //    { "with", true },
    //    { "within", true },
    //    { "without", true },
    //    { "would", true },
    //    { "yet", true },
    //    { "you", true },
    //    { "your", true },
    //    { "yours", true },
    //    { "yourself", true },
    //    { "yourselves", true }
    //};

    //    /// <summary>
    //    /// Chars that separate words.
    //    /// </summary>
    //    static char[] _delimiters = new char[]
    //    {
    //    ' ',
    //    ',',
    //    ';',
    //    '.'
    //    };

    //    /// <summary>
    //    /// Remove stopwords from string.
    //    /// </summary>
    //    public static string RemoveStopwords(string input)
    //    {
    //        // 1
    //        // Split parameter into words
    //        var words = input.Split(_delimiters,
    //            StringSplitOptions.RemoveEmptyEntries);
    //        // 2
    //        // Allocate new dictionary to store found words
    //        var found = new Dictionary<string, bool>();
    //        // 3
    //        // Store results in this StringBuilder
    //        StringBuilder builder = new StringBuilder();
    //        // 4
    //        // Loop through all words
    //        foreach (string currentWord in words)
    //        {
    //            // 5
    //            // Convert to lowercase
    //            string lowerWord = currentWord.ToLower();
    //            // 6
    //            // If this is a usable word, add it
    //            if (!_stops.ContainsKey(lowerWord) &&
    //                !found.ContainsKey(lowerWord))
    //            {
    //                builder.Append(currentWord).Append(' ');
    //                found.Add(lowerWord, true);
    //            }
    //        }
    //        // 7
    //        // Return string with words removed
    //        return builder.ToString().Trim();
    //    }
    //}
}
