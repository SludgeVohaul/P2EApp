using System;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Emby.Library;
using P2E.Interfaces.DataObjects.Plex.Library;

namespace P2E.Interfaces.AppLogic.Emby
{
    public interface IEmbyImportMovieLogic
    {
        event EventHandler ItemProcessed;

        Task<bool> RunAsync(IPlexMovieMetadata plexMovieMetadata, IMovieIdentifier embyMovieIdentifier);
    }
}