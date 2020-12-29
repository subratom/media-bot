using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search.ElasticSearchMedia.Implementations
{
    public class MissingRequiredArticleFieldException : Exception
    {
        public string FieldName { get; private set; }

        public MissingRequiredArticleFieldException(string fieldName) : base("Specified field name "  + fieldName + " is either null or empty")
        {
            this.FieldName = fieldName;
        }
    }
}
