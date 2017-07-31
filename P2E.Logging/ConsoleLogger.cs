using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using AppConsole;
using MediaBrowser.Model.Logging;
using P2E.Interfaces.Logging;

namespace P2E.Logging
{
    public class ConsoleLogger : ConsoleLock, IAppLogger
    {
        private enum Severity
        {
            Info,
            Error,
            Warn,
            Debug,
            Fatal,
            FatalException,
            ErrorException
        }

        private ConsoleColor _consoleForegroundColor = Console.ForegroundColor;

        public void Info(string message, params object[] paramList)
        {
            WriteToStdOut(Severity.Info, $"INFO: {message}", paramList);
        }

        public void Error(string message, params object[] paramList)
        {
            WriteToStdErr(Severity.Error, $"ERROR: {message}", paramList);
        }

        public void Warn(string message, params object[] paramList)
        {
            WriteToStdOut(Severity.Warn, $"WARN: {message}", paramList);
        }

        public void Debug(string message, params object[] paramList)
        {
            WriteToStdOut(Severity.Debug, $"DEBUG: {message}", paramList);
        }

        public void Fatal(string message, params object[] paramList)
        {
            WriteToStdErr(Severity.Fatal, $"FATAL: {message}", paramList);
        }

        public void FatalException(string message, Exception exception, params object[] paramList)
        {
            WriteToStdErr(Severity.FatalException, $"FATALEXCEPTION: {message}", paramList);
        }

        public void ErrorException(string message, Exception exception, params object[] paramList)
        {
            WriteToStdErr(Severity.ErrorException, $"ERROREXCEPTION: {message}", paramList);
        }

        public void LogMultiline(string message, LogSeverity severity, StringBuilder additionalContent)
        {
            throw new NotImplementedException();
        }

        public void Log(LogSeverity severity, string message, params object[] paramList)
        {
            throw new NotImplementedException();
        }

        private void WriteToStdErr(Severity severity, string message, params object[] paramList)
        {
            try
            {
                lock (ConsoleLockObject)
                {
                    Console.ForegroundColor = GetSeverityForegroundColor(severity);
                    if (paramList.Length == 0)
                    {
                        Console.Error.WriteLine($"{GetTimestamp()} {message}");
                        return;
                    }

                    Console.Error.WriteLine($"{GetTimestamp()} {message}\n{{0}}", paramList);
                    Console.ForegroundColor = _consoleForegroundColor;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = _consoleForegroundColor;
                Console.WriteLine($"Logging failed: {ex.Message}");
            }
        }

        private void WriteToStdOut(Severity severity, string message, params object[] paramList)
        {
            try
            {
                lock (ConsoleLockObject)
                {
                    Console.ForegroundColor = GetSeverityForegroundColor(severity);
                    if (paramList.Length == 0)
                    {
                        Console.Out.WriteLine($"{GetTimestamp()} {message}");
                        return;
                    }

                    Console.Out.WriteLine($"{GetTimestamp()} {message}\n{{0}}", paramList);
                    Console.ForegroundColor = _consoleForegroundColor;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = _consoleForegroundColor;
                Console.WriteLine($"Logging failed: {ex.Message}");
            }
        }

        private string GetTimestamp()
        {
            return DateTime.UtcNow.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }

        private ConsoleColor GetSeverityForegroundColor(Severity severity)
        {
            ConsoleColor foregroundColor;

            switch (severity)
            {
                case Severity.Debug:
                    foregroundColor = ConsoleColor.DarkCyan;
                    break;
                case Severity.Error:
                    foregroundColor = ConsoleColor.Red;
                    break;
                case Severity.ErrorException:
                    foregroundColor = ConsoleColor.Red;
                    break;
                case Severity.Fatal:
                    foregroundColor = ConsoleColor.Red;
                    break;
                case Severity.FatalException:
                    foregroundColor = ConsoleColor.Red;
                    break;
                case Severity.Info:
                    foregroundColor = _consoleForegroundColor;
                    break;
                case Severity.Warn:
                    foregroundColor = ConsoleColor.DarkRed;
                    break;
                default:
                    throw new InvalidEnumArgumentException($"Unknown severity type: '{severity}'.");
            }

            return foregroundColor;
        }
    }
}
