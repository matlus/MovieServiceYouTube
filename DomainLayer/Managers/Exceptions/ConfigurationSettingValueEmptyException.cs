using System;
using System.Diagnostics.CodeAnalysis;

namespace DomainLayer.Managers.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public sealed class ConfigurationSettingValueEmptyException : MovieServiceTechnicalBaseException
    {
        public override string Reason => "Configuration Setting Value Empty";
        public ConfigurationSettingValueEmptyException() { }
        public ConfigurationSettingValueEmptyException(string message) : base(message) { }
        public ConfigurationSettingValueEmptyException(string message, Exception inner) : base(message, inner) { }
        private ConfigurationSettingValueEmptyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
