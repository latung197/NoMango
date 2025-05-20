//==================================================================================================
// System  : DenKa
// File    : FileHelper.cs
// Author  : Tan Vu (tanvd@robotcom-fa.com)
// Updated : 05/06/2024
// Note    : Copyright 2018-2024, RFVN, All rights reserved
//
// FileHelper.
//
// Modification Log:
// Version  Date        By       Notes
// -------  ----------  -------  --------
// 1.0.0    05/06/2024  Tan Vu   Created the code
//==================================================================================================

using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlastMB.Helper
{
    /// <summary>
    /// Custom event args class.
    /// </summary>
    public class MsgFileArgs<T> : EventArgs
    {
        #region Private Data Members
        #endregion

        #region Public Properties - Can be whatever you want them to be, just providing this as a reference
        public List<T> Records
        {
            get;
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Class constructor, all parameters are required all the time!
        /// </summary>
        /// <param name="data"></param>
        /*public MsgFileArgs(T data)
        {
            Record = data;
        }*/

        public MsgFileArgs(List<T> lst)
        {
            Records = lst;
        }

        #endregion
    }

    public class FileHelper
    {
        /// <summary>
        /// The delegate used to handle this type of event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void HandlerFileCreated(object sender, EventArgs e);

        //public event EventHandler<MsgFileArgs>? FileEvent;
        public event HandlerFileCreated FileEvent;

        private FileSystemWatcher watcher;

        private string _path { get; set; } = string.Empty;

        private readonly string extension = "*.csv";

        // create a static logger field
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public FileHelper(string path)
        {
            _path = path;
        }

        public void RunWatcher()
        {
            // Check if the directories exist
            if (!Directory.Exists(_path))
            {
                //_logger.LogError("Vui lòng kiểm tra lại folder.");
                return;
            }

            watcher = new FileSystemWatcher
            {
                Path = _path,
                Filter = extension
            };

            watcher.Created += new FileSystemEventHandler(OnCreated);
            //watcher.Changed += new FileSystemEventHandler(OnChanged);
            //watcher.Renamed += new RenamedEventHandler(OnRenamed);
            watcher.Deleted += new FileSystemEventHandler(OnDeleted);
            watcher.Error += new ErrorEventHandler(WatcherError);
            watcher.EnableRaisingEvents = true;
            //notify follow by criteria watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.NotifyFilter = NotifyFilters.FileName;
        }

        /// <summary>
        /// Run for the first time or when there is a new file
        /// </summary>
        /// <returns></returns>
        public FileInfo[] ScanFolder()
        {
            if (!Directory.Exists(_path))
            {
                //_logger.LogError("Vui lòng kiểm tra lại folder.");
                //return -1;
                //return Array.Empty<FileInfo>();
                return new FileInfo[0];
            }
            var direct = new DirectoryInfo(_path);
            return direct.GetFiles(extension, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// event when new file created
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnCreated(object source, FileSystemEventArgs e)
        {
            try
            {
                var newfile = ScanFolder();

                // TODO: config delay watcher
                System.Threading.Thread.Sleep(200);     // 200 180

                // Raise event
                //FileEvent?.Invoke(this, new MsgFileArgs<int>(newfile));
                FileEvent?.Invoke(source, new MsgFileArgs<FileInfo>(newfile.ToList()));
            }
            catch (Exception ex)
            {
                logger.Error(Utility.GetExceptionInfo(ex, "FileHelper.cs"));
            }
        }

        /// <summary>
        /// event when file is delete
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            try
            {
                var newfile = ScanFolder();
                // Raise event
                FileEvent?.Invoke(source, new MsgFileArgs<FileInfo>(newfile.ToList()));
            }
            catch (Exception ex)
            {
                logger.Error(Utility.GetExceptionInfo(ex, "FileHelper.cs"));
            }
        }

        /// <summary>
        /// event when new watcher error
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void WatcherError(object source, ErrorEventArgs e)
        {
            //Loggers.LogAlert("Disconnect monitor !");
        }

        /// <summary>
        /// Disable file watching
        /// </summary>
        public void Stop()
        {
            try
            {
                if (watcher != null)
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                }
            }
            catch (Exception ex)
            {
                logger.Error(Utility.GetExceptionInfo(ex, "FileHelper.cs"));
            }
        }
    }
}
