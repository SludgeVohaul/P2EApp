using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P2E.ExtensionMethods;
using MediaBrowser.Model.Logging;
using P2E.Interfaces.DataObjects;

namespace P2E.Interfaces.Services
{
    public class ConnectionService : IConnectionService  
    {
        private readonly ILogger _logger;

        public ConnectionService(ILogger logger)
        {
            _logger = logger;
        }

        public bool TryLogin(IClient client)
        {
            return TryLogin(client, null);
        }

        public bool TryLogin(IClient client, IUserCredentials userCredentials)
        {
            try
            {
                var loginTask = client.LoginAsync(userCredentials?.Loginname, userCredentials?.Password);
                loginTask.Wait();

                return true;
            }
            catch (AggregateException ae)
            {
                ae.Flatten().InnerExceptions
                    .ToList()
                    .Select(e => e.GetInnermostException())
                    .ToList()
                    .ForEach(e => _logger.ErrorException(e.Message, e));

                return false;
            }
        }

        public void Logout(IClient client)
        {
            try
            {
                var logoutTask = client.LogoutAsync();
                logoutTask.Wait();
            }
            catch (AggregateException ae)
            {
                ae.Flatten().InnerExceptions
                    .ToList()
                    .Select(e => e.GetInnermostException())
                    .ToList()
                    .ForEach(e => _logger.ErrorException(e.Message, e));
            }
        }
    }
}
