using System;
using System.Text;
using MediaBrowser.Model.Logging;

namespace P2E.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void Info(string message, params object[] paramList)
        {
            Console.Out.WriteLine($"INFO: {message}", paramList);
        }

        public void Error(string message, params object[] paramList)
        {
            Console.Error.WriteLine($"ERROR: {message}", paramList);
        }

        public void Warn(string message, params object[] paramList)
        {
            Console.Out.WriteLine($"WARN: {message}", paramList);
        }

        public void Debug(string message, params object[] paramList)
        {
            Console.Out.WriteLine($"DEBUG: {message}", paramList);
        }

        public void Fatal(string message, params object[] paramList)
        {
            Console.Error.WriteLine($"FATAL: {message}", paramList);
        }

        public void FatalException(string message, Exception exception, params object[] paramList)
        {
            Console.Error.WriteLine($"FATALEXCEPTION: {message}", paramList);
        }

        public void ErrorException(string message, Exception exception, params object[] paramList)
        {
            Console.Error.WriteLine($"ERROREXCEPTION: {message}", paramList);
        }

        public void LogMultiline(string message, LogSeverity severity, StringBuilder additionalContent)
        {
            throw new NotImplementedException();
        }

        public void Log(LogSeverity severity, string message, params object[] paramList)
        {
            throw new NotImplementedException();
        }
    }
}
