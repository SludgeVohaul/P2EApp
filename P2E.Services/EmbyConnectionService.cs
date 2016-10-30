using System;
using MediaBrowser.Model.Logging;
using P2E.Extensions.Exception;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.Services;

namespace P2E.Services
{
    public class EmbyConnectionService : IEmbyConnectionService
    {
        private readonly ILogger _logger;

        public EmbyConnectionService(ILogger logger)
        {
            _logger = logger;
        }

        public bool TryLogin(IEmbyClient embyClient, IUserCredentialsService userCredentialsService)
        {
            try
            {
                if (userCredentialsService.HasUserCredentials == false) userCredentialsService.GetUserCredentials();

                var authTask = embyClient.AuthenticateUserAsync(userCredentialsService.Loginname, userCredentialsService.Password);
                // TODO - should do something with the auth. result....
                authTask.Wait();

                return true;
            }
            catch (AggregateException ae)
            {
                foreach (var innerException in ae.GetInnerExceptions())
                {
                    if (innerException is AggregateException) continue;
                    _logger.Error(innerException.Message);
                }

                return false;                
            }
        }

        public void Logout(IEmbyClient embyClient)
        {
            var logoutTask = embyClient.Logout();
            logoutTask.Wait();
        }
    }
}