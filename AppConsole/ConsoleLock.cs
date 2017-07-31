using P2E.Interfaces.AppConsole;

namespace AppConsole
{
    public class ConsoleLock : IConsoleLock
    {
        public object LockObject { get; } = new object();
    }
}
