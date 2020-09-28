using System;
using System.Diagnostics.CodeAnalysis;

namespace DomainLayer.Managers.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public sealed class ConfigurationSettingMissingException : MovieServiceTechnicalBaseException
    {
        public override string Reason => "Configuration Setting Missing";
        public ConfigurationSettingMissingException() { }
        public ConfigurationSettingMissingException(string message) : base(message) { }
        public ConfigurationSettingMissingException(string message, Exception inner) : base(message, inner) { }
        private ConfigurationSettingMissingException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
