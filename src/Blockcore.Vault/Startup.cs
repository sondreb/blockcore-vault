using Blockcore.Vault.Authentication;
using Blockcore.Vault.Managers;
using Blockcore.Vault.Services;
using Blockcore.Vault.Settings;
using Blockcore.Vault.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Blockcore.Vault
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SyncSettings>(Configuration.GetSection("Sync"));
            services.Configure<ApiSettings>(Configuration.GetSection("ApiSettings"));

            services.AddScoped<IDatabaseConnectionFactory, DatabaseConnectionFactory>();
            services.AddScoped<DatabaseRepository>();
            // services.AddScoped<DataStore>();
            services.AddScoped<IMoney, Money>();
            services.AddResponseCompression();
            services.AddMemoryCache();
            //services.AddHostedService<SyncServer>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DefaultScheme;
                options.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
            }).AddApiKeySupport(options => { });

            services.AddSingleton<IGetApiKeyQuery, AppSettingsGetApiKeyQuery>();

            // Create a new instance of SyncManager pr. external vault.
            services.AddTransient<SyncManager>();
            services.AddHostedService<SyncWorker>();

            services.AddSingleton<IUriService>(o =>
            {
                var accessor = o.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
                return new UriService(uri);
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                string assemblyVersion = typeof(Startup).Assembly.GetName().Version.ToString();

                c.AddSecurityDefinition(ApiKeyConstants.HeaderName, new OpenApiSecurityScheme
                {
                    Description = $"API key needed to access the endpoints. {ApiKeyConstants.HeaderName}: YOUR_KEY",
                    In = ParameterLocation.Header,
                    Name = ApiKeyConstants.HeaderName,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Name = ApiKeyConstants.HeaderName,
                                    Type = SecuritySchemeType.ApiKey,
                                    In = ParameterLocation.Header,
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = ApiKeyConstants.HeaderName
                                    },
                                 },
                                 new string[] {}
                             }
                        });

                c.SwaggerDoc("vault", new OpenApiInfo { Title = "Blockcore.Vault", Version = assemblyVersion });

                // integrate xml comments
                if (File.Exists(XmlCommentsFilePath))
                {
                    c.IncludeXmlComments(XmlCommentsFilePath);
                }

                c.EnableAnnotations();
            });

            services.AddCors(o => o.AddPolicy("VaultPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable Cors
            app.UseCors("VaultPolicy");

            app.UseResponseCompression();

            app.UseRouting();

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "docs/{documentName}/openapi.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "docs";
                c.SwaggerEndpoint("/docs/vault/openapi.json", "Blockcore Vault API");
            });

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        static string XmlCommentsFilePath
        {
            get
            {
                string basePath = PlatformServices.Default.Application.ApplicationBasePath;
                string fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }
    }
}
