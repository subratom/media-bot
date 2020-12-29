using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search.ElasticSearchMedia
{
    //public class Articles
    //{
    /*
     https://www.elastic.co/guide/en/elasticsearch/guide/current/_finding_exact_values.html

            PUT /media
            {
              "settings":{
                "analysis": {
                  "analyzer": {
                    "lowercasespaceanalyzer": {
                      "type": "custom",
                      "tokenizer": "whitespace",
                      "filter": [
                        "lowercase"
                      ]
                    }
                  }
                }  
              },
              "mappings": {
                "articles": {
                  "properties": {
                    "title": {
                      "type": "text"
                    },
                    "url": {
                      "type": "keyword",
                      "index": "true"
                    },
                    "imageurl": {
                      "type": "keyword",
                      "index": "true"
                    },
                    "content": {
                      "type": "text",
                      "analyzer": "lowercasespaceanalyzer"
                      },
                    "description": {
                      "type": "text"
                    },
                    "relatedcontentwords": {
                      "type": "text"
                    },
                    "cmskeywords": {
                      "type": "text"
                    },
                    "partnumbers": {
                      "type": "keyword",
                      "index": "true"
                    },
                    "pubdate": {
                      "type": "date"
                    }
                  }
                }
              }
            }

    */

    //03/19/2018 

    /*
     PUT /media
{
"settings":{
"analysis": {
"analyzer": {
"lowercasespaceanalyzer": {
  "type": "custom",
  "tokenizer": "whitespace",
  "filter": [
    "lowercase"
  ]
}
},
"normalizer": {
"lowercasenormalizer": {
  "type": "custom",
  "filter":  [ "lowercase"]
}
}
}  
},
"mappings": {
"articles": {
"properties": {
"title": {
  "type": "text"
},
"url": {
  "type": "keyword",
  "index": "true"
},
"imageurl": {
  "type": "keyword",
  "index": "true"
},
"content": {
  "type": "text",
  "analyzer": "lowercasespaceanalyzer"
  },
"description": {
  "type": "text"
},
"relatedcontentwords": {
  "type": "text"
},
"cmskeywords": {
  "type": "text"
},
"partnumbers": {
  "type": "keyword",
  "index": "true",
  "normalizer": "lowercasenormalizer"
},
"pubdate": {
  "type": "date"
}
}
}
}
}


     */




    // the rest of these are from source
    //    public string Site { get; set; }
    //    public string Title { get; set; }
    //    public string Url { get; set; }
    //    public string Imageurl { get; set; }
    //    public string Content { get; set; }
    //    public string Description { get; set; }
    //    public string Relatedcontentwords { get; set; }
    //    public string[] Cmskeywords { get; set; }
    //    public string[] Partnumbers { get; set; }
    //    public DateTime Pubdate { get; set; }

    //    public void Sync(Articles existingArticle)
    //    {
    //        if (string.IsNullOrEmpty(this.Site) == true)
    //        {
    //            this.Site = existingArticle.Site;
    //        }

    //        if (string.IsNullOrEmpty(this.Title) == true)
    //        {
    //            this.Title = existingArticle.Title;
    //        }

    //        if (string.IsNullOrEmpty(this.Url) == true)
    //        {
    //            this.Url = existingArticle.Url;
    //        }

    //        if (string.IsNullOrEmpty(this.Imageurl) == true)
    //        {
    //            this.Imageurl = existingArticle.Imageurl;
    //        }

    //        if (string.IsNullOrEmpty(this.Content) == true)
    //        {
    //            this.Content = existingArticle.Content;
    //        }

    //        if (string.IsNullOrEmpty(this.Description) == true)
    //        {
    //            this.Description = existingArticle.Description;
    //        }

    //        if (string.IsNullOrEmpty(this.Relatedcontentwords) == true)
    //        {
    //            this.Relatedcontentwords = existingArticle.Relatedcontentwords;
    //        }

    //        if (this.Cmskeywords == null)
    //        {
    //            this.Cmskeywords = existingArticle.Cmskeywords;
    //        }

    //        if (this.Partnumbers == null)
    //        {
    //            this.Partnumbers = existingArticle.Partnumbers;
    //        }

    //        if (this.Pubdate != null)
    //        {
    //            this.Pubdate = existingArticle.Pubdate;
    //        }

    //    }

    //    public override string ToString()
    //    {

    //        string result = $"*** Pub Date {this.Pubdate}, Title: {this.Title}, Url: {this.Url}\n";

    //        result += $"Image URL: {this.Imageurl}\n";

    //        result += "CMS Keywords: ";
    //        foreach (string word in Cmskeywords)
    //        {
    //            result += $"{word} ";
    //        }
    //        result += "\n";

    //        result += "Part Numbers: ";
    //        foreach (string part in Partnumbers)
    //        {
    //            result += $"{part} ";
    //        }
    //        result += "\n";

    //        result += $"Content: {this.Content}\n";

    //        return result;
    //    }
    //}
}
