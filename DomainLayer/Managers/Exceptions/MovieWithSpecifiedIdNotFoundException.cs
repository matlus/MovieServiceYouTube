using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DomainLayer.Managers.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public sealed class MovieWithSpecifiedIdNotFoundException : MovieServiceNotFoundBaseException
    {
        public MovieWithSpecifiedIdNotFoundException() { }
        public MovieWithSpecifiedIdNotFoundException(string message) : base(message) { }
        public MovieWithSpecifiedIdNotFoundException(string message, Exception inner) : base(message, inner) { }
        private MovieWithSpecifiedIdNotFoundException(
          SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override string Reason => "Movie with Specified Id Not Found";
    }
}
