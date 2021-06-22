namespace Inspire.Core.Infrastructure.Logger
{
    using System;

    public interface ILogger
    {
        LogLevel LogLevel { get; }

        void Error(Exception e);

        void Error(string e);

        void Info(string e);

        void Warning(string e);

        void Trace(string e);

        void Debug(string e);

        void Log(LogEventInfo e);

        bool IsEnabled(LogLevel level);
    }
}