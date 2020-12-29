using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Search.MediaStore.DDL.Helpers
{
    public static class XmlHelpers
    {
        public static string GetXmlNodeValue(XmlNode currentNode, string nodeName)
        {
            foreach (XmlNode childNodes in currentNode.ChildNodes)
            {
                if (childNodes.Name == nodeName)
                    return childNodes.InnerText.Replace(@"""", @"\""").Replace(@"CDATA", " ");
            }
            return string.Empty;
        }

        
    }
}
