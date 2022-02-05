// <copyright file="EncryptFactory.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Implements the encrypt factory class.
// </summary>
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using tiny.WebApi.DataObjects;
namespace tiny.WebApi.Helpers
{
    /// <summary>
    ///     An encrypt factory.
    /// </summary>
    [DebuggerStepThrough]
    public class EncryptFactory
    {
        /// <summary>
        ///     Encrypts.
        /// </summary>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        /// <param name="encryptString"> The encrypt string. </param>
        /// <param name="key">           The base 64 supported string as key. </param>
        /// <returns>
        ///     A string.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static string Encrypt(string encryptString, string key)
        {
            try
            {
                Global.LogDebug("Inside Encrypt, encrypting the plain text.");
                byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
                using (Aes encryptor = Aes.Create())
                {
                    using (Rfc2898DeriveBytes pdb = new(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }))
                    {
                        encryptor.Key = pdb.GetBytes(32);
                        encryptor.IV = pdb.GetBytes(16);
                    }
                    using MemoryStream ms = new();
                    using (CryptoStream cs = new(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
                Global.LogDebug("Returning the encrypted text.");
                return encryptString;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to encrypt the text. {Environment.NewLine}Error: {ex.Message}");
            }
        }
        /// <summary>
        ///     Decrypts.
        /// </summary>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        /// <param name="decryptString"> The decrypt string. </param>
        /// <param name="key">           The base 64 supported string as key. </param>
        /// <returns>
        ///     A string.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static string Decrypt(string decryptString, string key)
        {
            try
            {
                Global.LogDebug("Inside Decrypt, decrypting the encrypted text.");
                byte[] cipherBytes = Convert.FromBase64String(decryptString);
                using (Aes encryptor = Aes.Create())
                {
                    using (Rfc2898DeriveBytes pdb = new(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }))
                    {
                        encryptor.Key = pdb.GetBytes(32);
                        encryptor.IV = pdb.GetBytes(16);
                    }
                    using MemoryStream ms = new();
                    using (CryptoStream cs = new(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    decryptString = Encoding.Unicode.GetString(ms.ToArray());
                }
                Global.LogDebug("Returning the plain text.");
                return decryptString;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to decrypt the text. {Environment.NewLine}Error: {ex.Message}");
            }
        }
    }
}
