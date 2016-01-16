using System;
using System.Runtime.Serialization;

namespace YALV.Core.Exceptions
{
    public class NotValidValueException : Exception
    {
        public NotValidValueException() { }

        public NotValidValueException(string message) : base(message) { }

        public NotValidValueException(string message, Exception inner) : base(message, inner) { }

        protected NotValidValueException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}