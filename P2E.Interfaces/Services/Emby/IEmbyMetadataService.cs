using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Emby.Library;
using P2E.Interfaces.DataObjects.Plex.Library;

namespace P2E.Interfaces.Services.Emby
{
    public interface IEmbyMetadataService : IService
    {
        Task<bool> UpdateWatchedStatusAsync(IPlexMovieMetadata plexMovieMetadata, IMovieIdentifier movieIdentifier);
        Task<bool> UpdateMetadataAsync(IPlexMovieMetadata movieMetadata, IMovieIdentifier movieIdentifier);
    }
}