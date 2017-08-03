using System.Collections.Generic;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Plex.Library;

namespace P2E.Interfaces.Services.Plex
{
    public interface IPlexService : IService
    {
        Task<List<IPlexMovieMetadata>> GetMovieMetadataAsync(string libraryUrl);
    }
}
