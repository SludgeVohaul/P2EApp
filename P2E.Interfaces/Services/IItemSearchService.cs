using P2E.Interfaces.DataObjects.Emby;

namespace P2E.Interfaces.Services
{
    public interface IItemSearchService
    {
        IEmbyClient EmbyClient { get; set; }
        bool TryExecute();
    }
}