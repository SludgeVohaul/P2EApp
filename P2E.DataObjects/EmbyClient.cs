﻿using Emby.ApiInteraction;
using Emby.ApiInteraction.Cryptography;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Logging;
using P2E.Interfaces.DataObjects;
using P2E.Interfaces.DataObjects.Emby;

namespace P2E.DataObjects
{
    public class EmbyClient : ApiClient, IEmbyClient
    {
        public EmbyClient(ILogger logger, IDevice device, ICryptographyProvider cryptographyProvider, IConnectionInformation connectionInformation, IApplicationInformation applicationInformation)
            : base(logger, connectionInformation.ServerUrl, applicationInformation.Name, device, applicationInformation.Version, cryptographyProvider)
        {
        }
    }
}