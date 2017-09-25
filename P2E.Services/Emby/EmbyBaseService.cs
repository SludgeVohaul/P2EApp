using System;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Repositories.Emby;

namespace P2E.Services.Emby
{
    public abstract class EmbyBaseService
    {
        protected readonly IEmbyClient Client;
        protected readonly IAppLogger Logger;
        protected readonly IEmbyRepository Repository;

        protected EmbyBaseService(IAppLogger logger, IEmbyClient client, IEmbyRepository embyRepository)
        {
            Client = client;
            Logger = logger;
            Repository = embyRepository;
        }


        protected void LogException(Exception ex, string message)
        {
            Logger.Log(Severity.Error, message);
            Logger.Log(Severity.ErrorException, ex.Message);
            while (ex.InnerException != null)
            {
                Logger.Log(Severity.ErrorException, ex.InnerException.Message);
                ex = ex.InnerException;
            }
        }
    }
}