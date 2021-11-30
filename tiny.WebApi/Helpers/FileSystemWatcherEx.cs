// <copyright file="Extensions.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using System;
using System.Diagnostics;
using System.IO;
namespace tiny.WebApi.Helpers
{
    /// <summary>
    /// Extended File System Watcher
    /// </summary>
    [DebuggerStepThrough]
    public class FileSystemWatcherEx : FileSystemWatcher, IDisposable
    {
        /// <summary>
        /// File System Watcher Object
        /// </summary>
        [DebuggerHidden]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private FileSystemWatcher ObjWatcher { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Shared Object to be passed on along with generated events.
        /// </summary>
        [DebuggerHidden]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public object SharedObject { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Unique identifier to identify the monitoring job.
        /// </summary>
        [DebuggerHidden]
        public Guid Guid { get; set; } = Guid.NewGuid();
        /// <summary>
        /// Automatically disables the raising of events after first change occurs.
        /// </summary>
        [DebuggerHidden]
        public bool IsNotifyFirstChangeOnly { get; set; }
        /// <summary>
        /// Starts watching the file within a specified directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fileName"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public void StartWatching(string directory, string fileName)
        {
            if (ObjWatcher is null)
            {
                ObjWatcher = new();
            }
            ObjWatcher.Path = directory;
            ObjWatcher.Filter = fileName;
            ObjWatcher.Created -= ObjWatcher_Created;
            ObjWatcher.Deleted -= ObjWatcher_Deleted;
            ObjWatcher.Changed -= ObjWatcher_Changed;
            ObjWatcher.Renamed -= ObjWatcher_Renamed;
            ObjWatcher.Error -= ObjWatcher_Error;
            ObjWatcher.Created += ObjWatcher_Created;
            ObjWatcher.Deleted += ObjWatcher_Deleted;
            ObjWatcher.Changed += ObjWatcher_Changed;
            ObjWatcher.Renamed += ObjWatcher_Renamed;
            ObjWatcher.Error += ObjWatcher_Error;
            ObjWatcher.EnableRaisingEvents = true;
        }
        /// <summary>
        /// Recieve error notification.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        private void ObjWatcher_Error(object sender, ErrorEventArgs e)
        {
            try
            {
                if (ErrorEx is not null)
                {
                    if (IsNotifyFirstChangeOnly)
                        ObjWatcher.EnableRaisingEvents = false;
                    ErrorEx(this, new ErrorEventArgsEx(e) { SharedObject = this.SharedObject, Guid = this.Guid });
                }
            }
            catch
            {
                if (IsNotifyFirstChangeOnly)
                    ObjWatcher.EnableRaisingEvents = false;
                throw;
            }
        }
        /// <summary>
        /// Recieve change notification.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        private void ObjWatcher_Created(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (CreatedEx is not null)
                {
                    if (IsNotifyFirstChangeOnly)
                        ObjWatcher.EnableRaisingEvents = false;
                    CreatedEx(this, new FileSystemEventArgsEx(e) { SharedObject = this.SharedObject, Guid = this.Guid });
                }
            }
            catch
            {
                if (IsNotifyFirstChangeOnly)
                    ObjWatcher.EnableRaisingEvents = false;
                throw;
            }
        }
        /// <summary>
        /// Recieve deletion notification.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        private void ObjWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (DeletedEx is not null)
                {
                    if (IsNotifyFirstChangeOnly)
                        ObjWatcher.EnableRaisingEvents = false;
                    DeletedEx(this, new FileSystemEventArgsEx(e) { SharedObject = this.SharedObject, Guid = this.Guid });
                }
            }
            catch
            {
                if (IsNotifyFirstChangeOnly)
                    ObjWatcher.EnableRaisingEvents = false;
                throw;
            }
        }
        /// <summary>
        /// Recieve change notification.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        private void ObjWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (ChangedEx is not null)
                {
                    if (IsNotifyFirstChangeOnly)
                        ObjWatcher.EnableRaisingEvents = false;
                    ChangedEx(this, new FileSystemEventArgsEx(e) { SharedObject = this.SharedObject, Guid = this.Guid });
                }
            }
            catch
            {
                if (IsNotifyFirstChangeOnly)
                    ObjWatcher.EnableRaisingEvents = false;
                throw;
            }
        }
        /// <summary>
        /// Recieve rename notification.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        private void ObjWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            try
            {
                if (RenamedEx is not null)
                {
                    if (IsNotifyFirstChangeOnly)
                        ObjWatcher.EnableRaisingEvents = false;
                    RenamedEx(this, new RenamedEventArgsEx(e) { SharedObject = this.SharedObject, Guid = this.Guid });
                }
            }
            catch
            {
                if (IsNotifyFirstChangeOnly)
                    ObjWatcher.EnableRaisingEvents = false;
                throw;
            }
        }
        /// <summary>
        /// Extended changed event.
        /// </summary>
        public event FileSystemEventHandlerEx? ChangedEx;
        /// <summary>
        /// Extended error event.
        /// </summary>
        public event ErrorEventHandlerEx? ErrorEx;
        /// <summary>
        /// Extended deleted event.
        /// </summary>
        public event FileSystemEventHandlerEx? DeletedEx;
        /// <summary>
        /// Extended created event.
        /// </summary>
        public event FileSystemEventHandlerEx? CreatedEx;
        /// <summary>
        /// Extended renamed event.
        /// </summary>
        public event RenamedEventHandler? RenamedEx;
        /// <summary>
        /// Dispose the File System Watcher Extended object.
        /// </summary>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public new void Dispose()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            SharedObject = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            base.Dispose();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Extended File System Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void FileSystemEventHandlerEx(object sender, FileSystemEventArgsEx e);
        /// <summary>
        /// Extended Error Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ErrorEventHandlerEx(object sender, ErrorEventArgsEx e);
        /// <summary>
        /// Extended Renamed Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void RenamedEventHandler(object sender, RenamedEventArgsEx e);
    }
    /// <summary>
    /// Extended Error Event Args to return shared object. 
    /// </summary>
    [DebuggerStepThrough]
    public class ErrorEventArgsEx : ErrorEventArgs
    {
        /// <summary>
        /// Extended File System Event Args constructor.
        /// </summary>
        /// <param name="ex"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ErrorEventArgsEx(ErrorEventArgs ex) : base(ex.GetException()) { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Extended Error Event Args constructor.
        /// </summary>
        /// <param name="exception"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ErrorEventArgsEx(Exception exception) : base(exception) { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Shared Objected to be returned back.
        /// </summary>
        [DebuggerHidden]
        public dynamic SharedObject { get; set; }
        /// <summary>
        /// Unique Guid for the call.
        /// </summary>
        [DebuggerHidden]
        public Guid Guid { get; set; }
    }
    /// <summary>
    /// Extended Renamed Event Args to return shared object. 
    /// </summary>
    [DebuggerStepThrough]
    public class RenamedEventArgsEx : RenamedEventArgs
    {
        /// <summary>
        /// Extended File System Event Args constructor.
        /// </summary>
        /// <param name="ex"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public RenamedEventArgsEx(RenamedEventArgs ex) : base(ex.ChangeType, ex.FullPath, ex.Name, ex.OldName) { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Extended Renamed Event Args constructor.
        /// </summary>
        /// <param name="changeType"></param>
        /// <param name="directory"></param>
        /// <param name="name"></param>
        /// <param name="oldName"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public RenamedEventArgsEx(WatcherChangeTypes changeType, string directory, string? name, string? oldName) : base(changeType, directory, name, oldName) { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Shared Objected to be returned back.
        /// </summary>
        [DebuggerHidden]
        public dynamic SharedObject { get; set; }
        /// <summary>
        /// Unique Guid for the call.
        /// </summary>
        [DebuggerHidden]
        public Guid Guid { get; set; }
    }
    /// <summary>
    /// Extended File System Event Args to return shared object. 
    /// </summary>
    [DebuggerStepThrough]
    public class FileSystemEventArgsEx : FileSystemEventArgs
    {
        /// <summary>
        /// Extended File System Event Args constructor.
        /// </summary>
        /// <param name="ex"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public FileSystemEventArgsEx(FileSystemEventArgs ex) : base(ex.ChangeType, ex.FullPath, ex.Name) { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Extended File System Event Args constructor.
        /// </summary>
        /// <param name="changeType"></param>
        /// <param name="directory"></param>
        /// <param name="name"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public FileSystemEventArgsEx(WatcherChangeTypes changeType, string directory, string? name) : base(changeType, directory, name) { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Shared Objected to be returned back.
        /// </summary>
        [DebuggerHidden]
        public dynamic SharedObject { get; set; }
        /// <summary>
        /// Unique Guid for the call.
        /// </summary>
        [DebuggerHidden]
        public Guid Guid { get; set; }
    }
}
