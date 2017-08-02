using System.Collections.Generic;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Plex;
using P2E.Interfaces.DataObjects.Plex.Library;

namespace P2E.Interfaces.AppLogic
{
    public interface IPlexExportLogic : ILogic
    {
        List<IPlexMovieMetadata> MovieMetadataItems { get; }
        Task<bool> RunAsync(IPlexClient plexClient);
    }
}