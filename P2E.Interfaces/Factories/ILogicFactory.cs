using P2E.Interfaces.AppLogic;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Plex;

namespace P2E.Interfaces.Factories
{
    public interface ILogicFactory
    {
        IEmbyImportLogic CreateLogic(IEmbyClient client);
        IPlexExportLogic CreateLogic(IPlexClient client);
    }
}