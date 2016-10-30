using Emby.ApiInteraction.Cryptography;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Logging;
using Ninject;
using Ninject.Extensions.NamedScope;
using P2E.AppLogic;
using P2E.CommandLine;
using P2E.DataObjects;
using P2E.DataObjects.Emby;
using P2E.Interfaces.AppLogic;
using P2E.Interfaces.CommandLine;
using P2E.Interfaces.CommandLine.ServerOptions;
using P2E.Interfaces.DataObjects;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.Repositories;
using P2E.Interfaces.Services;
using P2E.Logging;
using P2E.Repositories;
using P2E.Services;

namespace P2EApp
{
    internal class Bootstrapper
    {
        public static void ConfigureContainer(IKernel kernel)
        {
            kernel.Bind<IConsoleOptions, IConsolePlexConnectionOptions, IConsoleEmbyConnectionOptions>().To<ConsoleOptions>().InSingletonScope();
            kernel.Bind<ILogger>().To<ConsoleLogger>().InSingletonScope();

            const string scopeName = "EmbyRepository";
            kernel.Bind<IEmbyRepository>().To<EmbyRepository>().DefinesNamedScope(scopeName);
            kernel.Bind<IEmbyClient>().To<EmbyClient>().InNamedScope(scopeName);

            kernel.Bind<IDevice>().To<Device>().InSingletonScope();
            kernel.Bind<ICryptographyProvider>().To<CryptographyProvider>();
            kernel.Bind<IEmbyConnectionService>().To<EmbyConnectionService>();
            kernel.Bind<IItemSearchService>().To<ItemSearchService>();
            kernel.Bind<IUserCredentialsService>().To<UserCredentialsService>();
            kernel.Bind<ILogic>().To<Logic>().InSingletonScope();
            kernel.Bind<IApplicationInformation>().To<ApplicationInformation>().InSingletonScope();
            kernel.Bind<IConnectionInformation>().To<EmbyConnectionInformation>().WhenInjectedInto<IEmbyClient>();            
        }
    }
}