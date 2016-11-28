using System.Collections.Generic;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Plex;
using P2E.Interfaces.DataObjects.Plex.Library;

namespace P2E.Interfaces.Repositories.Plex
{
    public interface IPlexRepository : IRepository
    {
        Task<string> GetLibraryUrlAsync(IPlexClient client, string libraryName);
        Task<List<IPlexMovieMetadata>> GetMovieMetadataAsync(IPlexClient client, string libraryUrl);
    }
}
