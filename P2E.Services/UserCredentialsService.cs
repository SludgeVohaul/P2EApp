using System;
using System.Text;
using P2E.Interfaces.Services;

namespace P2E.Services
{
    public class UserCredentialsService : IUserCredentialsService
    {
        // Syncs console output/input between instance so that only one 
        // instance can request user credentials at any time.
        private static readonly object LockObject = new object();

        public string Loginname { get; private set; }
        public string Password { get; private set; }

        public void GetUserCredentials()
        {
            // Each instance can request credentials only once.
            if (HasUserCredentials) return;
            lock (LockObject)
            {
                if (HasUserCredentials) return;

                Console.Out.Write("Username: ");
                Loginname = Console.ReadLine();
                Console.Out.Write("Password: ");
                Password = GetPassword();
            }
        }

        public bool HasUserCredentials => Loginname != null && Password != null;

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