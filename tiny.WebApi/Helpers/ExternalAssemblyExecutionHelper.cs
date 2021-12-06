// <copyright file="ExternalAssemblyExecutionHelper.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Implements the external assembly execution helper class.
// </summary>
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using tiny.WebApi.DataObjects;
using tiny.WebApi.IDataContracts;
namespace tiny.WebApi.Helpers
{
    /// <summary>
    ///     An external assembly execution helper.
    /// </summary>
    [DebuggerStepThrough]
    public class ExternalAssemblyExecutionHelper
    {
        /// <summary>
        ///     Loads plugin from file.
        /// </summary>
        /// <param name="filePath">  Full pathname of the file. </param>
        /// <param name="className"> Name of the class. </param>
        /// <returns>
        ///     The plugin from file.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static IProcessData LoadPluginFromFile(string filePath, string className)
        {
            try
            {
                Global.LogInformation("Inside LoadPluginFromFile, Load external assembly from the filepath if not already loaded in app domain.");
                var found = false;
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Assembly asm = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                var fileInfo = new FileInfo(filePath);
                foreach(var item in assemblies.Where(item => item.GetName().Name == fileInfo.Name.Replace(fileInfo.Extension, "")))
                {
                    asm = item;
                    found = true;
                    break;
                }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Type type = found ? asm.GetType(className) : Assembly.LoadFrom(filePath).GetType(className);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                Global.LogInformation("Returning the instance of class from the loaded external assembly.");
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
                return Activator.CreateInstance(type) as IProcessData;
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8603 // Possible null reference return.
            }
            catch (Exception ex)
            {
                Global.LogError($"Class : {className} mapped with dll file path : {filePath} could not be found or some other issue while trying to load the assembly. Please check error for more details... {Environment.NewLine} Error : {ex.Message}", ex);
#pragma warning disable CS8603 // Possible null reference return.
                return default;
#pragma warning restore CS8603 // Possible null reference return.
            }
        }
        /// <summary>
        /// Load plugin from file of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static T LoadPluginFromFile<T>(string filePath, string className)
        {
            try
            {
                Global.LogInformation("Inside LoadPluginFromFile, Load external assembly from the filepath if not already loaded in app domain.");
                var found = false;
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Assembly asm = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                var fileInfo = new FileInfo(filePath);
                foreach (var item in assemblies.Where(item => item.GetName().Name == fileInfo.Name.Replace(fileInfo.Extension, "")))
                {
                    asm = item;
                    found = true;
                    break;
                }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Type type = found ? asm.GetType(className) : Assembly.LoadFrom(filePath).GetType(className);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                Global.LogInformation("Returning the instance of class from the loaded external assembly.");
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                return (T)Activator.CreateInstance(type);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8603 // Possible null reference return.
            }
            catch (Exception ex)
            {
                Global.LogError($"Class : {className} mapped with dll file path : {filePath} could not be found or some other issue while trying to load the assembly. Please check error for more details... {Environment.NewLine} Error : {ex.Message}", ex);
#pragma warning disable CS8603 // Possible null reference return.
                return default;
#pragma warning restore CS8603 // Possible null reference return.
            }
        }
    }
}
