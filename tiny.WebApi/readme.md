# tiny.WebApi
tiny Web Api supports almost any type of input and output and connects to Microsoft SQL Server/Oracle database (very much extendable to other relational databases) and allows teams to focus only on database and UI development without even having to make any change in the service. This service does not have any service specific logic and hence could be easily used within any project having Microsoft SQL Server, Oracle as backend database. This supports Get, Post, Put, Delete method automations which could be overridden for extension.
Once deployed this service does not need any future deployment apart from configuration changes.

## Get: 
Using this one could execute any query and pass on any input parameter via the query params as part of URL.
## Post/Put/Delete: 
Using this one could input/upload individual variables, single level array of single type objects, single level array of complex objects via request body supported y JSON (string, number, Boolean, datetime, array of string, array of number, array of datetime, array of Boolean and mix and match of all).
The API is capable of mapping out any input to the supported query or stored procedure and get the output back by mapping the input field names as it is with sql query or stored procedure.
Supported input type is JSON only but it could take in file content as a byte array of file type Excel, CSV and BLOB.
Excel (First sheet only) and CSV file content is uploaded as byte array within request body the filed name in which byte array is passed is shared with api along with file type and a flag stating file content is present in the request. Sheet name is also shared if specific sheet within excel is needed to be read else first sheet of excel will be read. Based on file type data is converted in to xml and shared with stored procedure/query as an xml. BLOB file type is also shared same way and passed on to query/stored procedure as varbinary type.

## Note:
Multiple file upload at a time is supported provided fileContetFieldNames are separated by comma and all files are of same type.
Complex arrays are mapped using SQL table Types which need to follow ADO.Net/Microsoft SQL Server constraint of mapping the types in the order in which the field names are defined in the type. But browsers JSON serializers tend to serialize the objects and arrange properties alphabetically which hence the type definition in database to be done alphabetically.
For Oracle the UDT’s are supported from Oracle 21c onwards and hence before that the UDT’s could be mapped as XML or JSON and made available to the queries/stored procedures as XML/JSON serialized CLOBS.
This api supports hot-reload of the queries and hence restart of the service is not required. Queries are identified using unique key passed on with the calls.

## appsettings.json sample:
```xml
{
  "AllowedHosts": "*",
  "AllowedCorsHosts": "http://*:*;http://localhost:4200",
  "ConfigurationDirectoryPath": "",
  "environment": "local",
  "Logging": {
    "LogLevel": {
      "Default": "Trace",
      "Microsoft": "Trace",
      "Microsoft.Hosting.Lifetime": "Trace"
    },
    "options": {
      "file": "Log_$|Date[dd_MMM_yyyy]|$.log",
      "size": 1073741824
    }
  }
}
```

## connectionstring.<environment>.json sample::
```xml
{
  "db1": {
    "ConnectionString": "sql server connection string",
    "IsEncrypted": false,
    "IsImpersonationNeeded": true,
    "RunAsUser": "systemuser1",
    "EncryptionKey": "",
    "ConnectionTimeOut": 1200,
    "DatabaseType": "MSSQL"
  },
  "db2": {
    "ConnectionString": "oracle connection string",
    "IsEncrypted": true,
    "IsImpersonationNeeded": false,
    "RunAsUser": "",
    "EncryptionKey": "some base 64 encryption key",
    "ConnectionTimeOut": 1200,
    "DatabaseType": "ORACLE"
  }
}
queries.<environment>.json sample:
{
  "GetEmployeeDetails_Query": {
    "Query": "select employeeid as eid, employeename as ename from dbo.employee",
    "ExecutionType": "DataTableText",
    "InputFieldNamesInSequence_UDTDollarSeperatedByType": "",
    "Outputs_RefCursor_InSequence_CommaSeperated_WithIntFieldsSeperatedByColon_NotRequiredForMSSQL": "",
    "Database": "db1",
    "IsMapUDTAsJSON_ApplicableForOracle": false,
    "IsMapUDTAsXML_ApplicableForOracle": false,
    "ExternalDllPathImplementingIProcessDataInterface_PreProcessing": "",
    "ExternalDllNameImplementingIProcessDataInterface_PreProcessing": "",
    "FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PreProcessing": "",
    "ExternalDllPathImplementingIProcessDataInterface_PostProcessing": "",
    "ExternalDllNameImplementingIProcessDataInterface_PostProcessing": "",
    "FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing": "",
    "IsSendOutputViaEmailAlso": false,
    "Mailer": "somemailername1",
    "IsAllowSendingJSONInMail": false,
    "IsCachingRequired": false,
    "CacheDurationInSeconds": 0
  },
  "GetEmployeeDetails_StoredProcedure": {
    "Query": "usp_GetEmployeeDetails @EID = @EmployeeId, Department=@Department",
    "ExecutionType": "DataTableText",
    "InputFieldNamesInSequence_UDTDollarSeperatedByType": "EmployeeId,Department$dbo.Department",
    "Outputs_RefCursor_InSequence_CommaSeperated_WithIntFieldsSeperatedByColon_NotRequiredForMSSQL": "",
    "Database": "db1",
    "IsMapUDTAsJSON_ApplicableForOracle": false,
    "IsMapUDTAsXML_ApplicableForOracle": false,
    "ExternalDllPathImplementingIProcessDataInterface_PreProcessing": "",
    "ExternalDllNameImplementingIProcessDataInterface_PreProcessing": "",
    "FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PreProcessing": "",
    "ExternalDllPathImplementingIProcessDataInterface_PostProcessing": "",
    "ExternalDllNameImplementingIProcessDataInterface_PostProcessing": "",
    "FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing": "",
    "IsSendOutputViaEmailAlso": false,
    "Mailer": "somemailername2",
    "IsAllowSendingJSONInMail": false,
    "IsCachingRequired": false,
    "CacheDurationInSeconds": 0
  },
  "GetEmployeeDetails": {
    "Query": "usp_GetEmployeeDetails",
    "ExecutionType": "DataTableText",
    "InputFieldNamesInSequence_UDTDollarSeperatedByType": "",
    "Outputs_RefCursor_InSequence_CommaSeperated_WithIntFieldsSeperatedByColon_NotRequiredForMSSQL": "P_CUR:EMPLOYEEID:DEPARTMENTID,P_CUR1:DEPARTMENTID:VERTICALID",
    "Database": "db1",
    "IsMapUDTAsJSON_ApplicableForOracle": false,
    "IsMapUDTAsXML_ApplicableForOracle": false,
    "ExternalDllPathImplementingIProcessDataInterface_PreProcessing": "",
    "ExternalDllNameImplementingIProcessDataInterface_PreProcessing": "",
    "FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PreProcessing": "",
    "ExternalDllPathImplementingIProcessDataInterface_PostProcessing": "",
    "ExternalDllNameImplementingIProcessDataInterface_PostProcessing": "",
    "FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing": "",
    "IsSendOutputViaEmailAlso": false,
    "Mailer": "somemailername2",
    "IsAllowSendingJSONInMail": false,
    "IsCachingRequired": false,
    "CacheDurationInSeconds": 0
  }
}
```

