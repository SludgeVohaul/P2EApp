using System;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Plex.Library;

namespace P2E.Interfaces.Services.Emby
{
    public interface IEmbyService : IService
    {
        event EventHandler ItemProcessed;

        Task DoItAsync(IEmbyClient client);
        Task<bool> UpdateItemAsync(IEmbyClient client, IPlexMovieMetadata plexMovieMetadata, string embyLibraryName);
    }
}
