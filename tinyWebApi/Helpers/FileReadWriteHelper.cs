// <copyright file="MailerSpecification.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using System.IO;
using System.Text;
namespace tinyWebApi.Helpers
{
    /// <summary>
    /// File read write helper.
    /// </summary>
    public static class FileReadWriteHelper
    {
        /// <summary>
        /// Read text from file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ReadAllText(string filePath) => new StreamReader(filePath, Encoding.UTF8).ReadToEnd();
        /// <summary>
        /// Write text to file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        public static void WriteAllText(string filePath, string content) => new StreamWriter(filePath, false, Encoding.UTF8).Write(content);
    }
}
