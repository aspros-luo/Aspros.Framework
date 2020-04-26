using System.Threading.Tasks;
using Consul;

namespace Framework.Infrastructure.ServiceDiscovery
{
    public interface IServiceRegister
    {
        IConsulClient ConsulClient { get; set; }
        string ServiceAddress { get; }
        int ServicePort { get; }
        AgentServiceCheck AgentServiceCheck { get; set; }

        Task<bool> RegisterServiceAsync(string serviceName, string serviceId);
        Task<bool> RegisterServiceAsync(string serviceName);
    }
}
