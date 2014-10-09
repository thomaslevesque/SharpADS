using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace SharpADS
{
    static class Interop
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern SafeFileHandle CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            SECURITY_ATTRIBUTES lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            SafeFileHandle hTemplateFile
        );

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeleteFile(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetVolumeInformation(
            string lpRootPathName,
            StringBuilder lpVolumeNameBuffer,
            int nVolumeNameSize,
            out int lpVolumeNameSerialNumber,
            out int lpMaximumComponentLength,
            out FileSystemFlags lpFileSystemFlags,
            StringBuilder lpFileSystemNameBuffer,
            int nFileSystemNameSize);

        [StructLayout(LayoutKind.Sequential)]
        // ReSharper disable All
        public class SECURITY_ATTRIBUTES
        {
            internal int nLength = 0;
            // don't remove null, or this field will disappear in bcl.small
            internal unsafe byte* pSecurityDescriptor = null;
            internal int bInheritHandle = 0;
        }
        // ReSharper restore All

        [Flags]
        public enum FileSystemFlags
        {
            SupportsPreservedNameCase = 0x00000002,
            SupportsCaseSensitiveSearch= 0x00000001,
            SupportsCompression = 0x00000010,
            SupportsNamedStreams = 0x00040000,
            SupportsPersistentAcls = 0x00000008,
            ReadOnly = 0x00080000,
            SupportsSingleSequentialWrite = 0x00100000,
            SupportsEncryption = 0x00020000,
            SupportsExtendedAttributes = 0x00800000,
            SupportsHardLinks = 0x00400000,
            SupportsObjectIds = 0x00010000,
            SupportsOpenByFileId = 0x01000000,
            SupportsReparsePoints = 0x00000080,
            SupportsSparseFiles = 0x00000040,
            SupportsTransactions = 0x00200000,
            SupportsUsnJournal = 0x02000000,
            SupportsUnicodeOnDisk = 0x00000004,
            Compressed = 0x00008000,
            SupportsQuotas = 0x00000020
        }
    }
}
