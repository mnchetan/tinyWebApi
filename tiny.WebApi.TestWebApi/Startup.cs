// <copyright file="Startup.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Implements the startup class.
// </summary>
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.IO;
using tiny.WebApi.Configurations;
namespace tiny.WebApi.TestWebApi
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
        [DebuggerStepThrough]
        [DebuggerHidden]
        public void ConfigureServices(IServiceCollection services) =>
            _ = services.AddTinyWebApi(new TinyWebApiConfigurations()
            {
#pragma warning disable CS8601 // Possible null reference assignment.
                ConfigurationDirectoryPath = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName,
#pragma warning restore CS8601 // Possible null reference assignment.
                ConnectionStringJSONFileNameWithoutExtension = "connectionstring",
                MailerJSONFileNameWithoutExtension = "mailer",
                QueriesJSONFileNameWithoutExtension = "queries",
                RunAsUserJSONFileNameWithoutExtension = "users",
                DatabaseSpecifications = new(),
                MailerSpecifications = new(),
                QuerySpecifications = new(),
                RunAsUserSpecifications = new()
            });
        /// <summary>   Configures. </summary>
        /// <param name="app">  The application. </param>
        /// <param name="env">  The environment. </param>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </remarks>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) => app.UseTinyWebApi(env);
    }
}
