using System;
using System.Diagnostics.CodeAnalysis;

namespace DomainLayer.Managers.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public abstract class MovieServiceBusinessBaseException : MovieServiceBaseException
    {
        protected MovieServiceBusinessBaseException() { }
        protected MovieServiceBusinessBaseException(string message) : base(message) { }
        protected MovieServiceBusinessBaseException(string message, Exception inner) : base(message, inner) { }
        protected MovieServiceBusinessBaseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
