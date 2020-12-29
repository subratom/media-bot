using Ninject;
using Ninject.Modules;
using Search.ElasticSearchMedia;
using Search.ElasticSearchMedia.Implementations;
using Search.ElasticSearchMedia.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Search.Sitemap.Bot
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            // configure IMailSender and ILogging to resolve to their specified concrete implementations
#if DEBUG
            Bind<ISearchApi>().To<DevSearchApi>();
#else
            Bind<ISearchApi>().To<SearchApi>();
#endif

        }
    }
}