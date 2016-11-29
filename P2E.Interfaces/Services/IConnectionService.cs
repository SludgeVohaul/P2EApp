﻿using System.Threading.Tasks;
using P2E.Interfaces.DataObjects;

namespace P2E.Interfaces.Services
{
    public interface IConnectionService : IService
    {
        Task LoginAsync(IClient client, IUserCredentials userCredentials);

        Task LogoutAsync(IClient client);
    }
}
