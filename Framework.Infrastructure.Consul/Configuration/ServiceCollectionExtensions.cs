using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Framework.Infrastructure.Consul.Configuration
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection ConfigureReloadable<T>(
            this IServiceCollection self,
            IConfiguration configuration, 
            IConfigurationRegister registry,
            string sectionName)
            where T : class
        {
            var initialised = false;
            self.Configure<T>
            (config =>
            {
                Action bind = () =>
                {
                    var section = configuration.GetSection(sectionName);
                    section.Bind(config);
                };

                if (!initialised)
                {
                    registry.AddWatchOnEntireConfig(bind);
                    initialised = true;
                }

                bind();
            });

            return self;
        }
    }
}
