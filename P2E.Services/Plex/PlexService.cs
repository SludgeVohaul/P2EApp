using System.Collections.Generic;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Plex;
using P2E.Interfaces.DataObjects.Plex.Library;
using P2E.Interfaces.Services.Plex;
using P2E.Interfaces.Repositories.Plex;

namespace P2E.Services.Plex
{
    public class PlexService : IPlexService
    {
        private readonly IPlexRepository _repository;

        public PlexService(IPlexRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> GetLibraryUrlAsync(IPlexClient client, string libraryName)
        {
            return await _repository.GetLibraryUrlAsync(client, libraryName);
        }

        public async Task<List<IPlexMovieMetadata>> GetMovieMetadataAsync(IPlexClient client, string libraryUrl)
        {
            return await _repository.GetMovieMetadataAsync(client, libraryUrl);
        }
    }
}
