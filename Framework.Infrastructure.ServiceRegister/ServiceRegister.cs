using System.Net;
using System.Threading.Tasks;
using Consul;

namespace Framework.Infrastructure.ServiceDiscovery
{
    public class ServiceRegister : IServiceRegister
    {
        private const string FabioTagPrefix = "urlprefix-";

        public IConsulClient ConsulClient { get; set; }

        public string ServiceAddress { get; set; }

        public int ServicePort { get; }

        public AgentServiceCheck AgentServiceCheck { get; set; }

        public ServiceRegister() { }

        public ServiceRegister(IPAddress serviceAddress) : this(serviceAddress, ServiceRegistryExtensions.GetNextAvailablePort())
        {

        }

        public ServiceRegister(IPAddress serviceAddress, int servicePort)
        {
            ServiceAddress = serviceAddress.ToString();
            ServicePort = servicePort;
        }

        public async Task<bool> RegisterServiceAsync(string serviceName, string serviceId)
        {
            var agentServiceRegistration = new AgentServiceRegistration
            {
                Name = serviceName,
                ID = serviceId,
                Address = ServiceAddress,
                Port = ServicePort,
                EnableTagOverride = false,
                Checks = new[] { AgentServiceCheck },
                Tags = new[] { $"{FabioTagPrefix}/{serviceName}"
                }
            };

            var response = await ConsulClient.Agent.ServiceRegister(agentServiceRegistration);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> RegisterServiceAsync(string serviceName)
        {
            return await RegisterServiceAsync(serviceName, $"{ServiceAddress}:{serviceName}");
        }
    }
}
