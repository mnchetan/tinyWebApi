/// <file>
/// 	tiny.WebApi\install.bat
/// </file>
///
// <copyright file="install.bat" company="tiny">
/// 	Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
///
/// <summary>
/// 	Install class.
/// </summary>
sc create tiny.WebApi_WebApi binPath= %~dp0tiny.WebApi.exe
sc failure tiny.WebApi_WebApi actions= restart/60000/restart/60000/""/60000 reset= 86400
sc start tiny.WebApi_WebApi
sc config tiny.WebApi_WebApi start=auto