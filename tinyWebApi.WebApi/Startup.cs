// <copyright file="Startup.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Implements the startup class.
// </summary>
using System.Diagnostics;
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
        [DebuggerStepThrough]
        [DebuggerHidden]
        public void ConfigureServices(IServiceCollection services) =>
            _ = services.AddTinyWebApi(new TinyWebApiConfigurations()
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
        /// <summary>   Configures. </summary>
        /// <param name="app">  The application. </param>
        /// <param name="env">  The environment. </param>
        /// <remarks>
        /// This method gets called by the runtime.Use this method to configure the HTTP request pipeline.
        /// </remarks>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) => app.UseTinyWebApi(env);
    }
}
