using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Framework.Consul;

namespace Framework.Infrastructure.Consul.ServiceDiscovery
{
    public static class ServiceRegistryExtensions
    {
        public static async Task<string> GetLocalIpAddress(string hostName)
        {
            var host = await Dns.GetHostEntryAsync(hostName);
            foreach (var ip in host.AddressList.OrderBy(ip => ip.ToString()))
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
                {
                    return ip.ToString();
                }
            }
            return string.Empty;
        }

        public static string GetLocalIpAddress()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var network in networkInterfaces)
            {
                if (network.OperationalStatus != OperationalStatus.Up)
                    continue;

                var properties = network.GetIPProperties();

                if (properties.GatewayAddresses.Count == 0)
                    continue;

                foreach (var address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;

                    if (IPAddress.IsLoopback(address.Address))
                        continue;

                    //                    if (!address.IsDnsEligible)
                    //                    {
                    //                        if (mostSuitableIp == null)
                    //                            mostSuitableIp = address;
                    //                        continue;
                    //                    }
                    //
                    //                    // The best IP is the IP got from DHCP server
                    //                    if (address.PrefixOrigin != PrefixOrigin.Dhcp)
                    //                    {
                    //                        if (mostSuitableIp == null || !mostSuitableIp.IsDnsEligible)
                    //                            mostSuitableIp = address;
                    //                        continue;
                    //                    }

                    return address.Address.ToString();
                }
            }

            return "";
        }

        public static int GetNextAvailablePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            int port;
            listener.Start();
            port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            listener.Server.Dispose();
            return port;
        }

        public static IServiceRegister AddHttpHealthCheck(this IServiceRegister serviceRegister, string path, int intervalInSeconds, int deregisterIfCriticalAfterMinutes)
        {
            serviceRegister.AgentServiceCheck = BuildHttpHealthCheck(serviceRegister.ServiceAddress, serviceRegister.ServicePort, path, intervalInSeconds, deregisterIfCriticalAfterMinutes);
            return serviceRegister;
        }


  

        public static IServiceRegister SetConsul(this IServiceRegister serviceRegister, ConsulClient consulClient)
        {
            serviceRegister.ConsulClient = consulClient;
            return serviceRegister;
        }

      

        internal static AgentCheckRegistration BuildHttpHealthCheck(string address, int port, string path, int interval , int deregisterIfCriticalAfterMinutes)
        {
            var healthCheck = new AgentCheckRegistration()
            {
                HTTP = $"{address}:{port}/{path}",
                Interval = TimeSpan.FromSeconds(interval),
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(deregisterIfCriticalAfterMinutes)

            };
            if (!healthCheck.HTTP.StartsWith("HTTP", StringComparison.OrdinalIgnoreCase))
            {
                healthCheck.HTTP = "http://" + healthCheck.HTTP;
            }
            return healthCheck;
        }
    }
}
