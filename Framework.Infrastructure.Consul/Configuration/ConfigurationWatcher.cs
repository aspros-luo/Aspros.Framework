using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Infrastructure.Consul.Configuration
{
    public class ConfigurationWatcher
    {
        public string CurrentValue { get; set; }
        public Action<string> CallBack { get; set; }
        public Action CallbackAllKeys { get; set; }
        public string KeyToWatch { get; set; }
    }
}
