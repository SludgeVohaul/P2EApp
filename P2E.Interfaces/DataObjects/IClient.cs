using System.Threading.Tasks;


namespace P2E.Interfaces.DataObjects
{
    public interface IClient
    {
        IConnectionInformation ConnectionInformation { get; }

        Task LoginAsync(string loginname, string password);
        Task LogoutAsync();
    }
}
