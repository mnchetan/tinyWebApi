// <copyright file="ImpersonationHelper.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Implements the impersonation helper class.
// </summary>
using Microsoft.Win32.SafeHandles;
using Mono.Unix.Native;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using tiny.WebApi.DataObjects;
namespace tiny.WebApi.Helpers
{
    /// <summary>
    ///     An impersonation helper.
    /// </summary>
    [DebuggerStepThrough]
    public class ImpersonationHelper
    {
        /// <summary>
        ///     Logon user.
        /// </summary>
        /// <param name="userName">      Name of the user. </param>
        /// <param name="domain">        The domain. </param>
        /// <param name="password">      The password. </param>
        /// <param name="logonType">     Type of the logon. </param>
        /// <param name="logonProvider"> The logon provider. </param>
        /// <param name="token">         [out] The token. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(string userName, string domain, string password, int logonType, int logonProvider, out SafeAccessTokenHandle token);
        /// <summary>
        ///     (Immutable) the logon type new credentials.
        /// </summary>
        private const int LOGON_TYPE_NEW_CREDENTIALS = 0;
        /// <summary>
        ///     (Immutable) the logon 32 provide default.
        /// </summary>
        private const int LOGON32_PROVIDE_DEFAULT = 9;
        /// <summary>
        ///     Executes the function as impersonated.
        ///     For Linux impersonation if not ran with root access then KeyTab is needed.
        ///     Impersonation for Linux without KetTab and with root access will not work with Anonymous Authenitcation.
        /// </summary>
        /// <exception cref="Win32Exception"> Thrown when a Window 32 error condition occurs. </exception>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="func">          The function. </param>
        /// <param name="runAsUserName"> Name of the run as user. </param>
        /// <param name="runAsDomain">   The run as domain. </param>
        /// <param name="runAsPassword"> The run as password. </param>
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static T Execute<T>(Func<T> func, string runAsUserName, string runAsDomain, string runAsPassword)
        {
            Global.LogDebug("Inside Execute, execute the function and return data as impersonated.");
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                bool returnValue = LogonUser(runAsUserName, runAsDomain, runAsPassword, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDE_DEFAULT, out SafeAccessTokenHandle token);
                if (returnValue) return WindowsIdentity.RunImpersonated(token, () => func.Invoke());
                var ret = Marshal.GetLastWin32Error();
                throw new Win32Exception(ret);
            }
            else
            {
                T result;
                try
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    var oldUser = Syscall.getpwnam(Global.CurrentHttpContext?.User?.Identity?.Name.Split("@", StringSplitOptions.None)[0].Replace("@", ""));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    var newUser = Syscall.getpwnam(runAsUserName);
                    _ = Syscall.seteuid(newUser.pw_uid);
                    _ = Syscall.setegid(newUser.pw_gid);
                    result = func.Invoke();
                    _ = Syscall.seteuid(oldUser.pw_uid);
                    _ = Syscall.setegid(oldUser.pw_gid);
                }
                catch
                {
                    result = func.Invoke();
                }
                return result;
            }
        }
        /// <summary>
        ///     Executes the action as impersonated.
        ///     For Linux impersonation if not ran with root access then KeyTab is needed.
        ///     Impersonation for Linux without KetTab and with root access will not work with Anonymous Authenitcation.
        /// </summary>
        /// <exception cref="Win32Exception"> Thrown when a Window 32 error condition occurs. </exception>
        /// <param name="action">        The action. </param>
        /// <param name="runAsUserName"> Name of the run as user. </param>
        /// <param name="runAsDomain">   The run as domain. </param>
        /// <param name="runAsPassword"> The run as password. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void Execute(Action action, string runAsUserName, string runAsDomain, string runAsPassword)
        {
            Global.LogDebug("Inside Execute, execute the action as impersonated.");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                bool returnValue = LogonUser(runAsUserName, runAsDomain, runAsPassword, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDE_DEFAULT, out SafeAccessTokenHandle token);
                if (returnValue) WindowsIdentity.RunImpersonated(token, () => action.Invoke());
                var ret = Marshal.GetLastWin32Error();
                throw new Win32Exception(ret);
            }
            else
            {
                try
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    var oldUser = Syscall.getpwnam(Global.CurrentHttpContext?.User?.Identity?.Name.Split("@", StringSplitOptions.None)[0].Replace("@", ""));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    var newUser = Syscall.getpwnam(runAsUserName);
                    _ = Syscall.seteuid(newUser.pw_uid);
                    _ = Syscall.setegid(newUser.pw_gid);
                    action.Invoke();
                    _ = Syscall.seteuid(oldUser.pw_uid);
                    _ = Syscall.setegid(oldUser.pw_gid);
                }
                catch
                {
                    action.Invoke();
                }
            }
        }
        /// <summary>
        ///     Executes the action as impersonated.
        ///     For Linux impersonation if not ran with root access then KeyTab is needed.
        ///     Impersonation for Linux without KetTab and with root access will not work with Anonymous Authenitcation.
        /// </summary>
        /// <param name="action">                The action. </param>
        /// <param name="databaseSpecification"> The database specification. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void Execute(Action action, DatabaseSpecification databaseSpecification) => Execute(action, databaseSpecification.RunAsUserSpecification);
        /// <summary>
        ///     Executes the funtion as impersonated.
        ///     For Linux impersonation if not ran with root access then KeyTab is needed.
        ///     Impersonation for Linux without KetTab and with root access will not work with Anonymous Authenitcation.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="func">                  The function. </param>
        /// <param name="databaseSpecification"> The database specification. </param>
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static T Execute<T>(Func<T> func, DatabaseSpecification databaseSpecification) => Execute(func, databaseSpecification.RunAsUserSpecification);
        /// <summary>
        ///     Executes the action as impersonated.
        ///     For Linux impersonation if not ran with root access then KeyTab is needed.
        ///     Impersonation for Linux without KetTab and with root access will not work with Anonymous Authenitcation.
        /// </summary>
        /// <param name="action">                The action. </param>
        /// <param name="runAsUserSpecification"> The run as user specification. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void Execute(Action action, RunAsUserSpecification runAsUserSpecification) => Execute(action, runAsUserSpecification.RunAsUserName, runAsUserSpecification.RunAsDomain, runAsUserSpecification.IsRunAsPasswordEncrypted ? EncryptFactory.Decrypt(runAsUserSpecification.RunAsPassword, runAsUserSpecification.EncryptionKey) : runAsUserSpecification.RunAsPassword);
        /// <summary>
        ///     Executes the funtion as impersonated.
        ///     For Linux impersonation if not ran with root access then KeyTab is needed.
        ///     Impersonation for Linux without KetTab and with root access will not work with Anonymous Authenitcation.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="func">                  The function. </param>
        /// <param name="runAsUserSpecification"> The database specification. </param>
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static T Execute<T>(Func<T> func, RunAsUserSpecification runAsUserSpecification) => Execute(func, runAsUserSpecification.RunAsUserName, runAsUserSpecification.RunAsDomain, runAsUserSpecification.IsRunAsPasswordEncrypted ? EncryptFactory.Decrypt(runAsUserSpecification.RunAsPassword, runAsUserSpecification.EncryptionKey) : runAsUserSpecification.RunAsPassword);
        /// <summary>
        ///     Executes the action as impersonated.
        ///     For Linux impersonation if not ran with root access then KeyTab is needed.
        ///     Impersonation for Linux without KetTab and with root access will not work with Anonymous Authenitcation.
        /// </summary>
        /// <param name="action">                The action. </param>
        /// <param name="mailerSpecification"> The mailer specification. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void Execute(Action action, MailerSpecification mailerSpecification) => Execute(action, mailerSpecification.RunAsUserSpecification);
        /// <summary>
        ///     Executes the funtion as impersonated.
        ///     For Linux impersonation if not ran with root access then KeyTab is needed.
        ///     Impersonation for Linux without KetTab and with root access will not work with Anonymous Authenitcation.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="func">                  The function. </param>
        /// <param name="mailerSpecification"> The mailer specification. </param>
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static T Execute<T>(Func<T> func, MailerSpecification mailerSpecification) => Execute(func, mailerSpecification.RunAsUserSpecification);
    }
}
