// <copyright file="Startup.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Implements the extensions class.
// </summary>
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using tiny.Logger;
using tinyWebApi.Common;
using tinyWebApi.Common.DataObjects;
using tinyWebApi.Common.DBContext;
using tinyWebApi.Common.Exceptions;
using tinyWebApi.Common.IDBContext;
using tinyWebApi.Common.IRepository;
using tinyWebApi.Configurations;
namespace tinyWebApi.WebApi.Configurations
{
    /// <summary>
    ///     A extensions.
    /// </summary>
    [DebuggerStepThrough]
    public static class Extensions
    {
        /// <summary>
        ///     Boot strap repositories.
        /// </summary>
        /// <param name="services"> The services. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        private static void BootStrapRepositories(IServiceCollection services)
        {
            _ = services.AddScoped<ITinyWebApiRepository, TinyWebApiRepository>();
            _ = services.AddScoped<IDBContextSql, DBContextSql>();
            _ = services.AddScoped<IDBContextOracle, DBContextOracle>();
            _ = services.AddScoped<ITinyWebApiConfigurations, TinyWebApiConfigurations>();
        }
        /// <summary>
        /// Check if this process is running on Windows in an in process instance in IIS
        /// </summary>
        /// <returns>True if Windows and in an in process instance on IIS, false otherwise</returns>
        [DebuggerHidden]
        public static bool IsRunningInProcessIIS => Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().ProcessName).Contains("w3wp", StringComparison.OrdinalIgnoreCase) || Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().ProcessName).Contains("iisexpress", StringComparison.OrdinalIgnoreCase) && RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        /// <summary>
        /// Adds the tiny web api.
        /// ITinyWebApiConfigurations needs to be passed on as mandatory parameter.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="webHostEnvironment">The web host environment.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="tinyWebApiConfigurations">The tiny web api configurations.</param>
        /// <returns>An IServiceCollection.</returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static IServiceCollection AddTinyWebApi(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, ILoggerFactory loggerFactory, ITinyWebApiConfigurations tinyWebApiConfigurations)
        {
            if (string.IsNullOrEmpty(tinyWebApiConfigurations.ConfigurationDirectoryPath))
            {
                tinyWebApiConfigurations.ConfigurationDirectoryPath = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
            }
            (Global.Configuration, Global.WebHostingEnvironment, Global.TinyWebApiConfigurations, Global.LoggerFactory) = (configuration, webHostEnvironment, tinyWebApiConfigurations, loggerFactory);
            if (!((!string.IsNullOrEmpty(tinyWebApiConfigurations.QueriesJSONFileNameWithoutExtension) && File.Exists(tinyWebApiConfigurations.QueriesJSONFilePath)) || (tinyWebApiConfigurations.QuerySpecifications is not null && tinyWebApiConfigurations.QuerySpecifications.Count > 0)))
            {
                throw new Exception("QueriesJSONFileNameWithoutExtension needs to be specified and queries.<environment>.json should be present in the ContenRootPath or near executing assembly or QuerySpecifications need to be filled.");
            }
            BootStrapRepositories(services);
            services.AddMvc(options =>
            {
                options.Filters.Add<OperationCancelledExceptionFilter>();
            });
            services.AddCors(options =>
            {
                var corsHost = !string.IsNullOrEmpty(Global.Configuration?.GetSection("AllowedCorsHosts").Value) ? Global.Configuration?.GetSection("AllowedCorsHosts").Value : "http://*:*;https://*:*";
                options.AddPolicy("CorsPolicy", builder => builder.WithOrigins(corsHost.Split(';')).SetIsOriginAllowedToAllowWildcardSubdomains().AllowCredentials().AllowAnyHeader().WithMethods("GET", "PUT", "POST", "DELETE"));
            });
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = new[] { "text/plain", "text/css", "application/javascript", "text/html", "application/xml", "text/xml", "application/json", "text/json", "imagesvg+xml" };
            });
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
                options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
            services.AddTinyLogger(new LoggerOption("logs",Level:LogLevel.Trace)).AddTinyLoggerConsole();
            return services;
        }
        /// <summary>
        /// Uses the tiny configuration.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="webHostEnvironment"></param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void UseTinyWebApi(this IApplicationBuilder app, IWebHostEnvironment webHostEnvironment)
        {
            _ = webHostEnvironment.IsDevelopment() ? app.UseDeveloperExceptionPage() : app.UseHsts();
            app.UseMiddleware<CustomExceptionMiddleware>().UseHttpsRedirection().UseResponseCompression().UseRouting().UseCors().UseEndpoints(e => { e.MapControllers(); });
        }
    }
}
