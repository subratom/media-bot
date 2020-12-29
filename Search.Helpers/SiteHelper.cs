using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search.Helpers
{
    public static class SiteHelper
    {
        //Change this method to pick up site urls from a config file
        public static string GetSiteName(this String Url)
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
    }
}
