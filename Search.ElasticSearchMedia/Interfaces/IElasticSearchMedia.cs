using Search.API.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search.ElasticSearchMedia.Interfaces
{
    public interface IElasticSearchMedia
    {
        bool ClearParts(Articles newArticle);
        bool Upsert(Articles newArticle);
        void CreateTestDocuments(int total);
    }
}
