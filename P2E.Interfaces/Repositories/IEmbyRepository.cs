using P2E.Interfaces.DataObjects.Emby;

namespace P2E.Interfaces.Repositories
{
    public interface IEmbyRepository
    {
        IEmbyClient EmbyClient { get; set; }
        //void SetClientCapabilities();

        void GetStuff();
    }
}