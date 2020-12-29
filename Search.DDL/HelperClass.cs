using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Search.MediaStore.DDL
{
    public static class HelperClass
    {
        public static string SanitizeUrl(this String Url)
        {
            string[] UrlItems = Url.BreakURLStructure();

            if (UrlItems != null && UrlItems.Count() > 0)
            {
                List<string> urlList = new List<string>
                {
                    UrlItems[0] + UrlItems[1] + UrlItems[2] + UrlItems[3] + UrlItems[4] + UrlItems[5]
                };

                return string.Empty;
            }
            else
                return string.Empty;
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

        //private static string[] GetUrlParts(string Url)
        //{
        //    string[] 
        //    Uri testUrl = new Uri(Url);

        //    testUrl.Authority
        //}
    }
}
