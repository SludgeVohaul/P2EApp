using P2E.Interfaces.DataObjects.Emby;

namespace P2E.Interfaces.Repositories.Emby
{
    public interface IEmbyRepository : IRepository
    {
        IEmbyClient Client { get; set; }
        void GetStuff();
    }
}