namespace P2E.Interfaces.Repositories
{
    public interface IEmbyRepository
    {
        bool TryConnect();
        void Disconnect();

        //void SetClientCapabilities();

        void GetStuff();
    }
}