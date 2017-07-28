using System;
using System.Threading;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Plex.Library;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Services.Emby;
using P2E.Interfaces.Repositories.Emby;

namespace P2E.Services.Emby
{
    public class EmbyService : IEmbyService
    {
        // TODO - Is it a good idea to execute all updates asynchronously?
        // Do it sequentially first...
        private static readonly SemaphoreSlim SemSlim = new SemaphoreSlim(1, 1);

        private readonly IAppLogger _logger;
        private readonly IEmbyRepository _repository;

        public event EventHandler ItemProcessed;

        public EmbyService(IAppLogger logger, IEmbyRepository embyRepository)
        {
            _logger = logger;
            _repository = embyRepository;
        }

        public async Task DoItAsync(IEmbyClient client)
        {
            await _repository.GetStuffAsync(client);
        }

        public async Task<bool> UpdateItemAsync(IEmbyClient client, IPlexMovieMetadata plexMovieMetadata, string embyLibraryName)
        {
            await SemSlim.WaitAsync();
            try
            {
                await Task.Delay(2000);
                _logger.Debug($"Processing {plexMovieMetadata.Title}");
                return true;
            }
            finally
            {
                OnItemProcessed();
                SemSlim.Release();
            }

        }

        private void OnItemProcessed()
        {
            ItemProcessed?.Invoke(this, new EventArgs());
        }
    }
}
