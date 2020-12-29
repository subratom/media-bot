using Ninject;
using Ninject.Modules;
using Search.ElasticSearchMedia.Implementations;
using Search.ElasticSearchMedia.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search.DB.Bot.Classes
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            // configure IMailSender and ILogging to resolve to their specified concrete implementations
            Bind<ISearchApi>().To<SearchApi>();
        }
    }
}
