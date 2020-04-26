using Framework.Consul;
using System.Collections.Generic;

namespace Framework.Infrastructure.Consul.Configuration
{
    public interface IKeyParser
    {
        IEnumerable<KVPair> Parse(KVPair key);
    }
}
