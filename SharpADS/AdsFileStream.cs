using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using Microsoft.Win32.SafeHandles;

namespace SharpADS
{
    public class AdsFileStream : FileStream
    {
        internal const int DefaultBufferSize = 4096;

        public AdsFileStream(string path, string streamName, FileMode mode)
            : this(path, streamName, mode, (mode == FileMode.Append ? FileAccess.Write : FileAccess.ReadWrite))
        {
        }

        public AdsFileStream(string path, string streamName, FileMode mode, FileAccess access)
            : this(path, streamName, mode, access, FileShare.Read)
        {
        }

        public AdsFileStream(string path, string streamName, FileMode mode, FileAccess access, FileShare share)
            : this(path, streamName, mode, access, share, DefaultBufferSize)
        {
        }

        public AdsFileStream(string path, string streamName, FileMode mode, FileAccess access, FileShare share, int bufferSize)
            : this(path, streamName, mode, access, share, bufferSize, FileOptions.None)
        {
        }

        public AdsFileStream(string path, string streamName, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options)
            : this(path, streamName, mode, access, share, bufferSize, options, null)
        {
        }

        public AdsFileStream(string path, string streamName, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options, FileSecurity fileSecurity)
            : base(CreateHandle(path, streamName, mode, access, share, options, fileSecurity), access, bufferSize, options.HasFlag(FileOptions.Asynchronous))
        {
        }

        private static SafeFileHandle CreateHandle(
            string path,
            string streamName,
            FileMode mode,
            FileAccess access,
            FileShare share,
            FileOptions options,
            FileSecurity fileSecurity)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (streamName == null) throw new ArgumentNullException("streamName");
            if (path.Length == 0) throw new ArgumentException("path cannot be empty", "path");
            if (streamName.Length == 0) throw new ArgumentException("streamName cannot be empty", "streamName");

            string completePath = string.Join(":", path, streamName);
            GCHandle? pinningHandle = null;
            var secAttrs = fileSecurity != null ? GetSecAttrs(share, fileSecurity, out pinningHandle) : GetSecAttrs(share);
            try
            {
                var handle = Interop.CreateFile(
                    completePath,
                    (uint) access,
                    (uint) share,
                    secAttrs,
                    (uint) mode,
                    (uint) options,
                    IntPtr.Zero);

                if (handle.IsInvalid)
                {
                    var hr = Marshal.GetHRForLastWin32Error();
                    Marshal.ThrowExceptionForHR(hr);
                }
                return handle;
            }
            finally
            {
                if (pinningHandle != null)
                {
                    pinningHandle.Value.Free();
                }
            }
        }

        private static Interop.SECURITY_ATTRIBUTES GetSecAttrs(FileShare share)
        {
            Interop.SECURITY_ATTRIBUTES secAttrs = null;
            if ((share & FileShare.Inheritable) != 0)
            {
                secAttrs = new Interop.SECURITY_ATTRIBUTES();
                secAttrs.nLength = Marshal.SizeOf(secAttrs);

                secAttrs.bInheritHandle = 1;
            }
            return secAttrs;
        }

        private unsafe static Interop.SECURITY_ATTRIBUTES GetSecAttrs(FileShare share, FileSecurity fileSecurity, out GCHandle? pinningHandle)
        {
            pinningHandle = null;
            Interop.SECURITY_ATTRIBUTES secAttrs = null;
            if ((share & FileShare.Inheritable) != 0 || fileSecurity != null)
            {
                secAttrs = new Interop.SECURITY_ATTRIBUTES();
                secAttrs.nLength = Marshal.SizeOf(secAttrs);

                if ((share & FileShare.Inheritable) != 0)
                {
                    secAttrs.bInheritHandle = 1;
                }

                // For ACL's, get the security descriptor from the FileSecurity.
                if (fileSecurity != null)
                {
                    byte[] sd = fileSecurity.GetSecurityDescriptorBinaryForm();
                    pinningHandle = GCHandle.Alloc(sd, GCHandleType.Pinned);
                    fixed (byte* pSecDescriptor = sd)
                        secAttrs.pSecurityDescriptor = pSecDescriptor;
                }
            }
            return secAttrs;
        }
    }
}
