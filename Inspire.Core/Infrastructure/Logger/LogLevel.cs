namespace Inspire.Core.Infrastructure.Logger
{
    using System;

    [Flags]
    public enum LogLevel
    {
        None = 1 << 0,
        Trace = 1 << 1,
        Debug = 1 << 2,
        Info = 1 << 3,
        Warn = 1 << 4,
        Error = 1 << 5,
        Fatal = 1 << 6
    }
}