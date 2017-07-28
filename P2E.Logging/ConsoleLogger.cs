using System;
using System.Globalization;
using System.Text;
using System.Threading;
using MediaBrowser.Model.Logging;
using P2E.Interfaces.Logging;

namespace P2E.Logging
{
    public class ConsoleLogger : IAppLogger
    {
        private static readonly SemaphoreSlim SemSlim = new SemaphoreSlim(1, 1);

        public void Info(string message, params object[] paramList)
        {
            WriteToStdOut($"INFO: {message}", paramList);
        }

        public void Error(string message, params object[] paramList)
        {
            WriteToStdErr($"ERROR: {message}", paramList);
        }

        public void Warn(string message, params object[] paramList)
        {
            WriteToStdOut($"WARN: {message}", paramList);
        }

        public void Debug(string message, params object[] paramList)
        {
            WriteToStdOut($"DEBUG: {message}", paramList);
        }

        public void Fatal(string message, params object[] paramList)
        {
            WriteToStdErr($"FATAL: {message}", paramList);
        }

        public void FatalException(string message, Exception exception, params object[] paramList)
        {
            WriteToStdErr($"FATALEXCEPTION: {message}", paramList);
        }

        public void ErrorException(string message, Exception exception, params object[] paramList)
        {
            WriteToStdErr($"ERROREXCEPTION: {message}", paramList);
        }

        public void LogMultiline(string message, LogSeverity severity, StringBuilder additionalContent)
        {
            throw new NotImplementedException();
        }

        public void Log(LogSeverity severity, string message, params object[] paramList)
        {
            throw new NotImplementedException();
        }

        private void WriteToStdErr(string message, params object[] paramList)
        {
            SemSlim.Wait();
            try
            {
                if (paramList.Length == 0)
                {
                    Console.Error.WriteLine($"{GetTimestamp()} {message}");
                    return;
                }

                Console.Error.WriteLine($"{GetTimestamp()} {message}\n{{0}}", paramList);
            }
            finally
            {
                SemSlim.Release();
            }
        }

        private void WriteToStdOut(string message, params object[] paramList)
        {
            SemSlim.Wait();
            try
            {
                if (paramList.Length == 0)
                {
                    Console.Out.WriteLine($"{GetTimestamp()} {message}");
                    return;
                }

                Console.Out.WriteLine($"{GetTimestamp()} {message}\n{{0}}", paramList);
            }
            finally
            {
                SemSlim.Release();
            }
        }

        private string GetTimestamp()
        {
            return DateTime.UtcNow.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }
    }
}
