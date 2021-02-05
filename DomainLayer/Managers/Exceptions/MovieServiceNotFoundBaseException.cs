using System;
using System.Runtime.Serialization;

namespace DomainLayer.Managers.Exceptions
{
    [Serializable]
    public abstract class MovieServiceNotFoundBaseException : MovieServiceBusinessBaseException
    {
        protected MovieServiceNotFoundBaseException() { }
        protected MovieServiceNotFoundBaseException(string message) : base(message) { }
        protected MovieServiceNotFoundBaseException(string message, Exception inner) : base(message, inner) { }
        protected MovieServiceNotFoundBaseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
