using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace SharpADS
{
    static class Interop
    {
        [DllImport("kernel32.dll", EntryPoint = "CreateFileW", SetLastError = true)]
        public static extern SafeFileHandle CreateFile(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            [In] SECURITY_ATTRIBUTES lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            [In] System.IntPtr hTemplateFile
        );

        [DllImport("kernel32.dll", EntryPoint = "DeleteFileW", SetLastError = true)]
        public static extern bool DeleteFile([In] [MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

        [StructLayout(LayoutKind.Sequential)]
        // ReSharper disable All
        internal class SECURITY_ATTRIBUTES
        {
            internal int nLength = 0;
            // don't remove null, or this field will disappear in bcl.small
            internal unsafe byte* pSecurityDescriptor = null;
            internal int bInheritHandle = 0;
        }
        // ReSharper restore All
    }
}
