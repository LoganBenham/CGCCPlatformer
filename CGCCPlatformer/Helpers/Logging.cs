using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CGCCPlatformer.Helpers.ExternalUtils;

namespace CGCCPlatformer.Helpers
{
    public static class Logging
    {
        public const int MaxLogFiles = 25;

        public static bool Silenced;

        private static Level _logLevel;

        public enum Level
        {
            Error,
            Warning,
            Info,
            Debug
        }

        public static string LogFolderPath { get; private set; }
        public static string LogPath { get; private set; }
        public static string LatestLogPath { get; private set; }
        public static bool Initialized { get; private set; }

        public static Level LogLevel
        {
            get { return _logLevel; }
            set
            {
                _logLevel = value;
                WriteLine(Level.Info, "Logging Level set to " + _logLevel);
            }
        }

        public static string TimeStamp() => "[" + DateTime.Now.Hour.ToString("00") + ":" +
                                            DateTime.Now.Minute.ToString("00") + ":" +
                                            DateTime.Now.Second.ToString("00") + "] ";

        public static string MsTimeStamp() => "[" + DateTime.Now.Hour.ToString("00") + ":" +
                                              DateTime.Now.Minute.ToString("00") + ":" +
                                              DateTime.Now.Second.ToString("00") + ":" +
                                              DateTime.Now.Millisecond.ToString("000") + "] ";

        private static string LevelStamp(Level level) => "[" + level + "] ";

        private static object logLock = new object();

        public static void Initialize(Level level = Level.Info)
        {
            if (Initialized)
            {
                WriteLine(Level.Warning, "Logging initialize called unneccessarily");
                return;
            }
            Initialized = true;
            Silenced = false;
            _logLevel = level;

            //generate filename
            var filenameBuilder = new StringBuilder();
            filenameBuilder.Append(DateTime.Today.Year + "-" + DateTime.Today.Month + "-" + DateTime.Today.Day);
            filenameBuilder.Append("--" + DateTime.Now.Hour.ToString("00") + "-" +
                                   DateTime.Now.Minute.ToString("00") + "-" +
                                   DateTime.Now.Second.ToString("00"));
            string filename = filenameBuilder.ToString();

            //paths
            LogFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Logs\\";
            LogPath = LogFolderPath + filename + ".log";
            LatestLogPath = LogFolderPath + "latestdebug.log";

            //create dir and files
            var folder = Directory.CreateDirectory(LogFolderPath);
            var file = File.Create(LogPath);
            file.Close();
            var latest = File.Create(LatestLogPath);
            latest.Close();

            _logLevel = Level.Info;
            WriteLines(Level.Info,
                Assembly.GetExecutingAssembly().GetName().Name + " Log: " + filename + ".log",
                "Build Version: " + Assembly.GetExecutingAssembly().GetName().Version,
                "Build Date: " + Assembly.GetExecutingAssembly().GetLinkerTime().ToShortDateString(),
                "Logging Level: " + LogLevel);
            WriteLine(Level.Info);
            _logLevel = level;

            var logFiles = folder.EnumerateFiles("*.log").ToArray();
            int deleteLogs = logFiles.Length - 1 - MaxLogFiles;
            if (deleteLogs > 0) //delete oldest logs
            {
                WriteLine(Level.Warning, "You are over the limit of " + MaxLogFiles + " log files.");
                deleteLogs += 5;
                logFiles = logFiles.OrderBy(logFile => logFile.LastWriteTime).ToArray();
                for (var i = 0; i < deleteLogs; i++)
                {
                    WriteLine(Level.Warning, "Deleting " + logFiles[i].Name);
                    logFiles[i].Delete();
                }
            }
        }


        /// <summary> Writes a new line </summary>
        /// <param name="level">Logging level to determine importance</param>
        public static void WriteLine(Level level)
        {
            if (!Initialized)
                throw new InvalidOperationException("Initialize the logger before you write to it");
            if (Silenced) return;

            lock (logLock)
            {
                using (var file = new StreamWriter(LatestLogPath, true))
                {
                    file.WriteLine();
                }

                if (level > LogLevel)
                    return;

                Debug.WriteLine("");
                using (var file = new StreamWriter(LogPath, true))
                {
                    file.WriteLine();
                }
            }
            
        }

        /// <summary> Writes at Level.Info </summary>
        public static void WriteLine(string line, int stackSkip = 1, bool msTime = false) =>
            WriteLine(Level.Info, line, stackSkip, msTime);

        public static void WriteLine(Level level, string line, int stackSkip = 1, bool msTime = false)
        {
            if (!Initialized)
                throw new InvalidOperationException("Initialize the logger before you write to it");
            if (Silenced) return;
            

            if (stackSkip > 0)
            {
                /* Full stack trace
                try
                {
                    int i = -2;
                    while (true)
                    {
                        var typei = new StackFrame(i).GetMethod().DeclaringType;
                        Console.WriteLine(i + " " + typei + " " + typei.Name);
                        i++;
                    }
                } catch(NullReferenceException e) { }
                */

                var type = new StackFrame(stackSkip).GetMethod().DeclaringType;
                string source = type != null ? type.Name : "Unknown Source";
                line = "[" + source + "] " + line;
            }

            lock (logLock)
            {
                using (var file = new StreamWriter(LatestLogPath, true))
                {
                    file.WriteLine((msTime ? MsTimeStamp() : TimeStamp()) + LevelStamp(level) + line);
                }

                if (level > LogLevel)
                    return;

                Debug.WriteLine((msTime ? MsTimeStamp() : TimeStamp()) + LevelStamp(level) + line);
                using (var file = new StreamWriter(LogPath, true))
                {
                    file.WriteLine((msTime ? MsTimeStamp() : TimeStamp()) + LevelStamp(level) + line);
                }
            }
        }

        public static void WriteLines(Level level, params string[] lines) => WriteLines(level, false, 2, lines);

        public static void WriteLines(Level level, bool dateStamp = false, int stackSkip = 1, params string[] lines)
        {
            if (!Initialized)
                throw new InvalidOperationException("Initialize the logger before you write to it");
            if (Silenced) return;
            

            if (stackSkip > 0)
            {
                var type = new StackFrame(stackSkip).GetMethod().DeclaringType;
                string source = type != null ? type.Name : "Unknown Source";
                for (var i = 0; i < lines.Length; i++)
                    lines[i] = "[" + source + "] " + lines[i];
            }

            lock (logLock)
            {
                using (var file = new StreamWriter(LatestLogPath, true))
                {
                    foreach (string line in lines)
                    {
                        file.WriteLine(TimeStamp() + LevelStamp(level) + line);
                    }
                }

                if (level > LogLevel)
                    return;

                foreach (string line in lines)
                    Debug.WriteLine(TimeStamp() + LevelStamp(level) + line);
                using (var file = new StreamWriter(LogPath, true))
                {
                    foreach (string line in lines)
                    {
                        file.WriteLine(TimeStamp() + LevelStamp(level) + line);
                    }
                }
            }
        }
    }
}