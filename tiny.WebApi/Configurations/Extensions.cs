// <copyright file="Extensions.cs" company="tiny">
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
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using tiny.Logger;
using tiny.WebApi.DataObjects;
using tiny.WebApi.DBContext;
using tiny.WebApi.Exceptions;
using tiny.WebApi.IDBContext;
using tiny.WebApi.IRepository;
namespace tiny.WebApi.Configurations
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
        /// Itiny.WebApiConfigurations needs to be passed on as mandatory parameter.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="tinyWebApiConfigurations">The tiny web api configurations.</param>
        /// <returns>An IServiceCollection.</returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static IServiceCollection AddTinyWebApi(this IServiceCollection services, ITinyWebApiConfigurations tinyWebApiConfigurations)
        {
            var (configuration, webHostEnvironment) = (services.BuildServiceProvider().GetRequiredService<IConfiguration>(), services.BuildServiceProvider().GetRequiredService<IWebHostEnvironment>());
            if (string.IsNullOrEmpty(tinyWebApiConfigurations.ConfigurationDirectoryPath))
            {
#pragma warning disable CS8601 // Possible null reference assignment.
                tinyWebApiConfigurations.ConfigurationDirectoryPath = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
#pragma warning restore CS8601 // Possible null reference assignment.
            }
            (Global.Configuration, Global.WebHostingEnvironment, Global.TinyWebApiConfigurations) = (configuration, webHostEnvironment, tinyWebApiConfigurations);
            if (!((!string.IsNullOrEmpty(tinyWebApiConfigurations.QueriesJSONFileNameWithoutExtension) && File.Exists(tinyWebApiConfigurations.QueriesJSONFilePath)) || (tinyWebApiConfigurations.QuerySpecifications is not null && tinyWebApiConfigurations.QuerySpecifications.Count > 0)))
            {
                throw new Exception("QueriesJSONFileNameWithoutExtension needs to be specified and queries.<environment>.json should be present in the ContenRootPath or near executing assembly or QuerySpecifications need to be filled.");
            }
            BootStrapRepositories(services);
            services.AddMvc(options =>
            {
                options.Filters.Add<OperationCancelledExceptionFilter>();
            });
            _ = services.AddCors(options =>
              {
                  var corsHost = !string.IsNullOrEmpty(Global.Configuration?.GetSection("AllowedCorsHosts").Value) ? Global.Configuration?.GetSection("AllowedCorsHosts").Value : "http://*:*;https://*:*";
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                  options.AddPolicy("CorsPolicy", builder => builder.WithOrigins(corsHost.Split(';')).SetIsOriginAllowedToAllowWildcardSubdomains().AllowCredentials().AllowAnyHeader().WithMethods("GET", "PUT", "POST", "DELETE"));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
            services.AddTinyLogger().AddTinyLoggerConsole();
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
