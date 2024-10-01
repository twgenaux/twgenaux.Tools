using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResxFindStrings
{
    public class FindResxFiles
    {
        // TODO: Make list
        public string AllResxFilePattern { get; set; }

        // TODO: Make list
        public string TranslatedFilePattern { get; set; }

        public FindResxFiles()
        {
            AllResxFilePattern = "*.resx";
            TranslatedFilePattern = "*.??*.resx";
        }

        public List<string> FindAllEnglishResxFiles(string root, SearchOption serchOption = SearchOption.AllDirectories)
        {
            List<string> englishFiles = new List<string>();

            DirectoryInfo sourceDir = new DirectoryInfo(root);

            List<string> allFiles = FindAllResxFiles(sourceDir.FullName, AllResxFilePattern, serchOption);
            List<string> transFiles = FindAllResxFiles(sourceDir.FullName, TranslatedFilePattern, serchOption);
            englishFiles.AddRange(allFiles.Except(transFiles).ToList());

            englishFiles.Sort();

            return englishFiles;
        }

        public List<string> FindAllResxFiles(string root, string pattern, SearchOption serchOption = SearchOption.AllDirectories)
        {
            List<string> found = new List<string>();

            DirectoryInfo sourceDir = new DirectoryInfo(root);
            FileInfo[] files = sourceDir.GetFiles(pattern, serchOption);  //Get only files which you need to work with.
            foreach (var file in files)
            {
                string filename = file.FullName;
                found.Add(filename);
            }

            found.Sort();

            return found;
        }

    }
}
