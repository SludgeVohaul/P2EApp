using System.Collections.Generic;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Plex.Library;

namespace P2E.Interfaces.AppLogic.Plex
{
    public interface IPlexExportLogic : ILogic
    {
        List<IPlexMovieMetadata> MovieMetadataItems { get; }
        Task<bool> RunAsync();
    }
}