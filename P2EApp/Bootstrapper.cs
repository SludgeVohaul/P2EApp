using Emby.ApiInteraction.Cryptography;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Logging;
using Ninject;
using Ninject.Extensions.Factory;
using P2E.AppLogic;
using P2E.CommandLine;
using P2E.DataObjects;
using P2E.DataObjects.Emby;
using P2E.Interfaces.AppLogic;
using P2E.Interfaces.CommandLine;
using P2E.Interfaces.CommandLine.LibraryOptions;
using P2E.Interfaces.CommandLine.ServerOptions;
using P2E.Interfaces.DataObjects;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.Factories;
using P2E.Interfaces.Repositories;
using P2E.Interfaces.Services;
using P2E.Logging;
using P2E.Repositories.Emby;
using P2E.Services;

namespace P2EApp
{
    internal class Bootstrapper
    {
        public static void ConfigureContainer(IKernel kernel)
        {
            kernel.Bind(
                typeof(IConsoleOptions),
                typeof(IConsoleEmbyInstance1ConnectionOptions), 
                typeof(IConsolePlexInstance1ConnectionOptions),
                typeof(IConsolePlexLibraryOptions),
                typeof(IConsoleEmbyLibraryOptions)).To<ConsoleOptions>().InSingletonScope();

            kernel.Bind<ILogger>().To<ConsoleLogger>().InSingletonScope();

            kernel.Bind<IConnectionInformation<IConsoleEmbyInstance1ConnectionOptions>>().To<ConnectionInformation<IConsoleEmbyInstance1ConnectionOptions>>();
            kernel.Bind<IConnectionInformation<IConsolePlexInstance1ConnectionOptions>>().To<ConnectionInformation<IConsolePlexInstance1ConnectionOptions>>();

            kernel.Bind<IUserCredentials>().To<UserCredentials>();

            kernel.Bind<IDevice>().To<Device>().InSingletonScope();
            kernel.Bind<ICryptographyProvider>().To<CryptographyProvider>();

            kernel.Bind<IApplicationInformation>().To<ApplicationInformation>().InSingletonScope();

            kernel.Bind<IEmbyClient>().To<EmbyClient>();

            kernel.Bind<IEmbyClientFactory>().ToFactory();
            kernel.Bind<IUserCredentialsFactory>().ToFactory();
            kernel.Bind<IConnectionInformationFactory>().ToFactory();
            kernel.Bind<IServiceFactory>().ToFactory();
            kernel.Bind<IEmbyRepositoryFactory>().ToFactory();

            kernel.Bind<IUserCredentialsService>().To<UserCredentialsService>();
            kernel.Bind<IEmbyConnectionService>().To<EmbyConnectionService>();
            kernel.Bind<IItemSearchService>().To<ItemSearchService>();

            kernel.Bind<IEmbyRepository>().To<EmbyRepository>();

            kernel.Bind<ILogic>().To<Logic>().InSingletonScope();            
        }
    }
}