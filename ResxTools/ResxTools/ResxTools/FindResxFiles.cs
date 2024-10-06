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
        public List<string> AllResxFilePatterns { get; set; }

        // TODO: Make list
        public List<string> TranslatedFilePatterns { get; set; }

        // TODO: Make list
        public List<string> FilePatterns { get; set; }

        public FindResxFiles()
        {
            AllResxFilePatterns = new List<string>();
            TranslatedFilePatterns = new List<string>();
            FilePatterns = new List<string>();
        }

        public List<string> FindFiles(string root, SearchOption serchOption = SearchOption.AllDirectories)
        {
            List<string> noCodeFiles = new List<string>();

            // Do NoCode files include a language code?
            if (FilePatterns.Count > 0)
            {
                noCodeFiles = FindResxFiles.FindAllResxFiles(root, FilePatterns, serchOption);
            }

            else // Find all NoCode files base on the  differernce between the sets of all files and all translated files.
            {
                // Find all files and all translated files
                string rootFullName = new DirectoryInfo(root).FullName;
                List<string> allFiles = FindAllResxFiles(rootFullName, serchOption);
                List<string> transFiles = FindResxFiles.FindAllResxFiles(rootFullName, TranslatedFilePatterns, serchOption);

                // Add all files except for the translated files
                noCodeFiles.AddRange(allFiles.Except(transFiles).ToList());
            }

            noCodeFiles.Sort();

            return noCodeFiles;
        }

        public List<string> FindAllResxFiles(string root, SearchOption serchOption = SearchOption.AllDirectories)
        {
            List<string> found = FindResxFiles.FindAllResxFiles(root, AllResxFilePatterns, serchOption);

            return found;
        }

        private static List<string> FindAllResxFiles(string root, List<string> patterns, SearchOption serchOption = SearchOption.AllDirectories)
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
