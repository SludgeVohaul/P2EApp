using System.Threading.Tasks;
using P2E.Interfaces.DataObjects;
using P2E.Interfaces.Services;

namespace P2E.Services
{
    public class ConnectionService : IConnectionService  
    {
        public async Task LoginAsync(IClient client, IUserCredentials userCredentials)
        {
            await client.LoginAsync(userCredentials?.Loginname, userCredentials?.Password);
        }

        public async Task LogoutAsync(IClient client)
        {
            await client.LogoutAsync();
        }


    }
}
