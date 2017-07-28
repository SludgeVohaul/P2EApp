using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Emby;

namespace P2E.Interfaces.Repositories.Emby
{
    public interface IEmbyRepository : IRepository
    {
        Task GetStuffAsync(IEmbyClient client);
    }
}