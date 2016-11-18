using P2E.Interfaces.DataObjects.Emby;

namespace P2E.Interfaces.Services.Emby
{
    public interface IEmbyService : IService
    {
        bool TryExecute(IEmbyClient embyClient);
    }
}
