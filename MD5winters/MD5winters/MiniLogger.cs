// MIT License
// 
// Copyright (c) 2024 Theron W. Genaux
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace tgenaux.MiniLogger
{
    public class MiniLogger
    {
        /// <summary>
        /// PathNameReference holds information about a pathname
        /// </summary>
        protected class PathNameReference
        {
            /// <summary>
            /// Pathname - an indirect or full pathname
            /// </summary>
            public string Pathname { get; set; }

            /// <summary>
            /// Returns a new FileInfo(pathname)
            /// </summary>
            public FileInfo FileInfo { get; set; }

            /// <summary>
            /// ReferenceCount - Pathname reference counter
            /// </summary>
            public int ReferenceCount { get; set; }
        }

        public bool EnableTimestamp { get; set; }

        /// <summary>
        /// ConsolOut holds a reference to Console.Out
        /// Can be any TextWriter reference.
        /// Defaults to null
        /// </summary>
        public System.IO.TextWriter ConsolOut
        {
            set { _consoleOut = value; }
            get { return _consoleOut; }
        }
        protected System.IO.TextWriter _consoleOut = null;


        /// <summary>
        /// Disctionary of all pathnames.
        /// There can be many instances of MiniLogger, each with their own file list. A file may
        /// be in one or more instances. This dictionary provieds locks for each file to ensure 
        /// that only one thrsad can to write a file at a time.
        /// </summary>
        protected static Dictionary<string, PathNameReference> _pathnames = new Dictionary<string, PathNameReference>();

        /// <summary>
        /// _miniLoggerLock manages access to static and instance objects, allowing multiple 
        /// threads access for reading or exclusive access for writing.
        /// </summary>
        protected static ReaderWriterLockSlim _miniLoggerLock = new ReaderWriterLockSlim();

        public List<string> Files { get; set; }

        /// <summary>
        /// Adds a log target pathname
        /// </summary>
        /// <param name="pathname">An indirect or full pathname to a log target</param>
        public FileInfo Add(string pathname)
        {
            FileInfo fi = new FileInfo(pathname);

            _miniLoggerLock.EnterWriteLock();
            try
            {
                if (_pathnames.ContainsKey(fi.FullName))
                {
                    _pathnames[fi.FullName].ReferenceCount += 1;
                }
                else
                {
                    _pathnames[fi.FullName] = new PathNameReference()
                    { 
                        Pathname = fi.FullName, 
                        FileInfo = fi, 
                        ReferenceCount = 1 
                    };
                    Files.Add(fi.FullName);
                }
            }
            finally
            {
                _miniLoggerLock.ExitWriteLock();
            }

            return fi;
        }

        /// <summary>
        /// Removes a log target pathname
        /// </summary>
        /// <param name="pathname">An indirect or full pathname to a log target</param>
        public void Remove(string pathname)
        {
            _miniLoggerLock.EnterWriteLock();
            try
            {
                FileInfo fi = new FileInfo(pathname);
                _pathnames.Remove(fi.FullName);
                Files.Remove(fi.FullName);
            }
            finally
            {
                // All unlocking everywhare should reverse the sequence of locking
                _miniLoggerLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MiniLogger()
        {
            Files = new List<string>();
            EnableTimestamp = false;
        }


        #region WritLine

        /// <summary>
        /// WriteLine() - Write blank line to all log targets
        /// </summary>
        public void WriteLine()
        {
            ProtectedWriteLine("");
        }
        public void WriteLine(string text)
        {
            ProtectedWriteLine(text);
        }

        public void WriteLine(string format, params object[] list)
        {
            string text = string.Format(format, list);
            ProtectedWriteLine(text);
        }

        // TODO: 
        //public void WriteLine(char[] buffer, int index, int count)

        //public void WriteLine(string format, params object?[]? arg)

        //public void WriteLine(string format, object? arg0)

        //public void WriteLine(ulong value)
        //public void WriteLine(uint value)

        //public void WriteLine(float value)
        //public void WriteLine(doule value)

        //public void WriteLine(decimal value)

        //public void WriteLine(char[]? buffer)
        //public void WriteLine(char value)

        //public void WriteLine(bool value)

        //public void WriteLine(object? value)

        /// <summary>
        /// Thread-safe Writeline to all log targets
        /// </summary>
        /// <param name="text">the text</param>
        protected void ProtectedWriteLine(string text)
        {
            string _text = text;
            if (EnableTimestamp)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                _text = $"{timestamp}: " + text;
            }

            // Write to the console target
            if (null != ConsolOut)
            {
                ConsolOut.WriteLine(text);
            }

            _miniLoggerLock.EnterReadLock();

            // Wrtie text to all log targets
            try
            {
                foreach (var file in Files)
                {
                    if (_pathnames.ContainsKey(file))
                    {
                        System.IO.File.AppendAllText(file, $"{text}\n");
                    }
                }
            }

            catch (Exception ex)
            {
                // Do we want to rethrow the exception?
                // Will MiniLogger always be used with console programs?
                System.Console.WriteLine(ex);
            }

            finally
            {
                _miniLoggerLock.ExitReadLock();
            }
        }

        // required for strings that contain {}
        //        protected static void WriteLine(string text)
        //        {
        //            Console.WriteLine(text);
        //            if (null != log)
        //            {
        //                log.WriteLine(text);
        //                log.Flush();
        //            }
        //
        //            if (null != report)
        //            {
        //                report.WriteLine(text);
        //                report.Flush();
        //            }
        //        }
        //
        //        // Handles strings without params
        //        protected static void WriteLine(string format, params object[] list)
        //        {
        //            Console.WriteLine(format, list);
        //            if (null != log)
        //            {
        //                log.WriteLine(format, list);
        //                log.Flush();
        //            }
        //
        //            if (null != report)
        //            {
        //                report.WriteLine(format, list);
        //                report.Flush();
        //            }
        //        }
        //
        //        protected static void Write(string format, params object[] list)
        //        {
        //            Console.Write(format, list);
        //            if (null != log)
        //            {
        //                log.WriteLine(format, list);
        //                log.Flush();
        //            }
        //
        //            if (null != report)
        //            {
        //                report.Write(format, list);
        //                report.Flush();
        //            }
        //        }
        #endregion

    }
}
