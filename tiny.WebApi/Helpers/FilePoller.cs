// <copyright file="FilePoller.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
namespace tiny.WebApi.Helpers
{
    /// <summary>
    /// File Poller
    /// Use this only when FileSystemWatcher proves to be ineffecient like when trying to watch files within mounted drives.
    /// </summary>
    //[DebuggerStepThrough]
    public class FilePoller
    {
        /// <summary>
        /// Starts watching the file within the provided directory.
        /// Supports wild card characters like *.
        /// Returns the first matching file.
        /// This method will physically search the file in the provided directory path that is it will continuously search the file within the top level directory till the time out occurs unless explicitly specified to search in all nested directories as well.
        /// Poll time should also take in to consideration regarding the network latency for network paths.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fileName"></param>
        /// <param name="pollIntervalInSeconds"></param>
        /// <param name="pollTimeOutInSeconds"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        //[DebuggerHidden]
        //[DebuggerStepThrough]
        public async Task<string> StartWatching(string directory, string fileName, int pollIntervalInSeconds = 60, int pollTimeOutInSeconds = 1200, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            string result = "";
            try
            {
                var files = Directory.GetFiles(directory, fileName, searchOption);
                if (files.Length > 0) return await Task.FromResult(files[0]);
            }
            catch
            {
                throw;
            }
            bool isExit = false;
            DateTime startTime = DateTime.UtcNow;
            while (!isExit)
            {
                try
                {
                    var files = Directory.GetFiles(directory, fileName, searchOption);
                    if (files.Length > 0)
                    {
                        isExit = true;
                        result = files[0];
                    }
                    if (!isExit && DateTime.UtcNow.Subtract(startTime).TotalSeconds >= pollTimeOutInSeconds)
                        isExit = true;
                }
                catch { isExit = true; }
                if (!isExit)
                    await Task.Delay(pollIntervalInSeconds * 1000);
            }
            return await Task.FromResult(result);
        }
    }
}