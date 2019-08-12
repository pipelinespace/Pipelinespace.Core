using Microsoft.Extensions.DependencyInjection;
using System;

namespace PipelineSpace.Worker.Host.DI.Config
{
    internal class ServiceProviderBuilder : IServiceProviderBuilder
    {
        private readonly Action<IServiceCollection> _configureServices;

        public ServiceProviderBuilder(Action<IServiceCollection> configureServices) =>
            _configureServices = configureServices;

        public IServiceProvider Build()
        {
            var services = new ServiceCollection();
            _configureServices(services);
            return services.BuildServiceProvider();
        }
    }
}
