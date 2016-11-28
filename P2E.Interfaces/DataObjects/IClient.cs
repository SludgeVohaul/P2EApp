using System.Threading.Tasks;


namespace P2E.Interfaces.DataObjects
{
    public interface IClient
    {
        IConnectionInformation ConnectionInformation { get; }

        Task<bool> TryLoginAsync(string loginname, string password);
        Task LogoutAsync();
    }
}
