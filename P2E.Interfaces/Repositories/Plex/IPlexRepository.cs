using System.Collections.Generic;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Plex;
using P2E.Interfaces.DataObjects.Plex.Library;

namespace P2E.Interfaces.Repositories.Plex
{
    public interface IPlexRepository : IRepository
    {
        Task<string> GetLibraryIdAsync(IPlexClient client, string libraryName);
        Task<List<IPlexMovieMetadata>> GetMovieLibraryMetadataAsync(IPlexClient client, string libraryId);
    }
}
