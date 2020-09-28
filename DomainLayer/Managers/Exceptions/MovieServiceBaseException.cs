using System;
using System.Diagnostics.CodeAnalysis;

namespace DomainLayer.Managers.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public abstract class MovieServiceBaseException : Exception
    {
        public abstract string Reason { get; }
        protected MovieServiceBaseException() { }
        protected MovieServiceBaseException(string message) : base(message) { }
        protected MovieServiceBaseException(string message, Exception inner) : base(message, inner) { }
        protected MovieServiceBaseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
