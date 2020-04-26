using Framework.Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Infrastructure.Consul.Configuration
{
    public class ConfigurationRegister : IConfigurationRegister
    {
        private const char ConsulPathChar = '/';
        private const char CorePath = ':';
        private const string ConsulPath = "/";

        private readonly IConsulClient _consulClient;
        private readonly IKVEndpoint _kvEndpoint;
        private readonly List<Dictionary<string,string>> _configKeys= new List<Dictionary<string, string>>();
        private readonly List<ConfigurationWatcher> _configWatchers = new List<ConfigurationWatcher>();
        private IKeyParser _parser = new JsonKeyValueParser();
        public ConfigurationRegister() : this(new ConsulClient()) { }

        public ConfigurationRegister(IConsulClient consulClient)
        {
            _consulClient = consulClient;
            _kvEndpoint = _consulClient.KV;
        }

        public bool GetKeyValue(string key, out string value)
        {
            lock (_configKeys)
            {
                for (var i = _configKeys.Count - 1; i >= 0; i--)
                {
                    if (_configKeys[i].TryGetValue(key, out value)) return true;
                }
                value = null;
                return false;
            }
        }

        public async Task<bool> SetKeyValueAsync(string key, string value)
        {
            key = HttpUtils.StripFrontAndBackSlashes(key);
            var kv = new KVPair(key) {Value = Encoding.UTF8.GetBytes(value)};
            var response = await _kvEndpoint.Put(kv);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public IEnumerable<string> AllKeys => _configKeys.SelectMany(x => x.Keys);

        public void UpdateKeyParser(IKeyParser parser)
        {
            _parser = parser;
        }

        public void AddWatchOnEntireConfig(Action callback)
        {
            throw new NotImplementedException();
        }

        public async Task AddUpdatingPathAsync(string keyPath)
        {
            if (!keyPath.EndsWith(ConsulPath)) keyPath = keyPath + ConsulPath;
            var initialDictionary = await AddInitialKeyPathAsync(keyPath);
            if (initialDictionary == -1)
            {
                var newDictionary = new Dictionary<string, string>();
                initialDictionary = AddNewDictionaryToList(newDictionary);
            }
            //We got values so lets start watching but we aren't waiting for this we will let it run
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            WatchingLoop(initialDictionary, keyPath);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        }

        private async Task<int> AddInitialKeyPathAsync(string keyPath)
        {
            var response = await _kvEndpoint.List(keyPath);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return -1;
            }

            var dictionary = BuildDictionaryAsync(keyPath, response.Response);
            return AddNewDictionaryToList(dictionary);
        }

        private Dictionary<string, string> BuildDictionaryAsync(string keyPath, KVPair[] keys)
        {
            var parsedKeys = keys.SelectMany(k => _parser.Parse(k));

            var dictionary = parsedKeys.ToDictionary(
                kv => kv.Key.Substring(keyPath.Length).Replace(ConsulPathChar, CorePath),
                kv => kv.IsDerivedKey ? kv.ValueFromBase64() : kv.Value == null ? null : kv.ValueFromBase64(),
                StringComparer.OrdinalIgnoreCase);
            return dictionary;
        }

        private int AddNewDictionaryToList(Dictionary<string, string> dictionaryToAdd)
        {
            lock (_configKeys)
            {
                _configKeys.Add(dictionaryToAdd);
                return _configKeys.Count - 1;
            }
        }


        private async void WatchingLoop(int indexOfDictionary, string keyPath)
        {
            var consulIndex = "0";
            while (true)
            {
                var queryOptions = new QueryOptions
                {
                    WaitTime = TimeSpan.FromSeconds(300),
                    WaitIndex = ulong.Parse(consulIndex)
                };
                var response = await _kvEndpoint.List(keyPath, queryOptions);

                consulIndex = response.LastIndex.ToString();
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    //There is some error we need to do something 
                    continue;
                }

                var dictionary = BuildDictionaryAsync(keyPath, response.Response);
                UpdateDictionaryInList(indexOfDictionary, dictionary);
                FireWatchers();
            }
        }

        private void UpdateDictionaryInList(int index, Dictionary<string, string> dictionaryToAdd)
        {
            lock (_configKeys)
            {
                _configKeys[index] = dictionaryToAdd;
            }
        }
        private void FireWatchers()
        {
            lock (_configWatchers)
            {
                foreach (var watch in _configWatchers)
                {
                    if (watch.KeyToWatch == null)
                    {
                        Task.Run(watch.CallbackAllKeys);
                    }
                    else
                    {
                        string newValue;
                        GetKeyValue(watch.KeyToWatch, out newValue);
                        if (StringComparer.OrdinalIgnoreCase.Compare(watch.CurrentValue, newValue) != 0)
                        {
                            watch.CurrentValue = newValue;
                            Task.Run(() => watch.CallBack(newValue));
                        }
                    }
                }
            }
        }
    }
}
