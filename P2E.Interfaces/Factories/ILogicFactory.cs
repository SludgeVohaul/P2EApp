using P2E.Interfaces.AppLogic.Emby;
using P2E.Interfaces.AppLogic.Plex;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Plex;

namespace P2E.Interfaces.Factories
{
    public interface ILogicFactory
    {
        IEmbyImportLogic CreateEmbyImportLogic(IEmbyClient client);
        IEmbyImportMovieLogic CreateEmbyImportMovieLogic(IEmbyClient client);
        IPlexExportLogic CreatePlexExportLogic(IPlexClient client);
    }
}