using P2E.Interfaces.DataObjects.Plex;

namespace P2E.Interfaces.Repositories.Plex
{
    public interface IPlexRepository : IRepository
    {
        IPlexClient Client { get; set; }
    }
}
