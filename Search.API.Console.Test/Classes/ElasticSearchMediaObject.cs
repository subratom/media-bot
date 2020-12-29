using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search.API.Console.Test.Classes
{
    public class Source
    {
        public string site { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string imageUrl { get; set; }
        public string content { get; set; }
        public string description { get; set; }
        public string author { get; set; }
        public DateTime pubdate { get; set; }
    }

    public class RootObject
    {
        public string _index { get; set; }
        public string _type { get; set; }
        public string _id { get; set; }
        public int _score { get; set; }
        public Source _source { get; set; }
    }

    public class ESearchContainer
    {
        public ESearchContainer()
        {
            RootObject _root = new RootObject();
            Source _sourceItem = new Source();
        }
    }
}
