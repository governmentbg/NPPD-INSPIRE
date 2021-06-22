namespace Inspire.Utilities.Exception
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class UserLogException : UserException
    {
        public UserLogException()
        {
        }

        public UserLogException(string message)
            : base(message)
        {
        }

        public UserLogException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected UserLogException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}