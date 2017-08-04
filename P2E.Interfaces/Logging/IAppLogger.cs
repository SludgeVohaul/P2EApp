using MediaBrowser.Model.Logging;

namespace P2E.Interfaces.Logging
{
    public interface IAppLogger : ILogger
    {
        void Log(Severity severity, string message, params object[] paramList);
    }

    // TODO - this is ugly.
    public enum Severity
    {
        Info,
        Error,
        Warn,
        Debug,
        Fatal,
        FatalException,
        ErrorException
    }
}