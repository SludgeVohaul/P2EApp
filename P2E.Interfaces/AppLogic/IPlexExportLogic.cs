using System.Collections.Generic;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Plex.Library;

namespace P2E.Interfaces.AppLogic
{
    public interface IPlexExportLogic
    {
        List<IPlexMovieMetadata> MovieMetadataItems { get; }
        Task<bool> RunAsync();
    }
}