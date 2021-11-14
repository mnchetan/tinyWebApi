/// <copyright file="Startup.cs" company="tiny">
///     Copyright (c) 2021 tiny. All rights reserved.
/// </copyright>
/// <summary>
///     Implements the startup class.
/// </summary>
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using tinyWebApi.Configurations;
using tinyWebApi.WebApi.Configurations;
namespace tinyWebApi.WebApi
{
    /// <summary>
    ///     A startup.
    /// </summary>
    [DebuggerStepThrough]
    public class Startup
    {
        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"> The services. </param>
        public void ConfigureServices(IServiceCollection services)
        {
#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
            _ = services.AddTinyWebApi(services.BuildServiceProvider().GetRequiredService<IConfiguration>(), services.BuildServiceProvider().GetRequiredService<IWebHostEnvironment>(), services.BuildServiceProvider().GetRequiredService<ILoggerFactory>(), new TinyWebApiConfigurations()
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
            {
                ConfigurationDirectoryPath = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName,
                ConnectionStringJSONFileNameWithoutExtension = "connectionstring",
                MailerJSONFileNameWithoutExtension = "mailer",
                QueriesJSONFileNameWithoutExtension = "queries",
                RunAsUserJSONFileNameWithoutExtension = "users",
                DatabaseSpecifications = new(),
                MailerSpecifications = new(),
                QuerySpecifications = new(),
                RunAsUserSpecifications = new()
            });
        }
        /// <summary>
        ///     Configures.
        /// </summary>
        /// <param name="app"> The application. </param>
        /// <param name="env"> The environment. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        /// <summary>   Configures. </summary>
        /// <param name="app">  The application. </param>
        /// <param name="env">  The environment. </param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseTinyWebApi(env);
        }
    }
}
