using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Framework.Infrastructure.Consul.Configuration
{
   public class ConsulConfigurationSource:IConfigurationSource
   {
       private readonly IConfigurationRegister _configurationRegister;

       public ConsulConfigurationSource(IConfigurationRegister configurationRegister)
       {
           _configurationRegister = configurationRegister;
       }

       public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
           return new ConsulConfigurationProvider(_configurationRegister);
        }

    }
}
