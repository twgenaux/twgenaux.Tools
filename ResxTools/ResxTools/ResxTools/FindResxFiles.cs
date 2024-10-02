using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tgenaux.ResxTools
{
    public class FindResxFiles
    {
        // TODO: Make list
        public string AllResxFilePattern { get; set; }

        // TODO: Make list
        public string TranslatedFilePattern { get; set; }

        // TODO: Make list
        public string EnglishFilePattern { get; set; }

        public FindResxFiles()
        {
            AllResxFilePattern = "*.resx";
            TranslatedFilePattern = "*.??*.resx";
        }

        public List<string> FindAllEnglishResxFiles(string root, SearchOption serchOption = SearchOption.AllDirectories)
        {
            List<string> englishFiles = new List<string>();

            if (!string.IsNullOrEmpty(EnglishFilePattern))
            {
                englishFiles = FindResxFiles.FindAllResxFiles(root, EnglishFilePattern, serchOption);
            }

            else
            {
                // Find all files and all translated files
                string rootFullName = new DirectoryInfo(root).FullName;
                List<string> allFiles = FindAllResxFiles(rootFullName, serchOption);
                List<string> transFiles = FindResxFiles.FindAllResxFiles(rootFullName, TranslatedFilePattern, serchOption);

                // Add all files except for the translated files
                englishFiles.AddRange(allFiles.Except(transFiles).ToList());
            }

            englishFiles.Sort();

            return englishFiles;
        }

        public List<string> FindAllResxFiles(string root, SearchOption serchOption = SearchOption.AllDirectories)
        {
            List<string> found = FindResxFiles.FindAllResxFiles(root, AllResxFilePattern, serchOption);

            return found;
        }

        private static List<string> FindAllResxFiles(string root, string pattern, SearchOption serchOption = SearchOption.AllDirectories)
        {
            List<string> found = new List<string>();

            DirectoryInfo sourceDir = new DirectoryInfo(root);
            FileInfo[] files = sourceDir.GetFiles(pattern, serchOption);
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
