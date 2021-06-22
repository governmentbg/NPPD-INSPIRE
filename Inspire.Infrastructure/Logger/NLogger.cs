namespace Inspire.Infrastructure.Logger
{
    using System;

    using Inspire.Infrastructure.Utilities;
    using Inspire.Utilities.Exception;
    using Inspire.Utilities.Extensions;

    using NLog;

    using ILogger = Inspire.Core.Infrastructure.Logger.ILogger;
    using LogEventInfo = Inspire.Core.Infrastructure.Logger.LogEventInfo;
    using LogLevel = Inspire.Core.Infrastructure.Logger.LogLevel;

    public class NLogger : ILogger
    {
        private readonly Logger logger;

        public NLogger()
        {
            logger = LogManager.GetLogger(ReflectionUtils.CallClassName);
            LogLevel |= logger.IsTraceEnabled ? LogLevel.Trace : LogLevel.None;
            LogLevel |= logger.IsDebugEnabled ? LogLevel.Debug : LogLevel.None;
            LogLevel |= logger.IsErrorEnabled ? LogLevel.Error : LogLevel.None;
            LogLevel |= logger.IsWarnEnabled ? LogLevel.Warn : LogLevel.None;
            LogLevel |= logger.IsInfoEnabled ? LogLevel.Info : LogLevel.None;
        }

        public LogLevel LogLevel { get; }

        public void Error(Exception e)
        {
            LogException(e);
        }

        public void Error(string e)
        {
            logger.Error(e);
        }

        public void Info(string e)
        {
            logger.Info(e);
        }

        public void Warning(string e)
        {
            logger.Warn(e);
        }

        public void Trace(string e)
        {
            logger.Trace(e);
        }

        public void Debug(string e)
        {
            logger.Debug(e);
        }

        public void Log(LogEventInfo e)
        {
            var logData = new NLog.LogEventInfo
            {
                Message = e.Message,
                Exception = e.Exception,
                Level = ToNLogLogLevel(e.Level)
            };

            if (e.Properties.IsNotNullOrEmpty())
            {
                foreach (var key in e.Properties.Keys)
                {
                    logData.Properties.Add(key, e.Properties[key]);
                }
            }

            logger.Log(logData);
        }

        public bool IsEnabled(LogLevel level)
        {
            return logger.IsEnabled(ToNLogLogLevel(level));
        }

        private static NLog.LogLevel ToNLogLogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    {
                        return NLog.LogLevel.Trace;
                    }

                case LogLevel.Debug:
                    {
                        return NLog.LogLevel.Debug;
                    }

                case LogLevel.Info:
                    {
                        return NLog.LogLevel.Info;
                    }

                case LogLevel.Warn:
                    {
                        return NLog.LogLevel.Warn;
                    }

                case LogLevel.Error:
                    {
                        return NLog.LogLevel.Error;
                    }

                case LogLevel.Fatal:
                    {
                        return NLog.LogLevel.Fatal;
                    }

                default:
                    {
                        throw new ArgumentOutOfRangeException("level");
                    }
            }
        }

        private void LogException(Exception exception)
        {
            if (exception == null || exception.GetType() == typeof(UserException))
            {
                return;
            }

            logger.Error(exception);
            LogException(exception.InnerException);
        }
    }
}