using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Search.API.Classes
{
    //public class Authors
    //{
    //    private string _name;
    //    private string _imageUrl;

    //    public string Name
    //    {
    //        get { return _name; }
    //        set { _name = value; }
    //    }

    //    public string ImageUrl
    //    {
    //        get { return _imageUrl; }
    //        set { _imageUrl = value; }
    //    }
    //}

    [XmlRoot(ElementName = "author")]
    public class Author
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "url")]
        public string Url { get; set; }
    }

    [XmlRoot(ElementName = "authors")]
    [XmlInclude(typeof(Author))] // include type class Person
    public class Authors
    {
        [XmlElement(ElementName = "author")]
        public List<Author> AuthorList { get; set; }

        public Authors() { AuthorList = new List<Author>(); }
    }
}
