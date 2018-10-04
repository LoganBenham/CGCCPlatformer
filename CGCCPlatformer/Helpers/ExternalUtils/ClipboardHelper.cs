using System;
using System.Runtime.InteropServices;

//Written by Doug Benham

namespace CGCCPlatformer.Helpers.ExternalUtils
{
    public static class ClipboardHelper
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        static extern bool EmptyClipboard();

        [DllImport("user32.dll")]
        static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        static extern IntPtr GetClipboardData(uint uFormat);

        /// <summary>
        /// Sets the clipboard to the given text.
        /// </summary>
        public static void SetText(string Text)
        {
            if (!OpenClipboard(IntPtr.Zero))
                throw new Exception("OpenClipboard() failed (" + Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()) + ").");
            else
            {
                try
                {
                    if (!EmptyClipboard())
                        throw new Exception("EmptyClipboard() failed (" + Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()) + ").");

                    if (SetClipboardData(1, Marshal.StringToHGlobalAnsi(Text)) == IntPtr.Zero)
                        throw new Exception("SetClipboardData() failed (" + Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()) + ").");
                }
                finally
                {
                    if (!CloseClipboard())
                        throw new Exception("CloseClipboard() failed (" + Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()) + ").");
                }
            }
        }

        /// <summary>
        /// Retrieves text from the clipboard.
        /// </summary>
        public static string GetText(bool throwErrors = false)
        {
            if (!OpenClipboard(IntPtr.Zero))
            {
                if (throwErrors) throw new Exception("OpenClipboard() failed (" + Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()) + ").");
            }
            else
            {
                try
                {
                    IntPtr result = GetClipboardData(1);
                    if (result != IntPtr.Zero)
                        return Marshal.PtrToStringAnsi(result);
                }
                finally
                {
                    if (!CloseClipboard())
                    {
                        if (throwErrors) throw new Exception("CloseClipboard() failed (" + Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()) + ").");
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// If an exception is thrown, it will return false.
        /// </summary>
        public static bool TrySetText(string Text)
        {
            try
            {
                SetText(Text);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
