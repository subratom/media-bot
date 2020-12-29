using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search.Helpers
{
    public class UrlBuilder
    {
        public string Scheme { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }

        public override string ToString()
        {
            if (Int32.Parse(Port) != 443 && Int32.Parse(Port) != 80)
                return Scheme.ToString() + "://" + Host.ToString() + Port.ToString() + Path.ToString() + QueryString.ToString();
            if (QueryString == null && string.IsNullOrEmpty(QueryString))
                return Scheme.ToString() + "://" + Host.ToString() + Path.ToString();
            return Scheme.ToString() + "://" + Host.ToString() + Path.ToString() + QueryString.ToString();
        }
    }
}
