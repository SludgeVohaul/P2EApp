using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Emby.Library;
using P2E.Interfaces.DataObjects.Plex.Library;

namespace P2E.Interfaces.AppLogic.Emby
{
    public interface IEmbyImportMovieMetadataLogic : ILogic
    {
        Task<bool> RunAsync(IPlexMovieMetadata plexMovieMetadata, IMovieIdentifier movieIdentifier);
    }
}