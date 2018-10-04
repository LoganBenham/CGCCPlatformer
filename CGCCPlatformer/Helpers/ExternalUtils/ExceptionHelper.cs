using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

//Written by Doug Benham

namespace CGCCPlatformer.Helpers.ExternalUtils
{
    public static class ExceptionHelper
    {
        public static void Initialize()
        {
            int pid;
            var args = Environment.GetCommandLineArgs();
            if (args.Length >= 3 && args[1] == "-pid" && int.TryParse(args[2], out pid))
            {
                try
                {
                    Process.GetProcessById(pid).WaitForExit();
                }
                catch { }
            }

            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += Application_Exception;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        }

        public static string GetExceptionDetails(this Exception exception)
        {
            return string.Join("\n", GetExceptionDetailsInternal(exception));
        }

        private static string[] GetExceptionDetailsInternal(this Exception exception)
        {
            var type = exception.GetType();
            var properties = type.GetProperties();
            var fields = properties
                .SelectMany(x =>
                {
                    var val = x.GetValue(exception, null);
                    if (val == null)
                        return new string[0];
                    if (val is Exception)
                        return new[] {$"{x.Name} = "}.Concat((val as Exception).GetExceptionDetailsInternal().Select(s => "  " + s));
                    if (val is WebResponse)
                    {
                        var webResponse = val as WebResponse;
                        using (var sr = new StreamReader(webResponse.GetResponseStream()))
                            return new[] {"Response = " + sr.ReadToEnd()};
                    }
                    return new[] {$"{x.Name} = {val.ToString()}"};
                });

            return new[] {type.FullName}.Concat(fields).ToArray();
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            ProcessException(e.Exception.InnerException.GetExceptionDetails());            
        }

        private static void Application_Exception(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null) ProcessException(ex.GetExceptionDetails());
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ProcessException(e.Exception.GetExceptionDetails());
        }

        public static void ProcessException(string message)
        {
            bool success = ClipboardHelper.TrySetText(message);
            
            Logging.WriteLine(Logging.Level.Error, message, 3);
            MessageBox.Show(string.Format("There was an error. Please contact me with the error message information.{0}{1}{1}{2}",
                    (success ? " The error information was attached to the clipboard." : "Press ctrl+c to copy the error information."),
                    Environment.NewLine, message),
                Application.ProductName,
                MessageBoxButtons.OK, 
                MessageBoxIcon.Error);
        }
    }
}
