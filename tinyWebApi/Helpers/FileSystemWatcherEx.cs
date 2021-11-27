using System;
using System.IO;

namespace tinyWebApi.Helpers
{
    /// <summary>
    /// Extended File System Watcher
    /// </summary>
    public class FileSystemWatcherEx : FileSystemWatcher, IDisposable
    {
        private FileSystemWatcher objWatcher { get; set; }
        /// <summary>
        /// Shared Object to be passed on along with generated events.
        /// </summary>
        public object SharedObject { get; set; }
        /// <summary>
        /// Unique identifier to identify the monitoring job.
        /// </summary>
        public Guid Guid { get; set; } = Guid.NewGuid();
        /// <summary>
        /// Automatically disables the raising of events after first change occurs.
        /// </summary>
        public bool IsNotifyFirstChangeOnly { get; set; }
        /// <summary>
        /// Starts watching the file within a specified directory.
        /// If IsNotifyFirstChangeOnly is true then EnableRaisingEvents will be enabled immidiately.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fileName"></param>
        public void StartWatching(string directory, string fileName)
        {
            objWatcher = new();
            objWatcher.Path = directory;
            objWatcher.Filter = fileName;
            objWatcher.Created += ObjWatcher_Created;
            objWatcher.Deleted += ObjWatcher_Deleted;
            objWatcher.Changed += ObjWatcher_Changed;
            objWatcher.Renamed += ObjWatcher_Renamed;
            objWatcher.Error += ObjWatcher_Error;
            if (IsNotifyFirstChangeOnly) 
                objWatcher.EnableRaisingEvents = true;
        }
        /// <summary>
        /// Recieve error notification.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ObjWatcher_Error(object sender, ErrorEventArgs e)
        {
            try
            {
                if (ErrorEx is not null)
                {
                    if (IsNotifyFirstChangeOnly)
                        objWatcher.EnableRaisingEvents = false;
                    ErrorEx(this, new ErrorEventArgsEx(e) { SharedObject = this.SharedObject, Guid = this.Guid });
                }
            }
            catch
            {
                if (IsNotifyFirstChangeOnly)
                    objWatcher.EnableRaisingEvents = false;
                throw;
            }
        }
        /// <summary>
        /// Recieve change notification.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ObjWatcher_Created(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (CreatedEx is not null)
                {
                    if (IsNotifyFirstChangeOnly)
                        objWatcher.EnableRaisingEvents = false;
                    CreatedEx(this, new FileSystemEventArgsEx(e) { SharedObject = this.SharedObject, Guid = this.Guid });
                }
            }
            catch
            {
                if (IsNotifyFirstChangeOnly)
                    objWatcher.EnableRaisingEvents = false;
                throw;
            }
        }
        /// <summary>
        /// Recieve deletion notification.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ObjWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (DeletedEx is not null)
                {
                    if (IsNotifyFirstChangeOnly)
                        objWatcher.EnableRaisingEvents = false;
                    DeletedEx(this, new FileSystemEventArgsEx(e) { SharedObject = this.SharedObject, Guid = this.Guid });
                }
            }
            catch
            {
                if (IsNotifyFirstChangeOnly)
                    objWatcher.EnableRaisingEvents = false;
                throw;
            }
        }
        /// <summary>
        /// Recieve change notification.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ObjWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (ChangedEx is not null)
                {
                    if (IsNotifyFirstChangeOnly)
                        objWatcher.EnableRaisingEvents = false;
                    ChangedEx(this, new FileSystemEventArgsEx(e) { SharedObject = this.SharedObject, Guid = this.Guid });
                }
            }
            catch
            {
                if (IsNotifyFirstChangeOnly)
                    objWatcher.EnableRaisingEvents = false;
                throw;
            }
        }
        /// <summary>
        /// Recieve rename notification.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ObjWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            try
            {
                if (RenamedEx is not null)
                {
                    if (IsNotifyFirstChangeOnly)
                        objWatcher.EnableRaisingEvents = false;
                    RenamedEx(this, new RenamedEventArgsEx(e) { SharedObject = this.SharedObject, Guid = this.Guid });
                }
            }
            catch
            {
                if (IsNotifyFirstChangeOnly)
                    objWatcher.EnableRaisingEvents = false;
                throw;
            }
        }
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
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
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        /// <summary>
        /// Dispose the File System Watcher Extended object.
        /// </summary>
        public new void Dispose()
        {
            SharedObject = null;
            base.Dispose();
        }
    }
    /// <summary>
    /// Extended Error Event Args to return shared object. 
    /// </summary>
    public class ErrorEventArgsEx : ErrorEventArgs
    {
        /// <summary>
        /// Extended File System Event Args constructor.
        /// </summary>
        /// <param name="ex"></param>
        public ErrorEventArgsEx(ErrorEventArgs ex) : base(ex.GetException()) { }
        /// <summary>
        /// Extended Error Event Args constructor.
        /// </summary>
        /// <param name="exception"></param>
        public ErrorEventArgsEx(Exception exception) : base(exception) { }
        /// <summary>
        /// Shared Objected to be returned back.
        /// </summary>
        public dynamic SharedObject { get; set; }
        /// <summary>
        /// Unique Guid for the call.
        /// </summary>
        public Guid Guid { get; set; }
    }
    /// <summary>
    /// Extended Renamed Event Args to return shared object. 
    /// </summary>
    public class RenamedEventArgsEx : RenamedEventArgs
    {
        /// <summary>
        /// Extended File System Event Args constructor.
        /// </summary>
        /// <param name="ex"></param>
        public RenamedEventArgsEx(RenamedEventArgs ex) : base(ex.ChangeType, ex.FullPath, ex.Name, ex.OldName) { }
        /// <summary>
        /// Extended Renamed Event Args constructor.
        /// </summary>
        /// <param name="changeType"></param>
        /// <param name="directory"></param>
        /// <param name="name"></param>
        /// <param name="oldName"></param>
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public RenamedEventArgsEx(WatcherChangeTypes changeType, string directory, string? name, string? oldName) : base(changeType, directory, name, oldName) { }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        /// <summary>
        /// Shared Objected to be returned back.
        /// </summary>
        public dynamic SharedObject { get; set; }
        /// <summary>
        /// Unique Guid for the call.
        /// </summary>
        public Guid Guid { get; set; }
    }
    /// <summary>
    /// Extended File System Event Args to return shared object. 
    /// </summary>
    public class FileSystemEventArgsEx : FileSystemEventArgs
    {
        /// <summary>
        /// Extended File System Event Args constructor.
        /// </summary>
        /// <param name="ex"></param>
        public FileSystemEventArgsEx(FileSystemEventArgs ex) : base(ex.ChangeType, ex.FullPath, ex.Name) { }
        /// <summary>
        /// Extended File System Event Args constructor.
        /// </summary>
        /// <param name="changeType"></param>
        /// <param name="directory"></param>
        /// <param name="name"></param>
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public FileSystemEventArgsEx(WatcherChangeTypes changeType, string directory, string? name) : base(changeType, directory, name) { }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        /// <summary>
        /// Shared Objected to be returned back.
        /// </summary>
        public dynamic SharedObject { get; set; }
        /// <summary>
        /// Unique Guid for the call.
        /// </summary>
        public Guid Guid { get; set; }
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
