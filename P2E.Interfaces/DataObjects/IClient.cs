using System.Threading.Tasks;
using P2E.Interfaces.Services;


namespace P2E.Interfaces.DataObjects
{
    public interface IClient
    {
        /// <summary>
        /// The name of the server the client connects to.
        /// </summary>
        string ServerType { get; }
        void SetLoginData(IUserCredentialsService userCredentialsService);
        Task LoginAsync();
        Task LogoutAsync();
    }
}
