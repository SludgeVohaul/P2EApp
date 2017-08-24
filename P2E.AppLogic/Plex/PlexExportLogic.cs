using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using P2E.Interfaces.AppLogic.Plex;
using P2E.Interfaces.CommandLine;
using P2E.Interfaces.DataObjects.Plex;
using P2E.Interfaces.DataObjects.Plex.Library;
using P2E.Interfaces.Factories;
using P2E.Interfaces.Services.SpinWheel;

namespace P2E.AppLogic.Plex
{
    public class PlexExportLogic : IPlexExportLogic
    {
        private readonly IPlexClient _client;
        private readonly IServiceFactory _serviceFactory;
        private readonly IConsoleLibraryOptions _consoleLibraryOptions;

        public List<IPlexMovieMetadata> MovieMetadataItems { get; private set; }

        public PlexExportLogic(IPlexClient client,
            IServiceFactory serviceFactory,
            IConsoleLibraryOptions consoleLibraryOptions)
        {
            _client = client;
            _serviceFactory = serviceFactory;
            _consoleLibraryOptions = consoleLibraryOptions;
        }

        public async Task<bool> RunAsync()
        {
            var plexService = _serviceFactory.CreateService(_client);
            var spinWheelService = _serviceFactory.CreateService<ISpinWheelService>();
            var cts = new CancellationTokenSource();

            try
            {
                await spinWheelService.StartSpinWheelAsync(cts.Token);
                // TODO - This is not thread safe.
                MovieMetadataItems = await plexService.GetMovieMetadataAsync(_consoleLibraryOptions.PlexLibraryName);

                return MovieMetadataItems != null && MovieMetadataItems.Any();
            }
            finally
            {
                spinWheelService.StopSpinWheel(cts);
                cts.Dispose();
            }
        }
    }
}