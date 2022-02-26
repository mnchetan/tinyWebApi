// <copyright file="FileReadWriteHelper.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using System.Diagnostics;
using System.IO;
using System.Text;
namespace tiny.WebApi.Helpers
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
        /// <param name="encoding"></param>
        /// <returns></returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public static string ReadAllText(string filePath, Encoding encoding = null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        {
            using var s = new StreamReader(filePath, encoding is null ? Encoding.UTF8 : encoding);
            return s.ReadToEnd();
        }
        /// <summary>
        /// Write all text to file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        /// <param name="isAppend"></param>
        /// <param name="encoding"></param>
        /// <param name="isCreateFolderIfNotExists"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public static void WriteAllText(string filePath, string content, bool isAppend= false, Encoding encoding = null, bool isCreateFolderIfNotExists = true)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        {
#pragma warning disable CS8604 // Possible null reference argument.
            if (isCreateFolderIfNotExists && !Directory.Exists(Path.GetDirectoryName(filePath))) _ = Directory.CreateDirectory(Path.GetDirectoryName(filePath));
#pragma warning restore CS8604 // Possible null reference argument.
            using var s = new StreamWriter(filePath, isAppend, encoding is null ? Encoding.UTF8 : encoding);
            s.Write(content);
        }
        /// <summary>
        /// Write byte array to file.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        /// <param name="mode"></param>
        /// <param name="isCreateFolderIfNotExists"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void WriteByteArrayToFile(this byte[] data, string filePath, FileMode mode = FileMode.OpenOrCreate, bool isCreateFolderIfNotExists = true)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            if (isCreateFolderIfNotExists && !Directory.Exists(Path.GetDirectoryName(filePath))) _ = Directory.CreateDirectory(Path.GetDirectoryName(filePath));
#pragma warning restore CS8604 // Possible null reference argument.
            using var b = new BinaryWriter(File.Open(filePath, mode));
            b.Write(data);
        }
    }
}
