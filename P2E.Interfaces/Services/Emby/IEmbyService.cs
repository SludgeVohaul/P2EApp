using System;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Emby.Library;
using P2E.Interfaces.DataObjects.Plex.Library;

namespace P2E.Interfaces.Services.Emby
{
    public interface IEmbyService : IService
    {
        event EventHandler ItemProcessed;

        Task<ILibraryIdentifier> GetLibraryIdentifierAsync(string libraryName);
        Task<IFilenameIdentifier[]> GetFilenameIdentifiersAsync(ILibraryIdentifier libraryIdentifier);

        Task<bool> UpdateItemAsync(IPlexMovieMetadata plexMovieMetadata, string embyLibraryName);
    }
}
