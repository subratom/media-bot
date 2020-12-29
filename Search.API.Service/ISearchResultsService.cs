using Search.API.Service.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Search.API.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ISearchResultsService
    {

        //[WebInvoke]
        //[OperationContract]
        //IQueryable<ResultList> GetResultsByPartNumbers(string partNumbers, string[] additionalKeywords, int maxResults);

        [WebInvoke]
        [OperationContract]
        string GetResultsByPartNumbers(string partNumbers, string[] additionalKeywords, int maxResults);


        // TODO: Add your service operations here
    }
}
