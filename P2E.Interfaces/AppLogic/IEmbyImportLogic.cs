using System.Collections.Generic;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Plex.Library;

namespace P2E.Interfaces.AppLogic
{
    public interface IEmbyImportLogic
    {
        Task<bool> RunAsync(IEnumerable<IPlexMovieMetadata> plexMovieMetadataItems);
    }
}