// <copyright file="EncryptFactory.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace tiny.WebApi.EncryptDecryptUtility
{
    /// <summary>   Class EncryptDecryptUtility. </summary>
    [DebuggerStepThrough]
    public class EncryptFactory
    {
        /// <summary>
        /// Encrypts the plain text.
        /// </summary>
        /// <param name="encryptString">The encrypt string.</param>
        /// <param name="key">The key.</param>
        /// <returns>A string.</returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static string Encrypt(string encryptString, string key)
        {
            try
            {
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
                return encryptString;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to encrypt the text. {Environment.NewLine}Error: {ex.Message}");
            }
        }
        /// <summary>
        /// Decrypts the encrypted text.
        /// </summary>
        /// <param name="decryptString">The encrypt string.</param>
        /// <param name="key">The key.</param>
        /// <returns>A string.</returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static string Decrypt(string decryptString, string key)
        {
            try
            {
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
                return decryptString;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to decrypt the text. {Environment.NewLine}Error: {ex.Message}");
            }
        }
    }
}
