using Microsoft.Azure.WebJobs.Host.Config;
using PipelineSpace.Worker.Host.DI.Bindings;

namespace PipelineSpace.Worker.Host.DI.Config
{
    internal class InjectConfiguration : IExtensionConfigProvider
    {
        public readonly InjectBindingProvider _injectBindingProvider;

        public InjectConfiguration(InjectBindingProvider injectBindingProvider) =>
            _injectBindingProvider = injectBindingProvider;

        public void Initialize(ExtensionConfigContext context) => context
                .AddBindingRule<InjectAttribute>()
                .Bind(_injectBindingProvider);
    }
}

