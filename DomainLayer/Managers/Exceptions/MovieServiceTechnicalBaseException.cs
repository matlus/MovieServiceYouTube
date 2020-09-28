using System;
using System.Diagnostics.CodeAnalysis;

namespace DomainLayer.Managers.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public abstract class MovieServiceTechnicalBaseException : MovieServiceBaseException
    {
        protected MovieServiceTechnicalBaseException() { }
        protected MovieServiceTechnicalBaseException(string message) : base(message) { }
        protected MovieServiceTechnicalBaseException(string message, Exception inner) : base(message, inner) { }
        protected MovieServiceTechnicalBaseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
