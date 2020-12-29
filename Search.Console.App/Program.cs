using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using System.Reflection;
using Search.ElasticSearchMedia.Interfaces;
using Search.ElasticSearchMedia;

namespace Search.Console.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel();

            kernel.Load(Assembly.GetExecutingAssembly());

            var search = kernel.Get<ISearchApi>();
            //List<Hit> hits = search.RelatedPartNumber("lm31");
            //List<Hit> hits = search.RelatedContent("http://www.electronicproducts.com/Programming/Software/Google_s_Daydream_virtual_reality_specification_boosts_smartphone_processor_performance_demands.aspx", 5);
            //if (hits.Count > 0)
            //{
            //    foreach (var item in hits)
            //    {
            //        System.Console.WriteLine("Item is " + item.Id);
            //    }
            //}
        }
    }
}
