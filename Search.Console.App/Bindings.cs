using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Modules;
using Search.ElasticSearchMedia.Interfaces;
using Search.ElasticSearchMedia;
using Search.ElasticSearchMedia.Implementations;

namespace Search.Console.App
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
#if DEBUG
            Bind<ISearchApi>().To<SearchApi>();
#endif
        }
    }
}