## mailers.<environment>.json sample:
```xml
mailers.<environment>.json sample:
{
  "somemailername1": {
    "SMTP_SERVER": "smtp.server",
    "SMTP_PORT": 1234,
    "IsEncrypted": false,
    "IsImpersonationNeeded": false,
    "RunAsUser": "systemuser1",
    "From": "noreply@noreply.com",
    "To": "noreply@noreply.com",
    "CC": "noreply@noreply.com",
    "BCC": "noreply@noreply.com",
    "Subject": "some subject",
    "Body": "some body",
    "AttachmentName": "some attachment name : DD-MMM-YYYY",
    "IsBodyHtml": false
  },
  "somemailername2": {
    "SMTP_SERVER": "smtp.server",
    "SMTP_PORT": 1234,
    "IsEncrypted": false,
    "IsImpersonationNeeded": false,
    "RunAsUser": "systemuser1",
    "From": "noreply@noreply.com",
    "To": "noreply@noreply.com",
    "CC": "noreply@noreply.com",
    "BCC": "noreply@noreply.com",
    "Subject": "some subject",
    "Body": "some body",
    "AttachmentName": "some attachment name : DD-MMM-YYYY",
    "IsBodyHtml": false
  }
}
```

## users.<environment>.json sample:
```xml
{
  "systemuser1": {
    "RunAsDomain": "domain",
    "RunAsUserName": "username",
    "RunAsPassword": "password",
    "IsRunAsPasswordEncrypted": true,
    "EncryptionKey": "some base 64 encryption key"
  },
  "systemuser2": {
    "RunAsDomain": "domain",
    "RunAsUserName": "username",
    "RunAsPassword": "password",
    "IsRunAsPasswordEncrypted": true,
    "EncryptionKey": "some base 64 encryption key"
  }
}
```

## In .Net 5
## Program.cs
``` csharp
Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((h, c) =>
                    {
                        c.Build();
                    }).ConfigureWebHostDefaults(w =>
                    {
                        w.UseStartup<Startup>();
                    }).Build().Run();
                    break;
```
## Startup.cs
``` csharp
using tiny.WebApi.Configurations;
public class Startup
    {
        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"> The services. </param>
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
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </remarks>
        
public void Configure(IApplicationBuilder app, IWebHostEnvironment env) => app.UseTinyWebApi(env);
    } 
```

## In .Net 6
## Program.cs
``` csharp
using tiny.WebApi.Configurations;
using tiny.WebApi.WebApi.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTinyWebApi(new TinyWebApiConfigurations()
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

var app = builder.Build();

app.UseTinyWebApi(app.Environment);
app.Run();
```

## Note:
After adding the package to the project ensure that connectionstring.<environment>.json, queries.<environment>.json, mailers.<environment>.json & users.<environment>.json file(s) should be marked as Copy to Output Directory as Copy always or Copy if newer.

## License
MIT License

Copyright (c) 2021 tinyChetan

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

1. Mentioning name of this library in credits page is mandatory.

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Credits:
'tiny.Logger' used for default logging.
Api icons created by iconixar - Flaticon.
