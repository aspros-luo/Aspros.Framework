using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Infrastructure.Consul.Configuration
{
    public class ConsulConfigurationProvider: ConfigurationProvider
    {
        private readonly IConfigurationRegister _configurationRegister;

        public ConsulConfigurationProvider(IConfigurationRegister configurationRegister)
        {
            _configurationRegister = configurationRegister;
        }

        public override bool TryGet(string key, out string value)
        {
            return _configurationRegister.GetKeyValue(key,out value);
        }

        public override void Set(string key, string value)
        {
            _configurationRegister.SetKeyValueAsync(key, value).Wait();
        }

        public override void Load()
        {
            OnReload();
        }

        public override IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
        {
            var prefix = parentPath == null ? string.Empty : parentPath + ConfigurationPath.KeyDelimiter;
            //Need to override this as we are not setting base Data, so expose all keys on registry
            return _configurationRegister.AllKeys
                .Where(key => key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .Select(key => Segment(key, prefix.Length))
                .Concat(earlierKeys)
                .OrderBy(key => key, ConfigurationKeyComparer.Instance);
        }

        private static string Segment(string key, int prefixLength)
        {
            var indexOf = key.IndexOf(ConfigurationPath.KeyDelimiter, prefixLength, StringComparison.OrdinalIgnoreCase);
            return indexOf < 0 ? key.Substring(prefixLength) : key.Substring(prefixLength, indexOf - prefixLength);
        }
    }
}
