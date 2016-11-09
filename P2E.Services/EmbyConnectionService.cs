using System;
using System.Linq;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Users;
using P2E.ExtensionMethods;
using P2E.Interfaces.DataObjects;
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

        public AuthenticationResult Login(IEmbyClient embyClient, IUserCredentials userCredentials)
        {
            try
            {
                var authTask = embyClient.AuthenticateUserAsync(userCredentials.Loginname, userCredentials.Password);
                authTask.Wait();

                return authTask.Result;
            }
            catch (AggregateException ae)
            {
                ae.Flatten().InnerExceptions
                    .ToList()
                    .Select(e => e.GetInnermostException())
                    .ToList()
                    .ForEach(e => _logger.ErrorException(e.Message, e));

                return null;
            }
        }

        public void Logout(IEmbyClient embyClient)
        {
            try
            {
                var logoutTask = embyClient.Logout();
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