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
using System.IO;

namespace tgenaux.MD5Winters
{
    public class FileChecksum
    {
        /// <summary>
        /// File Name (FileInfo.Name)
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Base Name 
        /// FileName without the extension
        /// </summary>
        public string BaseFileName { get; set; }

        /// <summary>
        /// File type 
        /// file extension without the leading period
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Pathname 
        /// FileInfo.FullName minus the source directory path
        /// </summary>
        public string Pathname
        {
            get => _pathname;
            set
            {
                _pathname = value.Replace("\\", "/");
            }
        }
        string _pathname;

        /// <summary>
        /// FileInfo
        /// </summary>
        public FileInfo FileInfo { get; set; }

        /// <summary>
        /// MD5 Checksum
        /// </summary>
        public string Md5Checksum { get; set; }

        public override string ToString()
        {
            return string.Format($"{FileName}, {Md5Checksum}");
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public FileChecksum()
        {
            FileName = "";
            BaseFileName = "";
            Type = "";
            Pathname = "";
            Md5Checksum = "";
            FileInfo = new FileInfo("some file");
        }

        /// <summary>
        /// Auto fill constructor
        /// Fills in all but MD% using FileInfo and source DirectoryInfo
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="sourceDir"></param>
        public FileChecksum(FileInfo fileInfo, DirectoryInfo sourceDir)
        {
            Md5Checksum = "";
            FileInfo = fileInfo;
            FileName = fileInfo.Name;

            Pathname = fileInfo.FullName.Substring(sourceDir.FullName.Length).Trim('\\');

            BaseFileName = Pathname.Substring(0, Pathname.Length - fileInfo.Extension.Length);

            Type = fileInfo.Extension.Trim('.');
        }

        public FileChecksum ShallowCopy()
        {
            return (FileChecksum)this.MemberwiseClone();
        }
    }
}
