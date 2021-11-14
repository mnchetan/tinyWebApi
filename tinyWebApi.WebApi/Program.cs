using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using static tiny.Extensions;
using tinyWebApi.WebApi;
/// <copyright file="Program.cs" company="tiny">
///     Copyright (c) 2021 tiny. All rights reserved.
/// </copyright>
/// <summary>
///     Implements the program class.
/// </summary>
using static tinyWebApi.WebApi.Configurations.Extensions;
var filePath = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, "appsettings.json");
if (File.Exists(filePath))
{
    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(filePath));
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        var url = data.GetValueOrDefault<string, object>("urls");
        if (args is not null && args.Length > 0)
            for (int i = 0; i < args.Length; i++)
            {
                var param = args[i];
                if (param == "--urls")
                    if (args.Length > i + 1)
                    {
                        url = args[i + 1];
                        break;
                    }
            }
        if (IsRunningInProcessIIS)
        {
            Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((h, c) =>
            {
                c.Build();
            }).ConfigureTinyLogger().ConfigureWebHostDefaults(w =>
            {
                w.UseStartup<Startup>();
            }).Build().Run();
        }
        else
        {
            switch (args)
            {
                case not null when args.Length > 0:
                    {
                        CreateDefaultBuilderAndTinyLogger(args).ConfigureWebHostDefaults(wb =>
                        {
                            wb.UseUrls(url is string ? (url as string) : "http://localhost:5000");
                            wb.UseStartup<Startup>();
                        }).Build().Run();
                        break;
                    }
                case null when args.Length == 0:
                    Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((hbc, icb) =>
                    {
                        icb.Build();
                    }).ConfigureTinyLogger().ConfigureWebHostDefaults(wb =>
                    {
                        wb.UseUrls(url is string ? (url as string) : "http://localhost:5000");
                        wb.UseStartup<Startup>();
                    }).UseWindowsService();
                    break;
                default:
                    Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((h, c) =>
                    {
                        c.Build();
                    }).ConfigureTinyLogger().ConfigureWebHostDefaults(w =>
                    {
                        w.UseStartup<Startup>();
                    }).Build().Run();
                    break;
            }
        }
    }
    else CreateDefaultBuilderAndTinyLogger(args).ConfigureWebHostDefaults(wb => wb.UseStartup<Startup>()).Build().Run();
}
else Environment.FailFast($"Missing appsettings.json file at path : {filePath} and hence unable to start tinyWebApi.");
