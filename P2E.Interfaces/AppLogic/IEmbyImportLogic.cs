using System.Collections.Generic;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Plex.Library;

namespace P2E.Interfaces.AppLogic
{
    public interface IEmbyImportLogic : ILogic
    {
        Task<bool> RunAsync(IEmbyClient embyClient, IList<IPlexMovieMetadata> plexMovieMetadataItems);
    }
}