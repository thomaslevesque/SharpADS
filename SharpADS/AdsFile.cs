using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;

namespace SharpADS
{
    public static class AdsFile
    {
        #region Stream creation

        public static AdsFileStream Open(string path, string streamName, FileMode mode)
        {
            return new AdsFileStream(path, streamName, mode);
        }

        public static AdsFileStream Open(string path, string streamName, FileMode mode, FileAccess access)
        {
            return new AdsFileStream(path, streamName, mode, access);
        }

        public static AdsFileStream Open(string path, string streamName, FileMode mode, FileAccess access, FileShare share)
        {
            return new AdsFileStream(path, streamName, mode, access, share);
        }

        public static AdsFileStream Create(string path, string streamName,
            int bufferSize = AdsFileStream.DefaultBufferSize,
            FileOptions options = FileOptions.None,
            FileSecurity fileSecurity = null)
        {
            return new AdsFileStream(path, streamName, FileMode.Create, FileAccess.ReadWrite, FileShare.None, bufferSize, options, fileSecurity);
        }

        public static AdsFileStream OpenRead(string path, string streamName)
        {
            return new AdsFileStream(path, streamName, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public static AdsFileStream OpenWrite(string path, string streamName)
        {
            return new AdsFileStream(path, streamName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        }

        #endregion

        #region Read/Write all bytes

        public static byte[] ReadAllBytes(string path, string streamName)
        {
            using (var stream = Open(path, streamName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (stream.Length > int.MaxValue)
                    throw new IOException("File too long, maximum size is 2GB");
                using (var ms = new MemoryStream((int) stream.Length))
                {
                    stream.CopyTo(ms);
                    return ms.GetBuffer();
                }
            }
        }

        public static void WriteAllBytes(string path, string streamName, byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");
            using (var stream = Open(path, streamName, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        #endregion

        #region Text IO helpers

        public static StreamReader OpenText(string path, string streamName, Encoding encoding = null)
        {
            var stream = new AdsFileStream(path, streamName, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                return encoding != null
                    ? new StreamReader(stream, encoding)
                    : new StreamReader(stream);
            }
            catch
            {
                stream.Dispose();
                throw;
            }
        }

        public static StreamWriter CreateText(string path, string streamName, Encoding encoding = null)
        {
            var stream = new AdsFileStream(path, streamName, FileMode.Create, FileAccess.Write, FileShare.Read);
            try
            {
                return encoding != null
                    ? new StreamWriter(stream, encoding)
                    : new StreamWriter(stream);
            }
            catch
            {
                stream.Dispose();
                throw;
            }
        }

        public static StreamWriter AppendText(string path, string streamName, Encoding encoding = null)
        {
            var stream = new AdsFileStream(path, streamName, FileMode.Append, FileAccess.Write, FileShare.Read);
            try
            {
                return encoding != null
                    ? new StreamWriter(stream, encoding)
                    : new StreamWriter(stream);
            }
            catch
            {
                stream.Dispose();
                throw;
            }
        }

        public static string ReadAllText(string path, string streamName, Encoding encoding = null)
        {
            using (var reader = OpenText(path, streamName, encoding))
            {
                return reader.ReadToEnd();
            }
        }

        public static void WriteAllText(string path, string streamName, string contents, Encoding encoding = null)
        {
            if (contents == null) throw new ArgumentNullException("contents");
            using (var writer = CreateText(path, streamName, encoding))
            {
                writer.Write(contents);
            }
        }

        public static string[] ReadAllLines(string path, string streamName, Encoding encoding = null)
        {
            return ReadLines(path, streamName, encoding).ToArray();
        }

        public static IEnumerable<string> ReadLines(string path, string streamName, Encoding encoding = null)
        {
            using (var reader = OpenText(path, streamName, encoding))
            {
                return ReadLinesInternal(reader);
            }
        }

        private static IEnumerable<string> ReadLinesInternal(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }

        public static void WriteAllLines(string path, string streamName, IEnumerable<string> contents, Encoding encoding = null)
        {
            if (contents == null) throw new ArgumentNullException("contents");
            using (var writer = CreateText(path, streamName, encoding))
            {
                foreach (var line in contents)
                {
                    writer.WriteLine(line);
                }
            }
        }

        public static void AppendAllText(string path, string streamName, string contents, Encoding encoding = null)
        {
            if (contents == null) throw new ArgumentNullException("contents");
            using (var writer = AppendText(path, streamName, encoding))
            {
                writer.Write(contents);
            }
        }

        #endregion

        #region File operations

        public static bool Exists(string path, string streamName)
        {
            try
            {
                using (OpenRead(path, streamName))
                {
                    return true;
                }
            }
            catch (FileNotFoundException)
            {
            }
            catch (DirectoryNotFoundException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            return false;
        }

        public static void Delete(string path, string streamName)
        {
            Interop.DeleteFile(string.Join(":", path, streamName));
        }

        #endregion
    }
}
