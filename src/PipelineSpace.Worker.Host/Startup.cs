using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DomainModels = PipelineSpace.Domain.Models;
using PipelineSpace.Infra.Options;
using PipelineSpace.Worker.Events;
using PipelineSpace.Worker.Handlers;
using PipelineSpace.Worker.Handlers.Core;
using PipelineSpace.Worker.Handlers.Services;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Polly.Registry;
using Polly;
using Polly.Extensions.Http;
using Microsoft.Azure.WebJobs.Hosting;
using PipelineSpace.Worker.Host;
using Microsoft.Azure.WebJobs;
using PipelineSpace.Worker.Host.DI.Config;

[assembly: WebJobsStartup(typeof(Startup), "Web Jobs Extension")]
namespace PipelineSpace.Worker.Host
{
    internal class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder) => builder.AddDependencyInjection(ConfigureServices);

        private void ConfigureServices(IServiceCollection services)
        {
            string functionDirectory = Environment.GetEnvironmentVariable("FUNCTION_DIRECTORY");
            if (string.IsNullOrEmpty(functionDirectory))
                functionDirectory = Directory.GetCurrentDirectory();

            IConfigurationRoot Configuration = new ConfigurationBuilder()
                .SetBasePath(functionDirectory)
                .AddJsonFile("local.settings.json", optional: true)
                .AddJsonFile($"local.settings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("logging.json", optional: false)
                .AddJsonFile($"logging.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            //Basic Services
            services.AddTransient<IPasswordGeneratorService, PasswordGeneratorService>();
            services.AddTransient<IPipelineSpaceManagerService, PipelineSpaceManagerService>();
            services.AddTransient<IHttpClientWrapperService, HttpClientWrapperService>();

            //Polly Policies
            IPolicyRegistry<string> registry = services.AddPolicyRegistry();

            IAsyncPolicy<HttpResponseMessage> httpRetryPolicy =
                Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .RetryAsync(3);

            registry.Add("SimpleHttpRetryPolicy", httpRetryPolicy);

            Random jitterer = new Random();

            IAsyncPolicy<HttpResponseMessage> httWaitAndpRetryPolicy =
                Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                                        + TimeSpan.FromMilliseconds(jitterer.Next(0, 100)));

            registry.Add("SimpleWaitAndRetryPolicy", httWaitAndpRetryPolicy);

            IAsyncPolicy<HttpResponseMessage> noOpPolicy = Policy.NoOpAsync()
                .AsAsyncPolicy<HttpResponseMessage>();

            registry.Add("NoOpPolicy", noOpPolicy);

            services.AddHttpClient("RemoteServerFromWorker", client =>
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddPolicyHandlerFromRegistry((policyRegistry, httpRequestMessage) =>
            {
                if (httpRequestMessage.Method == HttpMethod.Get)
                {
                    return policyRegistry.Get<IAsyncPolicy<HttpResponseMessage>>("SimpleHttpRetryPolicy");
                }
                else if (httpRequestMessage.Method == HttpMethod.Post)
                {
                    return policyRegistry.Get<IAsyncPolicy<HttpResponseMessage>>("NoOpPolicy");
                }
                else
                {
                    return policyRegistry.Get<IAsyncPolicy<HttpResponseMessage>>("SimpleWaitAndRetryPolicy");
                }
            })
            .AddPolicyHandler((httpRequestMessage) =>
            {
                return HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
            });

            //Handler Services
            services.AddTransient<ProjectVSTSHandlerService>();
            services.AddTransient<ProjectBitbucketHandlerService>();
            services.AddTransient<ProjectGitHubHandlerService>();

            services.AddTransient<Func<DomainModels.ConfigurationManagementService, IProjectHandlerService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case DomainModels.ConfigurationManagementService.VSTS:
                        return serviceProvider.GetService<ProjectVSTSHandlerService>();
                    case DomainModels.ConfigurationManagementService.Bitbucket:
                        return serviceProvider.GetService<ProjectBitbucketHandlerService>();
                    case DomainModels.ConfigurationManagementService.GitHub:
                        return serviceProvider.GetService<ProjectGitHubHandlerService>();
                    default:
                        throw new KeyNotFoundException();
                }
            });

            services.AddTransient<ProjectServiceVSTSHandlerService>();
            services.AddTransient<ProjectServiceBitbucketHandlerService>();
            services.AddTransient<ProjectServiceGitHubHandlerService>();

            services.AddTransient<Func<DomainModels.ConfigurationManagementService, IProjectServiceHandlerService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case DomainModels.ConfigurationManagementService.VSTS:
                        return serviceProvider.GetService<ProjectServiceVSTSHandlerService>();
                    case DomainModels.ConfigurationManagementService.Bitbucket:
                        return serviceProvider.GetService<ProjectServiceBitbucketHandlerService>();
                    case DomainModels.ConfigurationManagementService.GitHub:
                        return serviceProvider.GetService<ProjectServiceGitHubHandlerService>();
                    default:
                        throw new KeyNotFoundException();
                }
            });

            services.AddTransient<ProjectFeatureVSTSHandlerService>();
            services.AddTransient<ProjectFeatureBitbucketHandlerService>();
            services.AddTransient<ProjectFeatureGitHubHandlerService>();

            services.AddTransient<Func<DomainModels.ConfigurationManagementService, IProjectFeatureHandlerService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case DomainModels.ConfigurationManagementService.VSTS:
                        return serviceProvider.GetService<ProjectFeatureVSTSHandlerService>();
                    case DomainModels.ConfigurationManagementService.Bitbucket:
                        return serviceProvider.GetService<ProjectFeatureBitbucketHandlerService>();
                    case DomainModels.ConfigurationManagementService.GitHub:
                        return serviceProvider.GetService<ProjectFeatureGitHubHandlerService>();
                    default:
                        throw new KeyNotFoundException();
                }
            });

            services.AddTransient<CPSAWSService>();
            services.AddTransient<CPSAzureService>();

            services.AddTransient<Func<DomainModels.CloudProviderService, ICPSService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case DomainModels.CloudProviderService.AWS:
                        return serviceProvider.GetService<CPSAWSService>();
                    case DomainModels.CloudProviderService.Azure:
                        return serviceProvider.GetService<CPSAzureService>();
                    default:
                        throw new KeyNotFoundException();
                }
            });

            services.AddTransient<IEventHandler<ProjectCreatedEvent>, ProjectCreatedHandler>();
            services.AddTransient<IEventHandler<ProjectImportedEvent>, ProjectImportedHandler>();

            services.AddTransient<IEventHandler<ProjectServiceCreatedEvent>, ProjectServiceCreatedHandler>();
            services.AddTransient<IEventHandler<ProjectServiceImportedEvent>, ProjectServiceImportedHandler>();
            services.AddTransient<IEventHandler<ProjectFeatureCreatedEvent>, ProjectFeatureCreatedHandler>();
            services.AddTransient<IEventHandler<ProjectFeatureServiceCreatedEvent>, ProjectFeatureServiceCreatedHandler>();

            services.AddTransient<IEventHandler<ProjectServiceBuildQueuedEvent>, ProjectServiceBuildQueuedHandler>();
            services.AddTransient<IEventHandler<ProjectFeatureServiceBuildQueuedEvent>, ProjectFeatureServiceBuildQueuedHandler>();

            services.AddTransient<IEventHandler<OrganizationDeletedEvent>, OrganizationDeletedHandler>();
            services.AddTransient<IEventHandler<ProjectDeletedEvent>, ProjectDeletedHandler>();
            services.AddTransient<IEventHandler<ProjectServiceDeletedEvent>, ProjectServiceDeletedHandler>();
            services.AddTransient<IEventHandler<ProjectFeatureDeletedEvent>, ProjectFeatureDeletedHandler>();
            services.AddTransient<IEventHandler<ProjectFeatureServiceDeletedEvent>, ProjectFeatureServiceDeletedHandler>();
            services.AddTransient<IEventHandler<ProjectFeatureCompletedEvent>, ProjectFeatureCompletedHandler>();

            services.AddTransient<IEventHandler<ProjectEnvironmentActivatedEvent>, ProjectEnvironmentActivatedHandler>();
            services.AddTransient<IEventHandler<ProjectEnvironmentCreatedEvent>, ProjectEnvironmentCreatedHandler>();
            services.AddTransient<IEventHandler<ProjectEnvironmentDeletedEvent>, ProjectEnvironmentDeletedHandler>();
            services.AddTransient<IEventHandler<ProjectEnvironmentInactivatedEvent>, ProjectEnvironmentInactivatedHandler>();

            services.AddTransient<IEventHandler<ProjectFeatureEnvironmentCreatedEvent>, ProjectFeatureEnvironmentCreatedHandler>();
            
            services.AddTransient<IEventHandler<OrganizationUserInvitedEvent>, OrganizationUserInvitedHandler>();
            services.AddTransient<IEventHandler<ProjectUserInvitedEvent>, ProjectUserInvitedHandler>();

            services.AddTransient<IEventHandler<ProjectServiceTemplateCreatedEvent>, ProjectServiceTemplateCreatedHandler>();

            services.AddTransient<IRealtimeService, RealtimeService>();
            

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")) && Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Equals("production", StringComparison.InvariantCultureIgnoreCase))
            {
                services.AddTransient<IEmailWorkerService, SendGridEmailWorkerService>();
            }
            else
            {
                services.AddTransient<IEmailWorkerService, PostmarkEmailWorkerService>();
            }

            services.Configure<ApplicationOptions>(options =>
            {
                options.Url = Configuration["Application:Url"];
                options.ClientId = Configuration["Application:ClientId"];
                options.ClientSecret = Configuration["Application:ClientSecret"];
                options.Scope = Configuration["Application:Scope"];
            });

            services.Configure<VSTSServiceOptions>(options =>
            {
                options.AccessId = Configuration["VSTS:AccessId"];
                options.AccessSecret = Configuration["VSTS:AccessSecret"];
                options.AccountId = Configuration["VSTS:AccountId"];
                options.ApiVersion = Configuration["VSTS:ApiVersion"];
            });

            services.Configure<FakeAccountServiceOptions>(options =>
            {
                options.AccessId = Configuration["FakeVSTS:AccessId"];
                options.AccessSecret = Configuration["FakeVSTS:AccessSecret"];
                options.AccountId = Configuration["FakeVSTS:AccountId"];
                options.ApiVersion = Configuration["FakeVSTS:ApiVersion"];
            });

            services.Configure<NotificationOptions>(options =>
            {
                options.SendGrid = new SendGridOptions();
                options.SendGrid.ApiKey = Configuration["Notification:SendGrid:ApiKey"];
                options.SendGrid.From = Configuration["Notification:SendGrid:From"];
            });
        }
    }
}
