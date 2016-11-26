using System.Threading.Tasks;
using MediaBrowser.Model.Logging;
using P2E.Interfaces.DataObjects;
using P2E.Interfaces.Services;

namespace P2E.Services
{
    public class ConnectionService : IConnectionService  
    {
        private readonly ILogger _logger;

        public ConnectionService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task LoginAsync(IClient client, IUserCredentials userCredentials)
        {
            using (var spinWheel = new SpinWheel(_logger))
            {
                var ignoreTask = spinWheel.SpinAsync();
                await client.LoginAsync(userCredentials?.Loginname, userCredentials?.Password);
            }
        }

        public async Task LoginAsync(IClient client)
        {
            // TODO use postsharp?
            using (var spinWheel = new SpinWheel(_logger))
            {
                var ignoreTask = spinWheel.SpinAsync();
                await LoginAsync(client, null);
            }
        }

        public async Task  LogoutAsync(IClient client)
        {
            
            using (var spinWheel = new SpinWheel(_logger))
            {
                var ignoreTask = spinWheel.SpinAsync();
                await client.LogoutAsync();
            }
        }
    }
}
