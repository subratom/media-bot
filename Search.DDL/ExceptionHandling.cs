using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search.MediaStore.Exceptions
{
    public class ExceptionHandling : Exception
    {
        public string FieldName { get; private set; }
        public ExceptionHandling(string fieldName) : base("Specified fieled name is either null or empty")
        {
            this.FieldName = fieldName;
        }
    }
}

