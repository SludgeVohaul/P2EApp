using System;
using System.Text;
using P2E.Interfaces.DataObjects;
using P2E.Interfaces.Factories;
using P2E.Interfaces.Services;

namespace P2E.Services
{
    public class UserCredentialsService : IUserCredentialsService
    {
        private readonly IUserCredentialsFactory _userCredentialsFactory;

        public UserCredentialsService(IUserCredentialsFactory userCredentialsFactory)
        {
            _userCredentialsFactory = userCredentialsFactory;
        }

        public IUserCredentials PromptForUserCredentials(IConnectionInformation connectionInformation)
        {
            var userCredentials = _userCredentialsFactory.CreateUserCredentials();

            Console.Out.Write($"{connectionInformation.IpAddress} username: ", connectionInformation.IpAddress);
            userCredentials.Loginname = Console.ReadLine();
            Console.Out.Write($"{userCredentials.Loginname}@{connectionInformation.IpAddress}'s password: ");
            userCredentials.Password = GetPassword();

            return userCredentials;
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