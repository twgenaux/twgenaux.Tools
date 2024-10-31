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

using System.Collections.Generic;
using System.IO;
using tgenaux.ResxTools;

namespace ResxFindStrings
{
    public class FindResxStrings
    {
        /// <summary>
        /// To-Be-Translated strings
        /// </summary>
        public ToBeTranslatedStrings TbtStrings { get; }

        /// <summary>
        /// Root folder containing all Resx files
        /// </summary>
        public string RootPathname { get; set; }

        /// <summary>
        /// To-Be-Translated XML output pathname
        /// </summary>
        public string OutPathname { get; set; }

        /// <summary>
        /// List Resx IDs (names) to find
        /// </summary>
        public List<string> Names { get; set; }

        /// <summary>
        /// File patterns for finding all Resx files
        /// </summary>
        public string AllResxFilePattern
        {
            set
            {
                FindResxFiles.AllResxFilePatterns.Add(value);
            }
        }

        /// <summary>
        /// File patterns for finding all translated files
        /// </summary>
        public string TranslatedFilePattern
        {
            set
            {
                FindResxFiles.TranslatedFilePatterns.Add(value);
            }
        }

        /// <summary>
        /// List of specific files patterns to find
        ///   If the translation source files are English, 
        ///   one might use the file pattern: *.en.resx
        /// </summary>
        public string FilePattern
        {
            set
            {
                FindResxFiles.FilePatterns.Add(value);
            }
        }

        /// <summary>
        /// Finds all translation source files (untranslated files)
        /// </summary>
        private FindResxFiles FindResxFiles { get; set; }

        public FindResxStrings()
        {
            TbtStrings = new ToBeTranslatedStrings();
            RootPathname = "";
            OutPathname = "";
            Names = new List<string>();
            FindResxFiles = new FindResxFiles();
        }

        /// <summary>
        /// Returns true if the minimum required properties have been defined
        /// </summary>
        /// <returns>true if the minimum required properties have been defined</returns>
        public bool Ready()
        {
            bool goodSoFar = Directory.Exists(RootPathname) && !string.IsNullOrEmpty(OutPathname);

            goodSoFar = goodSoFar && (FindResxFiles.FilePatterns.Count > 0) ||
                (FindResxFiles.AllResxFilePatterns.Count > 0 && FindResxFiles.TranslatedFilePatterns.Count > 0);

            goodSoFar = goodSoFar && (Names.Count > 0);

            return goodSoFar;

        }

        /// <summary>
        /// Find the Resx strings
        /// </summary>
        public void FindStrings()
        {
            List<string> resxFiles = FindResxFiles.FindFiles(new FileInfo(RootPathname).FullName);

            foreach (var resxFile in resxFiles)
            {
                ResxStrings resxStrings = ResxHelper.ReadResxFile(resxFile);

                foreach (var name in Names)
                {
                    if (resxStrings.ContainsKey(name))
                    {
                        ToBeTranslated tbt = new ToBeTranslated();

                        var resxString = resxStrings[name];

                        tbt.ID = resxString.Name;
                        tbt.Text = resxString.Value;
                        tbt.Comment = resxString.Comment;

                        FileInfo fileInfo = new FileInfo(resxFile);
                        string sourcePath = fileInfo.FullName.Substring(this.RootPathname.Length);
                        tbt.Sources.Add(sourcePath);

                        TbtStrings.Add(tbt);
                    }
                }
            }
        }
    }
}
