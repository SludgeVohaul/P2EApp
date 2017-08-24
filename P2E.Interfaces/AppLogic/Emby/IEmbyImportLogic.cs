using System.Collections.Generic;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Plex.Library;

namespace P2E.Interfaces.AppLogic.Emby
{
    public interface IEmbyImportLogic
    {
        Task<bool> RunAsync(IReadOnlyCollection<IPlexMovieMetadata> plexMovieMetadataItems);
    }
}