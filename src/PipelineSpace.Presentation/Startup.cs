using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PipelineSpace.Infra.CrossCutting.IoC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using IdentityServer4.AccessTokenValidation;
using PipelineSpace.Infra.Options;
using Microsoft.Extensions.Options;
using PipelineSpace.Presentation.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using PipelineSpace.Infra.CrossCutting.Logging.Filters;
using PipelineSpace.Infra.CrossCutting.Logging;
using PipelineSpace.Infra.CrossCutting.Logging.Middlewares;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using System.Linq;
using PipelineSpace.Presentation.Hubs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Threading.Tasks;
using PipelineSpace.Infra.CrossCutting.Identity;

namespace PipelineSpace.Presentation
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(environment.ContentRootPath)
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
              .AddEnvironmentVariables();
            Configuration = builder.AddInMemoryCollection(configuration.AsEnumerable()).Build();
            Environment = environment;
        }

        public IHostingEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorization(o =>
            {
                var authorizationPolicy = new AuthorizationPolicyBuilder(PipelineSpaceSchemeContants.DefaultSchema)
                    .RequireAuthenticatedUser()
                    .Build();
                o.AddPolicy("RequireLoggedOnUsers", authorizationPolicy);
            });
            
            services.AddMvc().AddJsonOptions(
                options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            //services.AddSpaceLogging(Configuration);

            //Options
            services.Configure<Infra.Options.AuthenticationOptions>(Configuration.GetSection("Authentication"));
            services.Configure<ApplicationOptions>(Configuration.GetSection("Application"));
            services.Configure<VSTSServiceOptions>(Configuration.GetSection("VSTS"));
            services.Configure<FakeAccountServiceOptions>(Configuration.GetSection("FakeVSTS"));
            services.Configure<NotificationOptions>(Configuration.GetSection("Notification"));
            services.Configure<Infra.Options.SwaggerOptions>(Configuration.GetSection("Swagger"));

            var sp = services.BuildServiceProvider();
            var authenticationOptions = sp.GetService<IOptions<Infra.Options.AuthenticationOptions>>();

            var swaggerOptions = sp.GetService<IOptions<Infra.Options.SwaggerOptions>>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(swaggerOptions.Value.Version, new Info
                {
                    Version = swaggerOptions.Value.Version,
                    Title = swaggerOptions.Value.Title,
                    Description = swaggerOptions.Value.Description,
                    TermsOfService = swaggerOptions.Value.TermsOfService,
                    Contact = new Contact { Name = swaggerOptions.Value.ContactName, Email = swaggerOptions.Value.ContactEmail }
                });

                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Service.xml");

                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                c.DescribeAllEnumsAsStrings();
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

            services.AddCors();

            services.AddAuthentication()
            .AddPolicyScheme(PipelineSpaceSchemeContants.DefaultSchema, PipelineSpaceSchemeContants.DefaultSchema, options =>
            {
                options.ForwardDefaultSelector = c =>
                {
                    if (c.Request.Path.StartsWithSegments("/pipelineHub"))
                    {
                        return JwtBearerDefaults.AuthenticationScheme;
                    }
                    if (c.Request.Path.StartsWithSegments("/internalapi"))
                    {
                        return JwtBearerDefaults.AuthenticationScheme;
                    }
                    if (c.Request.Path.StartsWithSegments("/api-core"))
                    {
                        return JwtBearerDefaults.AuthenticationScheme;
                    }
                    if (c.Request.Path.StartsWithSegments("/organizationprovidercredential/externallogins") ||
                        c.Request.Path.StartsWithSegments("/organizationprovidercredential/linklogin"))
                    {
                        return JwtBearerDefaults.AuthenticationScheme;
                    }
                    return PipelineSpaceSchemeContants.ApplicationSchema;
                };
            })
            .AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
             {
                 options.Authority = authenticationOptions.Value.Authority;
                 options.RequireHttpsMetadata = false;

                 options.ApiName = authenticationOptions.Value.ApiName;
                 options.NameClaimType = authenticationOptions.Value.NameClaimType;
                 options.RoleClaimType = authenticationOptions.Value.RoleClaimType;
                 options.TokenRetriever = PipelineSpaceTokenRetreiver.FromHeaderAndQueryString;

             });

            DependencyInjectorBootStrapper.RegisterServices(services, Environment, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IOptions<Infra.Options.SwaggerOptions> swaggerOptions)
        {
            app.UseSpaceLogging("/Home/Error");

            //TODO: Only for adminwebclient
            app.UseCors(o => o.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());

            app.UseHttpsRedirection();
            
            app.UseStaticFiles();

            app.UseXfo(o => o.SameOrigin());

            app.UseIdentityServer();
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = swaggerOptions.Value.Title;
                c.RoutePrefix = "docs";
                c.SwaggerEndpoint(swaggerOptions.Value.Endpoint, swaggerOptions.Value.Title);
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSignalR(routes =>
            {
                routes.MapHub<PipelineSpaceHub>("/pipelineHub");
            });

        }
    }
}
