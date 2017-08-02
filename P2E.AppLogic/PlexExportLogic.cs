using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using P2E.Interfaces.AppLogic;
using P2E.Interfaces.CommandLine;
using P2E.Interfaces.DataObjects.Plex;
using P2E.Interfaces.DataObjects.Plex.Library;
using P2E.Interfaces.Factories;
using P2E.Interfaces.Services.Plex;

namespace P2E.AppLogic
{
    public class PlexExportLogic : IPlexExportLogic
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IConsoleLibraryOptions _consoleLibraryOptions;

        public List<IPlexMovieMetadata> MovieMetadataItems { get; private set; }

        public PlexExportLogic(IServiceFactory serviceFactory,
            IConsoleLibraryOptions consoleLibraryOptions)
        {
            _serviceFactory = serviceFactory;
            _consoleLibraryOptions = consoleLibraryOptions;
        }

        public async Task<bool> RunAsync(IPlexClient plexClient)
        {
            await Task.Delay(5000);
            var plexService = _serviceFactory.CreateService<IPlexService>();
            MovieMetadataItems = await plexService.GetMovieMetadataAsync(plexClient, _consoleLibraryOptions.PlexLibraryName);

            return MovieMetadataItems != null && MovieMetadataItems.Any();
        }
    }
}