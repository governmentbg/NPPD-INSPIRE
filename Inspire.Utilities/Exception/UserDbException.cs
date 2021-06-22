namespace Inspire.Utilities.Exception
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [Serializable]
    public class UserDbException : UserLogException
    {
        public UserDbException()
        {
        }

        public UserDbException(string message)
            : base(message)
        {
        }

        public UserDbException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public UserDbException(string message, string code, string lastDbCall)
            : base(message)
        {
            Code = code;
            LastDbCall = lastDbCall;
        }

        public UserDbException(string message, Exception innerException, string code, string lastDbCall)
            : base(message, innerException)
        {
            Code = code;
            LastDbCall = lastDbCall;
        }

        protected UserDbException(SerializationInfo info, StreamingContext context, string code, string lastDbCall)
            : base(info, context)
        {
            Code = code;
            LastDbCall = lastDbCall;
        }

        public string Code { get; }

        public string LastDbCall { get; }

        public override bool Equals(object obj)
        {
            return obj is UserDbException exception &&
                   Code == exception.Code &&
                   LastDbCall == exception.LastDbCall;
        }

        public override int GetHashCode()
        {
            var hashCode = 775258979;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Code);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(LastDbCall);
            return hashCode;
        }

        public override string ToString()
        {
            return $"{GetType().Name}: {Message}\r\nCode: {Code}\r\nLastDbCall: {LastDbCall}\r\n{StackTrace}";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}