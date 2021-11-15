/// <copyright file="ExternalAssemblyExecutionHelper.cs" company="tiny">
///     Copyright (c) 2021 tiny. All rights reserved.
/// </copyright>
/// <summary>
///     Implements the external assembly execution helper class.
/// </summary>
using tinyWebApi.Common.DataObjects;
using tinyWebApi.Common.IDataContracts;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
namespace tinyWebApi.Common.Helpers
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
                Assembly asm = null;
                var fileInfo = new FileInfo(filePath);
                foreach(var item in assemblies.Where(item => item.GetName().Name == fileInfo.Name.Replace(fileInfo.Extension, "")))
                {
                    asm = item;
                    found = true;
                    break;
                }
                Type type = found ? asm.GetType(className) : Assembly.LoadFrom(filePath).GetType(className);
                Global.LogInformation("Returning the instance of class from the loaded external assembly.");
                return (IProcessData)Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                Global.LogError($"Class : {className} mapped with dll file path : {filePath} could not be found or some other issue while trying to load the assembly. Please check error for more details... {Environment.NewLine} Error : {ex.Message}", ex);
                return default;
            }
        }
    }
}
