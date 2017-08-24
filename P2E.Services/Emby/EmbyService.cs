using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using P2E.DataObjects.Emby.Library;
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

        public async Task<IReadOnlyCollection<IFilenameIdentifier>> GetFilenameIdentifiersAsync(ILibraryIdentifier libraryIdentifier)
        {
            return await _repository.GetFilenameIdentifiersAsync(_client, libraryIdentifier);
        }

        public async Task<IMovieUpdateResult> UpdateItemAsync(IPlexMovieMetadata plexMovieMetadata, IFilenameIdentifier filenameIdentifier)
        {
            await SemSlim.WaitAsync();
            try
            {
                var embyCollectionIdentifiers = await _repository.GetCollectionsAsync(_client);

                foreach (var plexCollection in plexMovieMetadata.Collections)
                {
                    
                }

                var missingCollectionNames = plexMovieMetadata.Collections.Except(embyCollectionIdentifiers.Select(x => x.Name));

                //await Task.Delay(1000);
                _logger.Info($"Processing {plexMovieMetadata.Title}");
                return new MovieUpdateResult
                {
                    Filename = filenameIdentifier.Name,
                    Title = plexMovieMetadata.Title,
                    IsUpdated = false
                };
            }
            finally
            {
                SemSlim.Release();
            }
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
                SemSlim.Release();
            }

        }
    }
}
