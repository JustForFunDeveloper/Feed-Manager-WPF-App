using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using HS_Feed_Manager.Core.GlobalValues;
using MahApps.Metro.Controls;

namespace HS_Feed_Manager.Core.Handler
{
    [Flags]
    public enum LogLevel
    {
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
    }

    public class LogHandler
    {
        #region Private Methods

        private static LogLevel _logLevel = LogLevel.Debug;

        private static long _debugCounter;
        private static long _warningCounter;
        private static long _infoCounter;
        private static long _errorCounter;

        private static FileHandler _fileHandler;

        #endregion

        #region Public Properties and Events

        /// <summary>
        /// A local counter for Debug Messages during Runtime.
        /// </summary>
        public static long DebugCounter
        {
            get => Interlocked.CompareExchange(ref _debugCounter, 0, 0);
        }
        /// <summary>
        /// A local counter for Warning Messages during Runtime.
        /// </summary>
        public static long WarningCounter
        {
            get => Interlocked.CompareExchange(ref _warningCounter, 0, 0);
        }
        /// <summary>
        /// A local counter for Info Messages during Runtime.
        /// </summary>
        public static long InfoCounter
        {
            get => Interlocked.CompareExchange(ref _infoCounter, 0, 0);
        }
        /// <summary>
        /// A local counter for Error Messages during Runtime.
        /// </summary>
        public static long ErrorCounter
        {
            get => Interlocked.CompareExchange(ref _errorCounter, 0, 0);
        }
        /// <summary>
        /// The current LogLevel of the LogHandler
        /// </summary>
        public static LogLevel LogLevel
        {
            get => _logLevel;
            set => _logLevel = value;
        }
        /// <summary>
        /// Is raised when the counter of the logs changed.
        /// </summary>
        public static event EventHandler<List<long>> CounterChangedEvent;

        public static string CurrentLogName => GetCurrentLogName();
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="fileHandler"></param>
        public LogHandler(LogLevel logLevel, FileHandler fileHandler)
        {
            _logLevel = logLevel;
            _fileHandler = fileHandler;
        }

        #region Public Methods

        /// <summary>
        /// Writes the given message into the log table or log file.
        /// </summary>
        /// <param name="message">The text to write into the log.</param>
        /// <param name="logLevel">The <see cref="LogLevel"/> of this message.</param>
        public static void WriteSystemLog(string message, LogLevel logLevel)
        {
            string text = message.Replace("'", "\"");
            WriteErrorToStandardFile(text, logLevel);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the counters.
        /// </summary>
        /// <param name="logLevel"></param>
        private static void UpdateLogCounter(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    Interlocked.Increment(ref _debugCounter);
                    break;
                case LogLevel.Warning:
                    Interlocked.Increment(ref _warningCounter);
                    break;
                case LogLevel.Info:
                    Interlocked.Increment(ref _infoCounter);
                    break;
                case LogLevel.Error:
                    Interlocked.Increment(ref _errorCounter);
                    break;
            }

            OnCounterChangedEvent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="logLevel"></param>
        private static void WriteErrorToStandardFile(string text, LogLevel logLevel)
        {
            if (logLevel >= _logLevel)
            {
                _fileHandler.CreateFileIfNotExist(GetCurrentLogName());
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(DateTime.Now).Append(";").Append(LogLevel.Error).Append(";").Append(text);
                _fileHandler.AppendText(GetCurrentLogName(), stringBuilder.ToString(),
                    LogicConstants.LogFilePath);
                UpdateLogCounter(logLevel);
            }
        }

        /// <summary>
        /// Gets the current prefix of the log file.
        /// Basically every month a new logfile is created.
        /// </summary>
        /// <returns></returns>
        private static string GetCurrentLogName()
        {
            DateTime date = DateTime.Now;
            string fileName = date.ToString("MMM.yyyy");
            fileName += "_" + LogicConstants.LogFileName;
            return fileName;
        }

        #endregion

        #region Event Methods

        private static void OnCounterChangedEvent()
        {
            CounterChangedEvent?.Invoke("LogHandler", new List<long>()
            {
                Interlocked.CompareExchange(ref _warningCounter, 0, 0),
                Interlocked.CompareExchange(ref _infoCounter, 0, 0),
                Interlocked.CompareExchange(ref _errorCounter, 0, 0)
            });
        }

        #endregion
    }
}
