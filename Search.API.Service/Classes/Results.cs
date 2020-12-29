using Search.API.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Search.API.Service.Classes
{
    [DataContract]
    public class ResultList
    {
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public int ResultCount { get; set; }
        [DataMember]
        public List<Articles> Results { get; set; }
    }
}