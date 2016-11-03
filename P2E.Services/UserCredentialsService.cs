using System;
using System.Text;
using P2E.Interfaces.DataObjects;
using P2E.Interfaces.Services;

namespace P2E.Services
{
    public class UserCredentialsService : IUserCredentialsService
    {
        private readonly IUserCredentials _userCredentials;
        public UserCredentialsService(IUserCredentials userCredentials)
        {
            _userCredentials = userCredentials;
        }

        public IUserCredentials GetUserCredentials(IConnectionInformation connectionInformation)
        {
            if (_userCredentials.HasCredentials) return _userCredentials;

            Console.Out.Write($"{connectionInformation.IpAddress} username: ", connectionInformation.IpAddress);
            _userCredentials.Loginname = Console.ReadLine();
            Console.Out.Write($"{_userCredentials.Loginname}@{connectionInformation.IpAddress}'s password: ");
            _userCredentials.Password = GetPassword();

            return _userCredentials;
        }

        private string GetPassword()
        {
            var password = new StringBuilder();
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password.Append(key.KeyChar);
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password.Remove(password.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                }
            }
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();

            return password.ToString();
        }
    }
}