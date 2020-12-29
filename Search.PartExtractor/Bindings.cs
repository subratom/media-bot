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

namespace Search.PartExtractor
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
#if DEBUG
            Bind<ISearchApi>().To<DevSearchApi>();
#else
            Bind<ISearchApi>().To<SearchApi>();
#endif
        }
    }
}
