using System.Windows.Forms;
using tiny.WebApi.EncryptDecryptUtility;
// <copyright file="Program.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Implements the program class.
// </summary>
Application.SetHighDpiMode(HighDpiMode.SystemAware);
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
Application.Run(new EncryptDecryptUtility());
