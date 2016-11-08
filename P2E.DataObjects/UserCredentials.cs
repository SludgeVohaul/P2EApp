using P2E.Interfaces.DataObjects;

namespace P2E.DataObjects
{
    public class UserCredentials : IUserCredentials
    {
        public string Loginname { get; set; }
        public string Password { get; set; }
    }
}