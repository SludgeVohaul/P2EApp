using System.Collections.Generic;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Emby.Library;
using P2E.Interfaces.DataObjects.Plex.Library;

namespace P2E.Interfaces.Services.Emby
{
    public interface IEmbyService : IService
    {
        Task<ILibraryIdentifier> GetLibraryIdentifierAsync(string libraryName);
        Task<IReadOnlyCollection<IFilenameIdentifier>> GetFilenameIdentifiersAsync(ILibraryIdentifier libraryIdentifier);

        Task<IMovieUpdateResult> UpdateItemAsync(IPlexMovieMetadata plexMovieMetadata, IFilenameIdentifier filenameIdentifier);
        Task<bool> UpdateItemAsync(IPlexMovieMetadata plexMovieMetadata, string embyLibraryName);

    }
}
