using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using P2E.Interfaces.AppLogic;
using P2E.Interfaces.CommandLine;
using P2E.Interfaces.DataObjects.Plex;
using P2E.Interfaces.DataObjects.Plex.Library;
using P2E.Interfaces.Factories;
using P2E.Interfaces.Services.Plex;
using P2E.Interfaces.Services.SpinWheel;

namespace P2E.AppLogic
{
    public class PlexExportLogic : IPlexExportLogic
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IConsoleLibraryOptions _consoleLibraryOptions;

        public List<IPlexMovieMetadata> MovieMetadataItems { get; private set; }

        public PlexExportLogic(IServiceFactory serviceFactory, IConsoleLibraryOptions consoleLibraryOptions)
        {
            _serviceFactory = serviceFactory;
            _consoleLibraryOptions = consoleLibraryOptions;
        }

        public async Task<bool> RunAsync(IPlexClient plexClient)
        {
            var plexService = _serviceFactory.CreateService<IPlexService>();
            var spinWheelService = _serviceFactory.CreateService<ISpinWheelService>();

            using (var cts = new CancellationTokenSource())
            {
                await spinWheelService.StartSpinWheelAsync(cts.Token);
                // TODO - This is not thread safe.
                MovieMetadataItems = await plexService.GetMovieMetadataAsync(plexClient, _consoleLibraryOptions.PlexLibraryName);
                spinWheelService.StopSpinWheel(cts);

                return MovieMetadataItems != null && MovieMetadataItems.Any();
            }
        }
    }
}