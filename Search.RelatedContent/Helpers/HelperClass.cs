using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Search.RelatedContent.Helpers
{
    public static class HelperClass
    {
        public static string GetImageByUrl(this String Url)
        {
            string ImageUrl = string.Empty;

            if (Url.Contains("eetimes.com"))
            {
                ImageUrl = "http://www.electronicproducts.com/Images2/NewsletterImages/EETimesLogos/EETimes_WeekinReviewLogo.jpg";
            }
            else if (Url.Contains("www.electronicproducts.com"))
            {
                ImageUrl = "http://rnd.aspencore.com/ep/images/ep.png";
            }
            else if (Url.Contains("www.edn.com"))
            {
                ImageUrl = "http://www.electronicproducts.com/Images2/NewsletterImages/EETimesLogos/EDN_header_nl_logo.jpg";
            }
            else if (Url.Contains("www.embedded.com"))
            {
                ImageUrl = "http://www.electronicproducts.com/Images2/NewsletterImages/EETimesLogos/embedded.png";
            }
            else if (Url.Contains("www.planetanalog.com"))
            {
                ImageUrl = "http://m.eet.com/images/planetanalog/Planet-Analog_CAP_top_600x122.gif";
            }
            else if (Url.Contains("www.ebnonline.com"))
            {
                ImageUrl = "http://img.lightreading.com/ebnonline/EBN_top_logo.gif";
            }
            else if (Url.Contains("www.techonline.com"))
            {
                ImageUrl = "http://www.techonline.com/img/structure/logo.png";
            }
            return ImageUrl;
        }
    }
}