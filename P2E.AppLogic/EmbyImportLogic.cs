using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using P2E.Interfaces.AppLogic;
using P2E.Interfaces.CommandLine;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Plex.Library;
using P2E.Interfaces.Factories;
using P2E.Interfaces.Services.Emby;
using P2E.Interfaces.Services.SpinWheel;

namespace P2E.AppLogic
{
    public class EmbyImportLogic : IEmbyImportLogic
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IConsoleLibraryOptions _consoleLibraryOptions;

        public EmbyImportLogic(IServiceFactory serviceFactory,
            IConsoleLibraryOptions consoleLibraryOptions)
        {
            _serviceFactory = serviceFactory;
            _consoleLibraryOptions = consoleLibraryOptions;
        }

        public async Task<bool> RunAsync(IEmbyClient embyClient, IList<IPlexMovieMetadata> plexMovieMetadataItems)
        {

            var didUpdateAll = await UpdateAllEmbyMovieMetadataAsync(embyClient, plexMovieMetadataItems, _consoleLibraryOptions.EmbyLibraryName);

            return didUpdateAll;
        }

        private async Task<bool> UpdateAllEmbyMovieMetadataAsync(IEmbyClient embyClient, IEnumerable<IPlexMovieMetadata> movieMetadataItems, string embyLibraryName)
        {
            var spinWheelService = _serviceFactory.CreateService<ISpinWheelService>();
            var embyService = _serviceFactory.CreateService<IEmbyService>();
            embyService.ItemProcessed += spinWheelService.OnItemProcessed;

            using (var cts = new CancellationTokenSource())
            {
                await spinWheelService.StartSpinWheelAsync(cts.Token);
                var updateTask = new Func<IPlexMovieMetadata, Task<bool>>(x => embyService.UpdateItemAsync(embyClient, x, embyLibraryName));
                var updateTasksCompletedTask = await Task.WhenAll(movieMetadataItems.ToArray().Take(1).Select(updateTask));
                spinWheelService.StopSpinWheel(cts);

                return updateTasksCompletedTask.All(x => x);
            }
        }
    }
}