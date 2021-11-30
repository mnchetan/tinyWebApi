/// <file>
/// 	tiny.WebApi\uninstall.bat
/// </file>
///
// <copyright file="uninstall.bat" company="tiny">
/// 	Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
///
/// <summary>
/// 	Uninstall class.
/// </summary>
sc stop tiny.WebApi_WebApi
timeout /t /5 /nobreak > NUL
sc delete tiny.WebApi_WebApi