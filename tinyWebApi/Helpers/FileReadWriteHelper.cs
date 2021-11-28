// <copyright file="MailerSpecification.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using System.Diagnostics;
using System.IO;
using System.Text;
namespace tinyWebApi.Helpers
{
    /// <summary>
    /// File read write helper.
    /// </summary>
    [DebuggerStepThrough]
    public static class FileReadWriteHelper
    {
        /// <summary>
        /// Read text from file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static string ReadAllText(string filePath) => new StreamReader(filePath, Encoding.UTF8).ReadToEnd();
        /// <summary>
        /// Write text to file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void WriteAllText(string filePath, string content) => new StreamWriter(filePath, false, Encoding.UTF8).Write(content);
    }
}
