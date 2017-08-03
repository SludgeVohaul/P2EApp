using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Emby.Library;
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

        private readonly IEmbyClient _client;
        private readonly IAppLogger _logger;
        private readonly IEmbyRepository _repository;

        public event EventHandler ItemProcessed;

        public EmbyService(IAppLogger logger, IEmbyClient client, IEmbyRepository embyRepository)
        {
            _client = client;
            _logger = logger;
            _repository = embyRepository;
        }

        public async Task<ILibraryIdentifier> GetLibraryIdentifierAsync(string libraryName)
        {
            var libraryIdentifiers = await _repository.GetLibraryIdentifiersAsync(_client);
            return libraryIdentifiers
                .FirstOrDefault(x => x.Name == libraryName);
        }

        public async Task<IFilenameIdentifier[]> GetFilenameIdentifiersAsync(ILibraryIdentifier libraryIdentifier)
        {
            return await _repository.GetFilenameIdentifiersAsync(_client, libraryIdentifier);
        }

        public async Task<bool> UpdateItemAsync(IPlexMovieMetadata plexMovieMetadata, string embyLibraryName)
        {
            await SemSlim.WaitAsync();
            try
            {
                //var movieIds = await _repository.GetMovieIdsAsync(_client, embyLibraryName);




                await Task.Delay(1000);
                _logger.Info($"Processing {plexMovieMetadata.Title}");
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
