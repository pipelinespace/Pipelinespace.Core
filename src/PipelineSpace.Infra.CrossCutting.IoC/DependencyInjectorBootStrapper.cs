using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Services;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using DomainModels = PipelineSpace.Domain.Models;
using PipelineSpace.Infra.CrossCutting.Identity;
using PipelineSpace.Infra.Data.SqlServer.Contexts;
using PipelineSpace.Infra.Data.SqlServer.Repositories;
using PipelineSpace.Infra.Notification.Postmark;
using System;
using PipelineSpace.Infra.Data.ServiceAgent.Repositories;
using System.Collections.Generic;
using PipelineSpace.Infra.Data.ServiceAgent;
using PipelineSpace.Worker.Handlers.Core;
using PipelineSpace.Worker.Events;
using PipelineSpace.Worker.Handlers;
using PipelineSpace.Infra.Messaging.InMemory;
using System.Reflection;
using PipelineSpace.Application.Services.InternalServices.Interfaces;
using PipelineSpace.Application.Services.InternalServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using PipelineSpace.Worker.Handlers.Services;
using Microsoft.AspNetCore.Hosting;
using PipelineSpace.Infra.Messaging.Azure.ServiceBus;
using PipelineSpace.Application.Services.PublicServices.Interfaces;
using PipelineSpace.Application.Services.PublicServices;
using PipelineSpace.Infra.CrossCutting.Identity.TokenProviderServices.Interfaces;
using PipelineSpace.Infra.CrossCutting.Monitor;
using Polly.Registry;
using Polly;
using System.Net.Http;
using Polly.Extensions.Http;
using PipelineSpace.Infra.Notification.SendGrid;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;
using PipelineSpace.Infra.CrossCutting.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Azure.KeyVault;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace PipelineSpace.Infra.CrossCutting.IoC
{
    public class DependencyInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services, IHostingEnvironment environment, IConfiguration configuration)
        {
            //Data Protection
            var dataProtectionBuilder = services.AddDataProtection()
                                                .SetApplicationName("PipelineSpace")
                                                .SetDefaultKeyLifetime(TimeSpan.FromDays(30));
            if (!environment.IsDevelopment())
            {
                //var redis = ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection"));
                dataProtectionBuilder
                        .PersistKeysToAzureBlobStorage(
                        new CloudBlobContainer(new StorageUri(new Uri(configuration["DataProtection:Storage:ContainerAddress"])),
                                                new StorageCredentials(configuration["DataProtection:Storage:AccountName"], configuration["DataProtection:Storage:KeyValue"], configuration["DataProtection:Storage:KeyName"])), configuration["DataProtection:Storage:BlobName"])
                        //.PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys")
                        .ProtectKeysWithAzureKeyVault(
                                configuration["DataProtection:AzureKeyVault:KeyIdentifier"],
                                configuration["DataProtection:AzureKeyVault:ClientId"],
                                configuration["DataProtection:AzureKeyVault:ClientSecret"]);
            }

            // Featured Services
            if (environment.IsDevelopment())
            {
                services.AddSignalR();
            }
            else
            {
                services.AddSignalR().AddStackExchangeRedis(configuration.GetConnectionString("RedisConnection"), options => {
                    options.Configuration.ChannelPrefix = "PipelineSpace";
                });
            }
            
            // Application
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<IOrganizationQueryService, OrganizationQueryService>();

            services.AddScoped<IOrganizationCMSService, OrganizationCMSService>();
            services.AddScoped<IOrganizationCMSQueryService, OrganizationCMSQueryService>();

            services.AddScoped<IOrganizationCPSService, OrganizationCPSService>();
            services.AddScoped<IOrganizationCPSQueryService, OrganizationCPSQueryService>();

            services.AddScoped<IOrganizationUserService, OrganizationUserService>();
            services.AddScoped<IOrganizationUserQueryService, OrganizationUserQueryService>();

            services.AddScoped<IOrganizationUserInvitationService, OrganizationUserInvitationService>();
            services.AddScoped<IOrganizationUserInvitationQueryService, OrganizationUserInvitationQueryService>();

            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IProjectQueryService, ProjectQueryService>();

            services.AddScoped<IProjectServiceService, ProjectServiceService>();
            services.AddScoped<IProjectServiceQueryService, ProjectServiceQueryService>();

            services.AddScoped<IProjectFeatureService, ProjectFeatureService>();
            services.AddScoped<IProjectFeatureQueryService, ProjectFeatureQueryService>();

            services.AddScoped<IProjectFeatureServiceService, ProjectFeatureServiceService>();
            services.AddScoped<IProjectFeatureServiceQueryService, ProjectFeatureServiceQueryService>();

            services.AddScoped<IProjectEnvironmentService, ProjectEnvironmentService>();
            services.AddScoped<IProjectEnvironmentQueryService, ProjectEnvironmentQueryService>();

            services.AddScoped<IProjectTemplateQueryService, ProjectTemplateQueryService>();
            services.AddScoped<IProjectServiceTemplateQueryService, ProjectServiceTemplateQueryService>();

            services.AddScoped<IProjectActivityQueryService, ProjectActivityQueryService>();
            services.AddScoped<IProjectServiceActivityQueryService, ProjectServiceActivityQueryService>();
            services.AddScoped<IProjectFeatureServiceActivityQueryService, ProjectFeatureServiceActivityQueryService>();

            services.AddScoped<IProjectServiceDeliveryQueryService, ProjectServiceDeliveryQueryService>();
            services.AddScoped<IProjectFeatureServiceDeliveryQueryService, ProjectFeatureServiceDeliveryQueryService>();
            
            services.AddScoped<IInternalProjectService, InternalProjectService>();
            services.AddScoped<IInternalProjectServiceService, InternalProjectServiceService>();
            services.AddScoped<IInternalProjectFeatureService, InternalProjectFeatureService>();

            services.AddScoped<IInternalProjectActivityService, InternalProjectActivityService>();
            services.AddScoped<IInternalProjectServiceActivityService, InternalProjectServiceActivityService>();
            services.AddScoped<IInternalProjectFeatureServiceActivityService, InternalProjectFeatureServiceActivityService>();

            services.AddScoped<IInternalProjectServiceTemplateService, InternalProjectServiceTemplateService>();
            
            services.AddScoped<IPublicProjectServiceEventService, PublicProjectServiceEventService>();
            services.AddScoped<IPublicProjectFeatureServiceEventService, PublicProjectFeatureServiceEventService>();
            
            services.AddScoped<IProjectServiceEventService, ProjectServiceEventService>();
            services.AddScoped<IProjectFeatureServiceEventService, ProjectFeatureServiceEventService>();
            
            services.AddScoped<IProjectServiceEventQueryService, ProjectServiceEventQueryService>();
            services.AddScoped<IProjectFeatureServiceEventQueryService, ProjectFeatureServiceEventQueryService>();

            services.AddScoped<IProjectServiceEnvironmentService, ProjectServiceEnvironmentService>();
            services.AddScoped<IProjectServiceEnvironmentQueryService, ProjectServiceEnvironmentQueryService>();
            
            services.AddScoped<IProjectFeatureServiceEnvironmentService, ProjectFeatureServiceEnvironmentService>();
            services.AddScoped<IProjectFeatureServiceEnvironmentQueryService, ProjectFeatureServiceEnvironmentQueryService>();
            
            services.AddScoped<IProjectServiceCloudQueryService, ProjectServiceCloudQueryService>();
            services.AddScoped<IProjectFeatureServiceCloudQueryService, ProjectFeatureServiceCloudQueryService>();

            services.AddScoped<IProjectUserService, ProjectUserService>();
            services.AddScoped<IProjectUserQueryService, ProjectUserQueryService>();

            services.AddScoped<IProjectUserInvitationService, ProjectUserInvitationService>();
            services.AddScoped<IProjectUserInvitationQueryService, ProjectUserInvitationQueryService>();

            services.AddScoped<IDashboardQueryService, DashboardQueryService>();

            services.AddScoped<ICMSExternalQueryService, CMSExternalQueryService>();

            services.AddScoped<IOrganizationProjectServiceTemplateService, OrganizationProjectServiceTemplateService>();
            services.AddScoped<IOrganizationProjectServiceTemplateQueryService, OrganizationProjectServiceTemplateQueryService>();

            services.AddScoped<IProgrammingLanguageQueryService, ProgrammingLanguageQueryService>();

            services.AddScoped<IProjectCloudCredentialService, ProjectCloudCredentialService>();

            // Domain
            services.AddScoped<IDomainManagerService, DomainManagerService>();

            // Infra - Notification
            if (environment.IsProduction())
            {
                services.AddTransient<IEmailSender, SendGridEmailSender>();
                services.AddTransient<IEmailWorkerService, SendGridEmailWorkerService>();
            }
            else
            {
                services.AddTransient<IEmailSender, PostmarkEmailSender>();
                services.AddTransient<IEmailWorkerService, PostmarkEmailWorkerService>();
            }
            
            services.AddTransient<ISlugService, DefaultSlugService>();

            // Infra - Identity
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IExternalAuthTokenService, AspnetIdentityExternalTokenService>();

            // Infra - Monitor
            services.AddScoped<IDataProtectorService, DataProtectorService>();

            // Infra - Monitor
            services.AddScoped<IActivityMonitorService, ActivityMonitorService>();
            
            // Infra - Data - Service Agent
            services.AddTransient<IHttpProxyService, HttpProxyService>();

            services.AddTransient<ICMSPipelineService, CMSPipelineServiceAgentRepository>();

            services.AddTransient<CMSVSTSServiceAgentRepository>();
            services.AddTransient<CMSVSTSQueryServiceAgentRepository>();
            services.AddTransient<CMSVSTSCredentialService>();

            services.AddTransient<CMSBitBucketServiceAgentRepository>();
            services.AddTransient<CMSBitBucketQueryServiceAgentRepository>();
            services.AddTransient<CMSBitBucketCredentialService>();

            services.AddTransient<CMSGitHubServiceAgentRepository>();
            services.AddTransient<CMSGitHubQueryServiceAgentRepository>();
            services.AddTransient<CMSGitHubCredentialService>();

            services.AddTransient<CMSGitLabServiceAgentRepository>();
            services.AddTransient<CMSGitLabQueryServiceAgentRepository>();
            services.AddTransient<CMSGitLabCredentialService>();


            services.AddTransient<Func<DomainModels.ConfigurationManagementService, ICMSService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case DomainModels.ConfigurationManagementService.VSTS:
                        return serviceProvider.GetService<CMSVSTSServiceAgentRepository>();
                    case DomainModels.ConfigurationManagementService.Bitbucket:
                        return serviceProvider.GetService<CMSBitBucketServiceAgentRepository>();
                    case DomainModels.ConfigurationManagementService.GitHub:
                        return serviceProvider.GetService<CMSGitHubServiceAgentRepository>();
                    case DomainModels.ConfigurationManagementService.GitLab:
                        return serviceProvider.GetService<CMSGitLabServiceAgentRepository>();
                    default:
                        throw new KeyNotFoundException();
                }
            });

            services.AddTransient<Func<DomainModels.ConfigurationManagementService, ICMSQueryService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case DomainModels.ConfigurationManagementService.VSTS:
                        return serviceProvider.GetService<CMSVSTSQueryServiceAgentRepository>();
                    case DomainModels.ConfigurationManagementService.Bitbucket:
                        return serviceProvider.GetService<CMSBitBucketQueryServiceAgentRepository>();
                    case DomainModels.ConfigurationManagementService.GitHub:
                        return serviceProvider.GetService<CMSGitHubQueryServiceAgentRepository>();
                    case DomainModels.ConfigurationManagementService.GitLab:
                        return serviceProvider.GetService<CMSGitLabQueryServiceAgentRepository>();
                    default:
                        throw new KeyNotFoundException();
                }
            });

            services.AddTransient<Func<DomainModels.ConfigurationManagementService, ICMSCredentialService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case DomainModels.ConfigurationManagementService.VSTS:
                        return serviceProvider.GetService<CMSVSTSCredentialService>();
                    case DomainModels.ConfigurationManagementService.Bitbucket:
                        return serviceProvider.GetService<CMSBitBucketCredentialService>();
                    case DomainModels.ConfigurationManagementService.GitHub:
                        return serviceProvider.GetService<CMSGitHubCredentialService>();
                    case DomainModels.ConfigurationManagementService.GitLab:
                        return serviceProvider.GetService<CMSGitLabCredentialService>();
                    default:
                        throw new KeyNotFoundException();
                }
            });

            services.AddTransient<CPSAWSQueryServiceAgentRepository>();
            services.AddTransient<CPSAzureQueryServiceAgentRepository>();

            services.AddTransient<Func<DomainModels.CloudProviderService, ICPSQueryService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case DomainModels.CloudProviderService.AWS:
                        return serviceProvider.GetService<CPSAWSQueryServiceAgentRepository>();
                    case DomainModels.CloudProviderService.Azure:
                        return serviceProvider.GetService<CPSAzureQueryServiceAgentRepository>();
                    default:
                        throw new KeyNotFoundException();
                }
            });

            services.AddTransient<CPSAWSCredentialService>();
            services.AddTransient<CPSAzureCredentialService>();

            services.AddTransient<Func<DomainModels.CloudProviderService, ICPSCredentialService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case DomainModels.CloudProviderService.AWS:
                        return serviceProvider.GetService<CPSAWSCredentialService>();
                    case DomainModels.CloudProviderService.Azure:
                        return serviceProvider.GetService<CPSAzureCredentialService>();
                    default:
                        throw new KeyNotFoundException();
                }
            });

            //Handler Services
            services.AddTransient<ProjectVSTSHandlerService>();
            services.AddTransient<ProjectBitbucketHandlerService>();
            services.AddTransient<ProjectGitHubHandlerService>();
            services.AddTransient<ProjectGitLabHandlerService>();

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
                    case DomainModels.ConfigurationManagementService.GitLab:
                        return serviceProvider.GetService<ProjectGitLabHandlerService>();
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

            services.AddTransient<VSTSTokenProviderService>();
            services.AddTransient<GitHubTokenProviderService>();
            services.AddTransient<BitbucketTokenProviderService>();

            services.AddTransient<Func<string, ITokenProviderService>>(serviceProvider => key =>
            {
                switch (key.ToLower())
                {
                    case "vsts":
                        return serviceProvider.GetService<VSTSTokenProviderService>();
                    case "github":
                        return serviceProvider.GetService<GitHubTokenProviderService>();
                    case "bitbucket":
                        return serviceProvider.GetService<BitbucketTokenProviderService>();
                    default:
                        throw new KeyNotFoundException();
                }
            });

            // Infra - Data - SqlServer
            services.AddScoped<IUserRepository, UserSqlServerRepository>();
            services.AddScoped<IOrganizationRepository, OrganizationSqlServerRepository>();
            services.AddScoped<IOrganizationCMSRepository, OrganizationCMSSqlServerRepository>();
            services.AddScoped<IOrganizationCPSRepository, OrganizationCPSSqlServerRepository>();
            services.AddScoped<IOrganizationUserInvitationRepository, OrganizationUserInvitationSqlServerRepository>();
            services.AddScoped<IProjectRepository, ProjectSqlServerRepository>();
            services.AddScoped<IProjectServiceRepository, ProjectServiceSqlServerRepository>();
            services.AddScoped<IProjectFeatureRepository, ProjectFeatureSqlServerRepository>();
            services.AddScoped<IProjectTemplateRepository, ProjectTemplateSqlServerRepository>();
            services.AddScoped<IProjectServiceTemplateRepository, ProjectServiceTemplateSqlServerRepository>();

            services.AddScoped<IProjectActivityRepository, ProjectActivitySqlServerRepository>();
            services.AddScoped<IProjectServiceActivityRepository, ProjectServiceActivitySqlServerRepository>();
            services.AddScoped<IProjectFeatureServiceActivityRepository, ProjectFeatureServiceActivitySqlServerRepository>();
            services.AddScoped<IProjectUserInvitationRepository, ProjectUserInvitationSqlServerRepository>();

            services.AddScoped<IProgrammingLanguageRepository, ProgrammingLanguageSqlServerRepository>();

            services.AddScoped(x => new PipelineSpaceDbContextFactory().Create(configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<PipelineSpaceDbContext>(options =>
                options.UseLazyLoadingProxies()
                       .UseSqlServer(configuration.GetConnectionString("DefaultConnection"), 
                       sqlServerOptionsAction: sqlOptions =>
                       {
                           sqlOptions.EnableRetryOnFailure(
                           maxRetryCount: 10,
                           maxRetryDelay: TimeSpan.FromSeconds(30),
                           errorNumbersToAdd: null);
                       })
            );

            services.AddIdentity<DomainModels.User, IdentityRole>(o =>
            {
                // User settings
                o.User.RequireUniqueEmail = true;

                // Sign In settings
                //o.SignIn.RequireConfirmedEmail = true;
                //o.SignIn.RequireConfirmedPhoneNumber = true;

                // Password settings
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = true;
                o.Password.RequireNonAlphanumeric = true;
                o.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<PipelineSpaceDbContext>()
            .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddBitbucket(options =>
            {
                options.ClientId = configuration["Authentication:Bitbucket:ClientId"];
                options.ClientSecret = configuration["Authentication:Bitbucket:ClientSecret"];

                var scopes = configuration["Authentication:Bitbucket:Scopes"].Split(',');

                foreach (var item in scopes)
                {
                    if (!string.IsNullOrEmpty(item))
                        options.Scope.Add(item);
                }

                options.SaveTokens = true;
            })
            .AddGitHub(options =>
            {
                options.ClientId = configuration["Authentication:GitHub:ClientId"];
                options.ClientSecret = configuration["Authentication:GitHub:ClientSecret"];
                var scopes = configuration["Authentication:GitHub:Scopes"].Split(',');

                foreach (var item in scopes)
                {
                    if (!string.IsNullOrEmpty(item))
                        options.Scope.Add(item);
                }

                options.SaveTokens = true;
            })
            .AddVisualStudio("VSTS", "Visual Studio Team Services", options =>
            {
                options.ClientId = configuration["Authentication:VisualStudio:ClientId"];
                options.ClientSecret = configuration["Authentication:VisualStudio:ClientSecret"];
                options.CallbackPath = new PathString("/signin-vsts");
                options.TokenEndpoint = "https://app.vssps.visualstudio.com/oauth2/token";
                options.AuthorizationEndpoint = "https://app.vssps.visualstudio.com/oauth2/authorize";
                
                var scopes = configuration["Authentication:VisualStudio:Scopes"].Split(',');

                options.Scope.Clear();

                foreach (var item in scopes)
                {
                    if (!string.IsNullOrEmpty(item))
                        options.Scope.Add(item);
                }

                options.SaveTokens = true;
            });

            var migrationsAssembly = typeof(PipelineSpaceDbContext).GetTypeInfo().Assembly.GetName().Name;

            var identityServerBuilder = services.AddIdentityServer()
                        .AddAspNetIdentity<DomainModels.User>()
                        .AddConfigurationStore(options =>
                        {
                            options.ConfigureDbContext = builder =>
                                builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                                    sqlServerOptionsAction: sqlOptions =>
                                    {
                                        sqlOptions.MigrationsAssembly(migrationsAssembly);
                                        sqlOptions.EnableRetryOnFailure(
                                          maxRetryCount: 10,
                                          maxRetryDelay: TimeSpan.FromSeconds(30),
                                          errorNumbersToAdd: null);
                                    });
                        })
                        .AddOperationalStore(options =>
                        {
                            options.ConfigureDbContext = builder =>
                                builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                                    sqlServerOptionsAction: sqlOptions =>
                                    {
                                        sqlOptions.MigrationsAssembly(migrationsAssembly);
                                        sqlOptions.EnableRetryOnFailure(
                                          maxRetryCount: 10,
                                          maxRetryDelay: TimeSpan.FromSeconds(30),
                                          errorNumbersToAdd: null);
                                    });
                            // this enables automatic token cleanup. this is optional.
                            options.EnableTokenCleanup = true;
                            options.TokenCleanupInterval = 30;
                        });

            if (environment.IsDevelopment())
            {
                identityServerBuilder.AddDeveloperSigningCredential();
            }
            else
            {
                var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(async (authority, resource, scope) =>
                {
                    var authContext = new AuthenticationContext(authority);
                    ClientCredential clientCreds = new ClientCredential(configuration["DataProtection:AzureKeyVault:ClientId"], configuration["DataProtection:AzureKeyVault:ClientSecret"]);

                    AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCreds);

                    if (result == null)
                    {
                        throw new InvalidOperationException("Failed to obtain the JWT token");
                    }

                    return result.AccessToken;
                }));

                var pfxSecret = keyVaultClient.GetSecretAsync(configuration["DataProtection:AzureKeyVault:SecretIdentifier"]).Result;
                var pfxBytes = Convert.FromBase64String(pfxSecret.Value);
                var certificate = new X509Certificate2(pfxBytes);

                identityServerBuilder.AddSigningCredential(certificate);
            }
            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Infra - Messaging
            if(environment.IsDevelopment())
                services.AddTransient<IEventBusService, InMemoryEventBusService>();
            else
                services.AddTransient<IEventBusService, AzureEventBusService>();

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

            services.AddHttpClient("RemoteServerFromService", client =>
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
            .AddPolicyHandler((httpRequestMessage) => {
                return HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
            });

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
            .AddPolicyHandler((httpRequestMessage) => {
                return HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
            });

            services.AddTransient<Worker.Handlers.Services.Interfaces.IPipelineSpaceManagerService, PipelineSpaceManagerService>();
            services.AddTransient<IHttpClientWrapperService, HttpClientWrapperService>();
            services.AddTransient<IRealtimeService, RealtimeService>();
            services.AddTransient<IUserIdProvider, NameUserIdProvider>();
            services.AddTransient<ITemplateService, PipelineSpaceTemplateRepository>();
            
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

            services.AddTransient<IEventHandler<ProjectServiceTemplateCreatedEvent>, ProjectServiceTemplateCreatedHandler>();
        }
    }
}
