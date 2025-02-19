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
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace tgenaux.MD5Winters
{
    // TODO:
    // Report problems writting MS5 files to disk
    class Md5WintersChecksums
    {
        /// <summary>
        /// Console logger
        /// </summary>
        static public MiniLogger.MiniLogger _logger = 
            new MiniLogger.MiniLogger();

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine($"Md5winters pathname ");
                Console.WriteLine("");
                Console.WriteLine($"    pathname(s) - one or more file or folder paths");
                Console.Write("Press any key to continue ...");
                Console.WriteLine("");
                Console.ReadKey();
                Environment.Exit(-1);
            }

            Console.Title = "MD5winters";
            _logger.ConsolOut = Console.Out;
            _logger.WriteLine("# MD5 checksums by MD5winters");
            _logger.WriteLine();

            foreach (var sourcePathname in args)
            {
                FileInfo md5File = null;

                // if MD5 file
                if (File.Exists(sourcePathname))
                {
                    FileInfo fi = new FileInfo(sourcePathname);
                    md5File = String.Equals(".md5", fi.Extension,
                        StringComparison.InvariantCultureIgnoreCase) ? fi : null;
                }

                if (md5File == null)
                {
                    List<FileChecksum> checksums;
                    List<FileChecksum> md5Files;

                    GenerateChecksums(sourcePathname, out checksums, out md5Files);
                    MD5ReportWriter(sourcePathname, checksums, md5Files);
                }

                else // Verify MD5 File
                {
                    VerifyMD5fileChecksums(md5File);
                }
            }
        }

        /// <summary>
        /// Generate Checksums from a file or directory
        /// </summary>
        /// <param name="sourcePathname"></param>
        /// <param name="checksums"></param>
        /// <param name="md5Files"></param>
        public static void GenerateChecksums(string sourcePathname, out List<FileChecksum> checksums, out List<FileChecksum> md5Files)
        {
            checksums = new List<FileChecksum>();
            md5Files = new List<FileChecksum>();

            if (File.Exists(sourcePathname))
            {
                FileInfo fi = new FileInfo(sourcePathname);
                Console.Title = $"MD5Winters - {sourcePathname}";

                FileChecksum checksum = new FileChecksum(fi, fi.Directory)
                {
                    Md5Checksum = CreateMD5(fi.FullName).ToLower()
                };

                checksums.Add(checksum);
                _logger.WriteLine(checksum.ToString());
                _logger.WriteLine();
            }

            else if (Directory.Exists(sourcePathname))
            {
                List<string> found = new List<string>();

                DirectoryInfo sourceDir = new DirectoryInfo(sourcePathname);
                Console.Title = $"MD5Winters - {sourceDir.Name}";

                DirectoryInfo di = new DirectoryInfo(sourcePathname);

                FileInfo[] files = di.GetFiles("*.*",
                    SearchOption.TopDirectoryOnly);

                foreach (var fi in files)
                {
                    Console.Title = $"MD5Winters - {sourceDir.Name}\\{fi.Name}";

                    FileChecksum checksum = new FileChecksum(fi, new DirectoryInfo(sourcePathname))
                    {
                        Md5Checksum = CreateMD5(fi.FullName)
                    };
                    checksums.Add(checksum);

                    // if this is an MD5 file, then add it to the md5Files for later verification
                    // against the related generated file.
                    // Only MD5 files that hold 1 checksum can be added
                    if (IsMD5file(checksum))
                    {
                        List<FileChecksum> fileChecksums = GetChecksumsFromMd5(fi.FullName);
                        // to keep it simple, only add for MD5 files that have one checksum
                        if (fileChecksums.Count == 1)
                        {
                            // Make a copy with the checksum from the md5 file
                            FileChecksum copy = checksum.ShallowCopy();
                            copy.Md5Checksum = fileChecksums[0].Md5Checksum;
                            md5Files.Add(copy);
                        }
                    }

                    _logger.WriteLine(checksum.ToString());
                    _logger.WriteLine();
                }

                var folders = di.GetDirectories();
                foreach (var folder in folders)
                {
                    if (folder.Name != "System Volume Information")
                    {
                        FileInfo[] dirFiles = folder.GetFiles("*.*", SearchOption.AllDirectories);
                        foreach (var fi in dirFiles)
                        {
                            Console.Title = $"MD5Winters - {sourceDir.Name}\\{fi.Name}";

                            FileChecksum checksum = new FileChecksum(fi, new DirectoryInfo(sourcePathname))
                            {
                                Md5Checksum = CreateMD5(fi.FullName).ToLower()
                            };
                            checksums.Add(checksum);

                            // if this is an MD5 file, then add it to the md5Files for later verification
                            // against the related generated file.
                            // Only MD5 files that hold 1 checksum can be added
                            if (IsMD5file(checksum))
                            {
                                List<FileChecksum> fileChecksums = GetChecksumsFromMd5(fi.FullName);
                                // to keep it simple, only add for MD5 files that have one checksum
                                if (fileChecksums.Count == 1)
                                {
                                    // Make a copy with the checksum from the md5 file
                                    FileChecksum copy = checksum.ShallowCopy();
                                    copy.Md5Checksum = fileChecksums[0].Md5Checksum;
                                    md5Files.Add(copy);
                                }
                            }

                            _logger.WriteLine(checksum.ToString());
                        }
                    }
                }
            }
            else //source does not exist
            {
                _logger.WriteLine($"Source does not exist: {sourcePathname}");
                return;
            }
        }

        public static string GetDriveLable(string iAmRoot)
        {
            string label = "";
            DriveInfo driveInfo = new DriveInfo(iAmRoot);

            if (null != driveInfo)
            {
                //  Volume label: J72421  USB2
                string[] parts = driveInfo.VolumeLabel.Split(':');
                label = driveInfo.VolumeLabel;
            }
            return label;
        }


        /// <summary>
        /// MD5ReportWriter - writes the MD5 file 
        /// </summary>
        /// <param name="sourcePathname">the source (file or directory) for the MD5 checksums</param>
        /// <param name="checksums">Collection of source files</param>
        /// <param name="md5Files">Subset of source files that are also MD5 files</param>
        static public void MD5ReportWriter(string sourcePathname, List<FileChecksum> checksums, List<FileChecksum> md5Files)
        {
            if (checksums.Count <= 0)
            {
                // nothing to do
                return;
            }
            string md5ReportPathname = ""; // the new MD5 file

            // if source is a single file
            if (File.Exists(sourcePathname))
            {
                // The write the MD5 file in the same folder as the source file
                FileInfo fi = new FileInfo(sourcePathname);

                string basename = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
                md5ReportPathname = fi.DirectoryName + "/" + basename + ".md5";
            }

            // else if the source is a directory
            else if (Directory.Exists(sourcePathname))
            {
                DirectoryInfo di = new DirectoryInfo(sourcePathname);

                // Is this the root of a drive? (D:\)
                if (di.FullName.Contains(":") && (di.FullName.Length == 3))
                {
                    // then use its name too
                    string md5Filename = "Checksums.md5"; // default

                    string driveLabel = GetDriveLable(sourcePathname);
                    if (driveLabel.Length > 0)
                    {
                        md5Filename = $"{driveLabel} {md5Filename}";
                    }

                    md5ReportPathname = di.FullName.TrimEnd('\\') + $"\\{md5Filename}";
                }
                else // No, it is a subfolder
                {
                    // Then write the MD5 file in the same folder as the source directory
                    md5ReportPathname = di.FullName.TrimEnd('\\') + ".md5";
                }
            }
            else //source does not exist
            {
                _logger.WriteLine($"Source does not exist: {sourcePathname}");
                return;
            }

            if (File.Exists(md5ReportPathname))
            {
                File.Delete(md5ReportPathname);
            }

            // the report writer
            MiniLogger.MiniLogger _report = new MiniLogger.MiniLogger();
            _report.Add(md5ReportPathname);

            _report.WriteLine("# MD5 checksums generated by MD5winters");
            _report.WriteLine($"# Generated {DateTime.Now.ToString()}");
            _report.WriteLine();

            //checksums.Sort((a, b) => a.ToString().CompareTo(b.ToString()));
            //checksums.Sort((a, b) => a.Md5Checksum.CompareTo(b.Md5Checksum));

            checksums.Sort((a, b) => a.Pathname.CompareTo(b.Pathname));

            foreach (var checksum in checksums)
            {
                _report.WriteLine($"{checksum.Md5Checksum} *{checksum.Pathname}");
            }

            if (md5Files.Count > 0)
            {
                _report.WriteLine();

                // Verify MD5 file's chechsums against their related files that have been processed
                foreach (var fileChecksum in md5Files)
                {
                    // Find the related file for this MD5 file's checksum
                    var foundPosition = checksums.FindIndex(x =>
                      x.BaseFileName.Contains(fileChecksum.BaseFileName) && !IsMD5file(x));

                    if (foundPosition >= 0)
                    {
                        FileChecksum cs = checksums[foundPosition];
                        if (cs.Md5Checksum.Equals(fileChecksum.Md5Checksum, StringComparison.InvariantCultureIgnoreCase))
                        {
                            _report.WriteLine($"# OK - {cs.Md5Checksum} *{cs.Pathname}");
                        }
                        else
                        {
                            _report.WriteLine($"# BAD* - {cs.Md5Checksum} *{cs.Pathname}");
                        }
                    }
                }
            }

        }


        /// <summary>
        /// Verify MD5 file Checksums
        /// </summary>
        /// <param name="md5File">The MD5 file to verify</param>
        public static void VerifyMD5fileChecksums(FileInfo md5File)
        {
            // Initialize the report writer
            MiniLogger.MiniLogger _report = new MiniLogger.MiniLogger();

            string reportpathname = "";

            string reportSuffixChecking = ".Verify,Checking.txt";
            string reportSuffixOk = ".Verify.Good.txt";
            string reportSuffixBad = ".Verify.Bad.txt";

            bool allGoodSoFar = true;

            string dir = md5File.DirectoryName;

            List<FileChecksum> verifyChecksums = GetChecksumsFromMd5(md5File.FullName);

            if (verifyChecksums.Count > 0)
            {
                // Delete any existing results file
                reportpathname = Path.Combine(md5File.FullName.Substring(0, md5File.FullName.Length - ".md5".Length));
                if (File.Exists(reportpathname + reportSuffixChecking))
                {
                    File.Delete(reportpathname + reportSuffixChecking);
                }
                else if (File.Exists(reportpathname + reportSuffixOk))
                {
                    File.Delete(reportpathname + reportSuffixOk);
                }
                else if (File.Exists(reportpathname + reportSuffixBad))
                {
                    File.Delete(reportpathname + reportSuffixBad);
                }

                // Set the reports pathname to Checking
                reportpathname += reportSuffixChecking;

                _report.ConsolOut = Console.Out;
                _report.Add(reportpathname);

                // Report header
                _report.WriteLine("# MD5 checksums verified by MD5winters");
                _report.WriteLine($"# Generated {DateTime.Now.ToString()}");
                _report.WriteLine();
            }

            foreach (var cs in verifyChecksums)
            {
                string path;

                // Is the cs.Pathname in the same folder as the MD5 file?
                if (File.Exists(cs.Pathname))
                {
                    path = Path.Combine(dir, cs.Pathname);
                }
                else // No, the cs.Pathname be in a directory of the MD5 file base name
                {
                    // Convert cs.Pathname to a file under a folder 
                    path = Path.Combine(
                        md5File.FullName.Substring(0, md5File.FullName.Length - ".md5".Length), 
                        cs.Pathname);
                }

                if (File.Exists(path))
                {
                    Console.Title = $"MD5Winters - Verifying: {path}";

                    if (VerifyMd5(md5File, cs))
                    {
                        _report.WriteLine($"OK - {cs.Md5Checksum} *{cs.Pathname}");
                    }
                    else
                    {
                        allGoodSoFar = false; // found at least one bad apple
                        _report.WriteLine($"BAD* - {cs.Md5Checksum} *{cs.Pathname}");
                    }
                }
                else 
                {
                    _report.WriteLine($"Not Found - {cs.Md5Checksum} *{cs.Pathname}");
                }
            }
            _report.WriteLine();

            // Rename the report file with a Ok or Bad suffix
            FileInfo fi = new FileInfo(reportpathname);

            fi.MoveTo(fi.FullName.Replace(reportSuffixChecking,
                allGoodSoFar ? reportSuffixOk : reportSuffixBad));
        }

        // Move to class
        public static List<FileChecksum> GetChecksumsFromMd5(string pathname)
        {
            List<FileChecksum> checksums = new List<FileChecksum>();
                        
            using (StreamReader sr = new StreamReader(pathname))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    line.Trim();
                    // ignore blank lines and lines beginning with #
                    if ((line.Length == 0) || (line[0] == '#'))
                    {
                        continue;
                    }
                    // 198d9dd22ce67a6c792bc78fc2c22361 *MSTR_4.4.3.0.wim
                    // 198d9dd22ce67a6c792bc78fc2c22361 MSTR_4.4.3.0.wim
                    string[] parts = line.Split(' ');

                    string fileChecksum = parts[0];
                    fileChecksum = fileChecksum.Trim();
                    fileChecksum = fileChecksum.Trim('*');

                    string filePath = line.Substring(parts[0].Length);
                    filePath = filePath.Trim();
                    filePath = filePath.Trim('*');

                    FileChecksum checksum = new FileChecksum() 
                    { 
                        Md5Checksum = fileChecksum, 
                        FileName = filePath, 
                        Pathname = filePath 
                    };
                    checksums.Add(checksum);
                }
            }

            return checksums;
        }

        public static bool VerifyMd5(FileInfo md5File, FileChecksum checksum)
        {
            bool allGoodSoFar = false;

            string dir = md5File.DirectoryName;
            string path;

            // Is this an MD5 file for a file?
            if (File.Exists(checksum.Pathname))
            {
                path = Path.Combine(dir, checksum.Pathname);
            }
            else // No, for a directory
            {
                path = Path.Combine(md5File.FullName.Substring(0, md5File.FullName.Length - ".md5".Length), checksum.Pathname);
            }

            if (File.Exists(path))
            {
                Console.Title = $"MD5Winters - {path}";

                string targetChecksum = CreateMD5(path);
                allGoodSoFar = targetChecksum == checksum.Md5Checksum;
            }
            return allGoodSoFar;
        }

        public static string CreateMD5(string filepath)
        {
            string md5 = "";
            using (var md5Gen = MD5.Create())
            {
                using (var stream = File.OpenRead(filepath))
                {
                    md5 = md5Gen.ComputeHash(stream).ToHexString();
                }
            }

            return md5.ToLower(); // Match MD5summer output
        }

        public static bool IsMD5file(FileChecksum checksum)
        {
            return IsMD5file(checksum.FileInfo);
        }

        public static bool IsMD5file(FileInfo fi)
        {
            return IsMD5file(fi.Name);
        }

        public static bool IsMD5file(string file)
        {
            string type = file.Substring(file.Length - "MD5".Length);
            bool isMd5 = type.Equals("MD5", StringComparison.InvariantCultureIgnoreCase);

            return isMd5;
        }
    }
}
