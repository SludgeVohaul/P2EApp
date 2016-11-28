using System.Threading.Tasks;
using P2E.Interfaces.DataObjects;
using P2E.Interfaces.Services;

namespace P2E.Services
{
    public class ConnectionService : IConnectionService  
    {
        public async Task<bool> TryLoginAsync(IClient client, IUserCredentials userCredentials)
        {
            return await client.TryLoginAsync(userCredentials?.Loginname, userCredentials?.Password);
        }

        public async Task LogoutAsync(IClient client)
        {
            await client.LogoutAsync();
        }
    }
}
