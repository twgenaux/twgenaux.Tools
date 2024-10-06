using ResxFindStrings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        /// Directory of Resx files
        /// </summary>
        public string RootPathname { get; set; }

        /// <summary>
        /// To-Be-Translated XMLoutput pathname
        /// </summary>
        public string OutPathname { get; set; }

        /// <summary>
        /// Resx IDs (names) to find
        /// </summary>
        public List<string> Names { get; set; }

        /// <summary>
        /// File pattern for finding all Resx files
        /// </summary>
        public string AllResxFilePattern
        {
            set
            {
                FindResxFiles.AllResxFilePatterns.Add(value);
            }
        }

        /// <summary>
        /// File pattern for finding all translated Resx files
        /// </summary>
        public string TranslatedFilePattern
        {
            set
            {
                FindResxFiles.TranslatedFilePatterns.Add(value);
            }
        }

        /// <summary>
        /// File pattern for finding the langaue files Resx files
        /// </summary>
        public string NoCodeFilePattern
        {
            set
            {
                FindResxFiles.FilePatterns.Add(value);
            }
        }

        /// <summary>
        /// 
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
        /// Returns true if enough properites have been defined
        /// </summary>
        /// <returns></returns>
        public bool Ready()
        {
            bool goodSoFar = Directory.Exists(RootPathname);

            goodSoFar = goodSoFar && (FindResxFiles.FilePatterns.Count > 0) ||
                (FindResxFiles.AllResxFilePatterns.Count > 0 && FindResxFiles.TranslatedFilePatterns.Count > 0);

            goodSoFar = goodSoFar && (Names.Count > 0);

            return goodSoFar;

        }

        /// <summary>
        /// Find Resx strings
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
                        DirectoryInfo di = fileInfo.Directory;
                        tbt.Sources.Add($"{di.Name}\\{fileInfo.Name}");

                        TbtStrings.Add(tbt);
                    }
                }
            }
        }
    }
}
