using Ninject.Modules;
using Search.ElasticSearchMedia.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Search.ElasticSearchMedia.Implementations;


namespace Search.ElasticSearchMedia
{
    public class NinjectDI : NinjectModule
    {
        public override void Load()
        {
#if DEBUG
            Bind<ISearchApi>().To<DevSearchApi>();
#elif BETA
            Bind<ISearchApi>().To<MockSearchApi>();
#else
            Bind<ISearchApi>().To<SearchApi>();
#endif
        }
    }
}
