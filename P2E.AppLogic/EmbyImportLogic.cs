using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using P2E.Interfaces.AppLogic;
using P2E.Interfaces.CommandLine;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Emby.Library;
using P2E.Interfaces.DataObjects.Plex.Library;
using P2E.Interfaces.Factories;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Services.Emby;
using P2E.Interfaces.Services.SpinWheel;

namespace P2E.AppLogic
{
    public class EmbyImportLogic : IEmbyImportLogic
    {
        private readonly IAppLogger _logger;
        private readonly IEmbyClient _client;
        private readonly IServiceFactory _serviceFactory;
        private readonly IConsoleLibraryOptions _consoleLibraryOptions;

        public EmbyImportLogic(IAppLogger logger,
            IEmbyClient client,
            IServiceFactory serviceFactory,
            IConsoleLibraryOptions consoleLibraryOptions)
        {
            _logger = logger;
            _client = client;
            _serviceFactory = serviceFactory;
            _consoleLibraryOptions = consoleLibraryOptions;
        }

        public async Task<bool> RunAsync(IEnumerable<IPlexMovieMetadata> plexMovieMetadataItems)
        {
            var spinWheelService = _serviceFactory.CreateService<ISpinWheelService>();
            var embyService = _serviceFactory.CreateService(_client);

            var libraryIdentifier = await GetLibraryIdentifierAsync(embyService, spinWheelService, _consoleLibraryOptions.EmbyLibraryName);
            if (libraryIdentifier == null)
            {
                _logger.Error($"Cannot find Emby library '{_consoleLibraryOptions.EmbyLibraryName}'.");
                return false;
            }

            var filenameIdentifiers = await GetFilenameIdentifiersAsync(embyService, spinWheelService, libraryIdentifier);
            if (filenameIdentifiers.Any() == false)
            {
                _logger.Error($"No movie files found in Emby library '{libraryIdentifier.Name}'.");
                return false;
            }

            var didUpdateAll = await UpdateAllEmbyMovieMetadataAsync(embyService, spinWheelService, plexMovieMetadataItems, _consoleLibraryOptions.EmbyLibraryName);
            return didUpdateAll;

            return true;
        }

        private async Task<ILibraryIdentifier> GetLibraryIdentifierAsync(IEmbyService embyService, ISpinWheelService spinWheelService, string libraryName)
        {
            var cts = new CancellationTokenSource();
            try
            {
                await spinWheelService.StartSpinWheelAsync(cts.Token);
                return await embyService.GetLibraryIdentifierAsync(libraryName);
            }
            finally
            {
                spinWheelService.StopSpinWheel(cts);
                cts.Dispose();
            }
        }

        private async Task<IFilenameIdentifier[]> GetFilenameIdentifiersAsync(IEmbyService embyService, ISpinWheelService spinWheelService, ILibraryIdentifier libraryIdentifier)
        {
            var cts = new CancellationTokenSource();
            try
            {
                await spinWheelService.StartSpinWheelAsync(cts.Token);
                return await embyService.GetFilenameIdentifiersAsync(libraryIdentifier);
            }
            finally
            {
                spinWheelService.StopSpinWheel(cts);
                cts.Dispose();
            }
        }

        private async Task<bool> UpdateAllEmbyMovieMetadataAsync(IEmbyService embyService, ISpinWheelService spinWheelService, IEnumerable<IPlexMovieMetadata> movieMetadataItems, string embyLibraryName)
        {
            try
            {
                embyService.ItemProcessed += spinWheelService.OnItemProcessed;

                using (var cts = new CancellationTokenSource())
                {
                    await spinWheelService.StartSpinWheelAsync(cts.Token);
                    var updateTask = new Func<IPlexMovieMetadata, Task<bool>>(x => embyService.UpdateItemAsync(x, embyLibraryName));
                    var updateTasksCompletedTask = await Task.WhenAll(movieMetadataItems.ToArray().Take(5).Select(updateTask));
                    spinWheelService.StopSpinWheel(cts);

                    return updateTasksCompletedTask.All(x => x);
                }
            }
            finally
            {
                embyService.ItemProcessed -= spinWheelService.OnItemProcessed;
            }
        }
    }
}