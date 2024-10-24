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
using System.Linq;

namespace tgenaux.ResxTools
{
    /// <summary>
    /// FindResxFiles
    /// 
    /// Finds files for translation
    /// </summary>
    public class FindResxFiles
    {
        /// <summary>
        /// File patterns for finding all Resx files
        /// </summary>
        public List<string> AllResxFilePatterns { get; set; }

        /// <summary>
        /// File patterns for finding all translated files
        /// </summary>
        public List<string> TranslatedFilePatterns { get; set; }

        /// <summary>
        /// List of specific files patterns to find
        ///   If the translation source files are English, 
        ///   one might use the file pattern: *.en.resx
        /// </summary>
        public List<string> FilePatterns { get; set; }

        public FindResxFiles()
        {
            AllResxFilePatterns = new List<string>();
            TranslatedFilePatterns = new List<string>();
            FilePatterns = new List<string>();
        }

        /// <summary>
        /// Find matching Resx files
        /// </summary>
        /// <param name="root">Root folder containing all Resx files</param>
        /// <param name="serchOption">DirectoryInfo.GetFiles SearchOption</param>
        /// <returns> Returns files matching the file patterns</returns>
        public List<string> FindFiles(string root, SearchOption serchOption = SearchOption.AllDirectories)
        {
            List<string> filesFound = new List<string>();

            // Find specific language files
            if (FilePatterns.Count > 0)
            {
                filesFound = FindResxFiles.FindAllResxFiles(root, FilePatterns, serchOption);
            }

            else // Find translation source files by returning the differernce between the set of all files and the set of all translated files.
            {
                // Find all files and all translated files
                string rootFullName = new DirectoryInfo(root).FullName;
                List<string> allFiles = FindAllResxFiles(rootFullName, serchOption);
                List<string> transFiles = FindResxFiles.FindAllResxFiles(rootFullName, TranslatedFilePatterns, serchOption);

                // Add all files except for the translated files
                filesFound.AddRange(allFiles.Except(transFiles).ToList());
            }

            filesFound.Sort();

            return filesFound;
        }

        /// <summary>
        /// Find all Resx files
        /// </summary>
        /// <param name="root">Root folder containing all Resx files</param>
        /// <param name="serchOption">DirectoryInfo.GetFiles SearchOption</param>
        /// <returns> Returns all Resx files</returns>
        public List<string> FindAllResxFiles(string root, SearchOption serchOption = SearchOption.AllDirectories)
        {
            List<string> found = FindResxFiles.FindAllResxFiles(root, AllResxFilePatterns, serchOption);

            return found;
        }


        /// <summary>
        /// Finds all Resx files that match the file patterns
        /// </summary>
        /// <param name="root">Root folder containing all Resx files</param>
        /// <param name="serchOption">DirectoryInfo.GetFiles SearchOption</param>
        /// <returns>Returns files matching the file patterns</returns>
        private static List<string> FindAllResxFiles(string root, List<string> patterns, 
            SearchOption serchOption = SearchOption.AllDirectories)
        {
            List<string> found = new List<string>();

            DirectoryInfo sourceDir = new DirectoryInfo(root);

            foreach (var pattern in patterns)
            {
                FileInfo[] files = sourceDir.GetFiles(pattern, serchOption);
                foreach (var file in files)
                {
                    string filename = file.FullName;
                    found.Add(filename);
                }
            }

            found.Sort();

            return found;
        }
    }
}
