using DomainLayer.Managers.Exceptions;

namespace DomainLayer.Managers.ConfigurationProviders
{
    internal abstract class ConfigurationProviderBase
    {
        public string GetImdbServiceBaseUrl()
        {
            var imdbServiceBaseUrl = RetrieveConfigurationSettingValueThrowIfMissing("ImdbServiceBaseUrl");
            return imdbServiceBaseUrl.EndsWith("/", System.StringComparison.OrdinalIgnoreCase) ? imdbServiceBaseUrl : imdbServiceBaseUrl + "/";
        }

        public string GetDbConnectionString()
        {
            return RetrieveConfigurationSettingValueThrowIfMissing("DbConnectionString");
        }

        private string RetrieveConfigurationSettingValueThrowIfMissing(string key)
        {
            var valueAsRetrieved = RetrieveConfigurationSettingValue(key);
            if (valueAsRetrieved == null)
            {
                throw new ConfigurationSettingMissingException($"The Configuration Setting with Key: {key}, is Missing from the Configuration file");
            }
            else if (valueAsRetrieved.Length == 0)
            {
                throw new ConfigurationSettingValueEmptyException($"The Configuration Setting with Key: {key}, Exists but its value is Empty");
            }
            else if (IsWhiteSpaces(valueAsRetrieved))
            {
                throw new ConfigurationSettingValueEmptyException($"The Configuration Setting with Key: {key}, Exists but its value is White spaces.");
            }

            return valueAsRetrieved;
        }

        private static bool IsWhiteSpaces(string valueAsRetrieved)
        {
            foreach (var chr in valueAsRetrieved)
            {
                if (!char.IsWhiteSpace(chr))
                {
                    return false;
                }
            }

            return true;
        }

        protected abstract string RetrieveConfigurationSettingValue(string key);
    }
}
