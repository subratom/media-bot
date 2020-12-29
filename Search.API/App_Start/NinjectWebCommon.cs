[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Search.API.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Search.API.App_Start.NinjectWebCommon), "Stop")]

namespace Search.API.App_Start
{
    using System;
    using System.Web;
    using System.Web.Http;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Web.Common.WebHost;
    using Ninject.Web.WebApi;
    using Search.API.Implementation;
    using Search.API.Interfaces;
    using Search.ElasticSearchMedia.Implementations;
    using Search.ElasticSearchMedia.Interfaces;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            //bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                RegisterServices(kernel);
                GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
#if DEBUG
            kernel.Bind<ISearchApi>().To<DevSearchApi>();
#elif BETA
            kernel.Bind<ISearchApi>().To<MockSearchApi>();
#else
            kernel.Bind<ISearchApi>().To<SearchApi>();
#endif

            kernel.Bind<ILogger>().To<Logger>();

            //kernel.Bind<ILogger>().ToMethod(ctx =>
            //{
            //    var name = ctx.Request.Target.Member.DeclaringType.FullName;
            //    var loggerFactory = ctx.Kernel.Get<ILogger>();
            //    var log4Netlogger = ;
            //    return new Logger(log4Netlogger);
            //});
            //kernel.Bind<ILogger>().To<ILogger>();

        }
    }

}
