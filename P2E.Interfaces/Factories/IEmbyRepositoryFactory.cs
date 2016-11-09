using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.Repositories;

namespace P2E.Interfaces.Factories
{
    public interface IEmbyRepositoryFactory
    {
        IEmbyRepository CreateEmbyRepository(IEmbyClient embyClient);
    }
}