using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Framework.Infrastructure.Consul.Configuration
{
    public interface IConfigurationRegister
    {
        bool GetKeyValue(string key,out string value);

        Task<bool> SetKeyValueAsync(string key, string value);

        IEnumerable<string> AllKeys { get; }

        void AddWatchOnEntireConfig(Action callback);

        Task AddUpdatingPathAsync(string keyPath);

        void UpdateKeyParser(IKeyParser parser);
    }
}
