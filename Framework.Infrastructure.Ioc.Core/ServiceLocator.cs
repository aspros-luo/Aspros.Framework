using System;

namespace Framework.Infrastructure.Ioc.Core
{
    public static class ServiceLocator
    {
        public static IServiceProvider Instance { get; set; }
    }
}
