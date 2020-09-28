using DomainLayer.Managers.Exceptions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DomainLayer.Managers.Services.ImdbService.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public sealed class ImdbServiceNotFoundException : MovieServiceTechnicalBaseException
    {
        public override string Reason => "Imdb Service Reported - Not Found";
        public ImdbServiceNotFoundException() { }
        public ImdbServiceNotFoundException(string message) : base(message) { }
        public ImdbServiceNotFoundException(string message, Exception inner) : base(message, inner) { }
        private ImdbServiceNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
