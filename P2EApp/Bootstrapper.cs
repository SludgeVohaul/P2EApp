using Emby.ApiClient.Cryptography;
using Emby.ApiClient.Model;
using Ninject;
using Ninject.Extensions.Factory;
using P2E.AppLogic;
using P2E.AppLogic.Emby;
using P2E.AppLogic.Plex;
using P2E.CommandLine;
using P2E.DataObjects;
using P2E.DataObjects.Emby;
using P2E.DataObjects.Plex;
using P2E.Interfaces.AppLogic;
using P2E.Interfaces.AppLogic.Emby;
using P2E.Interfaces.AppLogic.Plex;
using P2E.Interfaces.CommandLine;
using P2E.Interfaces.CommandLine.ServerOptions;
using P2E.Interfaces.DataObjects;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Plex;
using P2E.Interfaces.Factories;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Repositories.Emby;
using P2E.Interfaces.Repositories.Plex;
using P2E.Interfaces.Services;
using P2E.Interfaces.Services.Emby;
using P2E.Interfaces.Services.Plex;
using P2E.Interfaces.Services.SpinWheel;
using P2E.Logging;
using P2E.Repositories.Emby;
using P2E.Repositories.Plex;
using P2E.Services;
using P2E.Services.Emby;
using P2E.Services.Plex;
using P2E.Services.SpinWheel;


namespace P2EApp
{
    internal static class Bootstrapper
    {
        public static void ConfigureContainer(IKernel kernel)
        {
            kernel.Bind(
                typeof(IConsoleOptions),
                typeof(IConsoleEmbyInstance1ConnectionOptions),
                typeof(IConsolePlexInstance1ConnectionOptions),
                typeof(IConsoleLibraryOptions),
                typeof(IConsoleSyncOptions)).To<ConsoleOptions>().InSingletonScope();

            kernel.Bind<IAppLogger>().To<ConsoleLogger>().InSingletonScope();

            kernel.Bind<IConnectionInformation<IConsoleEmbyInstance1ConnectionOptions>>().To<ConnectionInformation<IConsoleEmbyInstance1ConnectionOptions>>();
            kernel.Bind<IConnectionInformation<IConsolePlexInstance1ConnectionOptions>>().To<ConnectionInformation<IConsolePlexInstance1ConnectionOptions>>();

            kernel.Bind<IUserCredentials>().To<UserCredentials>();

            kernel.Bind<IDevice>().To<Device>().InSingletonScope();
            kernel.Bind<ICryptographyProvider>().To<CryptographyProvider>();

            kernel.Bind<IApplicationInformation>().To<ApplicationInformation>().InSingletonScope();

            kernel.Bind<ISpinWheel>().To<SpinWheel>().InSingletonScope();

            kernel.Bind<IEmbyClient>().To<EmbyClient>().InSingletonScope();
            kernel.Bind<IPlexClient>().To<PlexClient>().InSingletonScope();

            kernel.Bind<IClientFactory>().ToFactory();
            kernel.Bind<IUserCredentialsFactory>().ToFactory();
            kernel.Bind<IConnectionInformationFactory>().ToFactory();
            kernel.Bind<IServiceFactory>().ToFactory();
            kernel.Bind<ILogicFactory>().ToFactory().InSingletonScope();

            kernel.Bind<IMainLogic>().To<MainLogic>().InSingletonScope();
            kernel.Bind<IEmbyImportLogic>().To<EmbyImportLogic>().InSingletonScope();
            kernel.Bind<IEmbyImportMovieLogic>().To<EmbyImportMovieLogic>().InSingletonScope();
            kernel.Bind<IEmbyImportMovieImagesLogic>().To<EmbyImportMovieImagesLogic>().InSingletonScope();
            kernel.Bind<IPlexExportLogic>().To<PlexExportLogic>().InSingletonScope();

            kernel.Bind<IUserCredentialsService>().To<UserCredentialsService>();
            kernel.Bind<IEmbyService>().To<EmbyService>();
            kernel.Bind<IEmbyImageService>().To<EmbyImageService>();
            kernel.Bind<IPlexService>().To<PlexService>();
            kernel.Bind<ISpinWheelService>().To<SpinWheelService>();

            kernel.Bind<IEmbyRepository>().To<EmbyRepository>();
            kernel.Bind<IPlexRepository>().To<PlexRepository>();
        }
    }
}