﻿using P2E.Interfaces.DataObjects;

namespace P2E.Interfaces.Services
{
    public interface IConnectionService : IService
    {
        bool TryLogin(IClient embyClient);
        bool TryLogin(IClient embyClient, IUserCredentials userCredentials);
        void Logout(IClient embyClient);
    }
}
