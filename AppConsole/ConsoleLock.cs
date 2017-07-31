namespace AppConsole
{
    public abstract class ConsoleLock
    {
        protected static readonly object ConsoleLockObject = new object();
    }
}
