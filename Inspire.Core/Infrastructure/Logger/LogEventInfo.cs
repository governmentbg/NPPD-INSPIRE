namespace Inspire.Core.Infrastructure.Logger
{
    using System;
    using System.Collections.Generic;

    public class LogEventInfo
    {
        public string Message { get; set; }

        public Exception Exception { get; set; }

        public LogLevel Level { get; set; }

        public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();
    }
}