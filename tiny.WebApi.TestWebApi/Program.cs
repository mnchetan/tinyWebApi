// <copyright file="Program.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Implements the program class.
// </summary>
using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.InteropServices;
using tiny.WebApi.TestWebApi;
using static tiny.WebApi.Configurations.Extensions;
#pragma warning disable CS8604 // Possible null reference argument.
var filePath = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, "appsettings.json");
if (File.Exists(filePath))
{
    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(tiny.WebApi.Helpers.FileReadWriteHelper.ReadAllText(filePath));
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
            }).ConfigureWebHostDefaults(w =>
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
                        Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(wb =>
                        {
                            wb.UseUrls(url is string ? (url as string) : "http://localhost:5000");
                            wb.UseStartup<Startup>();
                        }).Build().Run();
                        break;
                    }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                case null when args.Length == 0:
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((hbc, icb) =>
                    {
                        icb.Build();
                    }).ConfigureWebHostDefaults(wb =>
                    {
                        wb.UseUrls(url is string ? (url as string) : "http://localhost:5000");
                        wb.UseStartup<Startup>();
                    }).UseWindowsService();
                    break;
                default:
                    Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((h, c) =>
                    {
                        c.Build();
                    }).ConfigureWebHostDefaults(w =>
                    {
                        w.UseStartup<Startup>();
                    }).Build().Run();
                    break;
            }
        }
    }
    else Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(wb => wb.UseStartup<Startup>()).Build().Run();
}
else Environment.FailFast($"Missing appsettings.json file at path : {filePath} and hence unable to start tiny.WebApi.");
#pragma warning restore CS8604 // Possible null reference argument.

