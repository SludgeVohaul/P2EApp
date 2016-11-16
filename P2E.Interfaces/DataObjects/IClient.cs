using System.Threading.Tasks;


namespace P2E.Interfaces.DataObjects
{
    public interface IClient
    {
        IConnectionInformation ConnectionInformation { get; }

        // TODO -wrap the AuthenticateUserAsync and create a new method for plex, and use iconnectionservice
        Task LoginAsync(string loginname, string password);
        Task LogoutAsync();
    }
}
