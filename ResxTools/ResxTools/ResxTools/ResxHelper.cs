using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Resources;
using System.Xml.Linq;

// Copyright @ 2015-2023 Theron W. Genaux
// See "ResX-Tools Readme.md" for license.

namespace tgenaux.ResxTools
{
    public static class ResxHelper
    {
        static public List<FilePair> MatchEngFilesToTrans(string path)
        {
            List<FilePair> pairedFiles = new List<FilePair>();

            DirectoryInfo di = new DirectoryInfo(path);

            FindResxFiles findResxFiles = new FindResxFiles();

            List<string> allFiles = findResxFiles.FindAllResxFiles(di.FullName);
            List<string> transFiles = findResxFiles.FindAllResxFiles(di.FullName);
            List<string> englishFiles = allFiles.Except(transFiles).ToList();

            // Pair the English (root) files with each of their translations
            foreach (var root in englishFiles)
            {
                List<FilePair> rootFiles = FindTranslatedFiles(root);

                pairedFiles.AddRange(rootFiles);
            }
            return pairedFiles;
        }

        static public List<FilePair> FindTranslatedFiles(string englishFilename)
        {
            List<FilePair> pairedFiles = new List<FilePair>();

            FindResxFiles findResxFiles = new FindResxFiles();

            FileInfo fi = new FileInfo(englishFilename);

            string name = fi.Name;
            string extension = fi.Extension;
            DirectoryInfo folder = fi.Directory;

            string target = name.Substring(0, name.Length - extension.Length);
            target = target.Replace("\\", "\\\\");
            target = target.Replace(".", "\\.");
            List<string> relatedFiles = findResxFiles.FindAllResxFiles(folder.FullName, SearchOption.AllDirectories);

            foreach (var relatedFile in relatedFiles)
            {
                FilePair fp = new FilePair()
                {
                    LeftFilename = fi.FullName,
                    RightFilename = relatedFile,
                };

                pairedFiles.Add(fp);
            }

            return pairedFiles;
        }

        static public List<string> FindDuplicatesResxStringIds(String filename)
        {
            List<string> resxIds = new List<string>();
            List<string> dupIds = new List<string>();

            FileInfo fi = new FileInfo(filename);
            XDocument xdoc = XDocument.Load(fi.FullName);

            XElement rootElement = xdoc.Element("root");

            var elements =
            from elm in rootElement.Descendants("data")
            select new
            {
                root = elm,
                attrs = elm.Attributes(),
                elems = elm.Elements(),

                name = elm.Attribute("name").Value,
            };

            foreach (var element in elements)
            {
                var values = element.root.Descendants("value");
                var comments = element.root.Descendants("comment");

                ResxString rs = new ResxString()
                {
                    Name = element.name,
                    Value = (values.Count() > 0) ? values.FirstOrDefault().Value : "",
                    Comment = (comments.Count() > 0) ? comments.FirstOrDefault().Value : "",
                };

                string id = element.name;
                if (resxIds.Contains(id))
                {
                    dupIds.Add(id);
                }
                else
                {
                    resxIds.Add(id);
                }
            }

            return dupIds;
        }

        /// <summary>
        ///  ReadResxFile - Resx string resource XML file reader
        ///  If Resx file contains duplicate IDs , only one is retained. 
        ///  Tests indicate that only the last one read is retained.
        ///  This is a side effect of the ResXResourceReader and not the use of ResxStrings.
        ///  True a dictionary would only keep one entry, but only one value is retrieved by ResXResourceReader.
        /// </summary>
        /// <param name="filename">Resx file name</param>
        /// <returns>ResxStrings containing the ResxStrings</returns>
        static public ResxStrings ReadResxFile(String filename)
        {
            ResxStrings resxDict = new ResxStrings();

            using (ResXResourceReader resxReader = new ResXResourceReader(filename))
            {
                resxReader.UseResXDataNodes = true;

                IDictionaryEnumerator dict = resxReader.GetEnumerator();

                while (dict.MoveNext())
                {
                    ResXDataNode node = (ResXDataNode)dict.Value;
                    ITypeResolutionService typeres = null;

                    String name = node.Name;
                    String value = (node.GetValue(typeres)).ToString();
                    String comment = node.Comment;

                    resxDict.AddOrReplace(name, value, comment);
                }
            }

            return resxDict;
        }

        /// <summary>
        /// WriteResxFile - Resx string resource XML file writer
        /// </summary>
        /// <param name="resxDict">ResxStrings containing the ResxStrings</param>
        /// <param name="filename">Resx file name</param>
        static public void WriteResxFile(ResxStrings resxDict, String filename)
        {
            FileInfo backup = new FileInfo(filename + ".bak");
            if (backup.Exists)
            {
                backup.IsReadOnly = false;
                backup.Delete();
            }

            FileInfo info = new FileInfo(filename);
            if (info.Exists)
            {
                info.IsReadOnly = false;
                info.MoveTo(filename + ".bak");
            }

            try
            {
                using (ResXResourceWriter resxWriter = new ResXResourceWriter(filename))
                {
                    foreach (var pair in resxDict)
                    {
                        ResxString te = pair.Value;
                        if (te.Comment != String.Empty)
                        {
                            resxWriter.AddResource(new ResXDataNode(te.Name, te.Value) { Comment = te.Comment });
                        }
                        else
                        {
                            resxWriter.AddResource(new ResXDataNode(te.Name, te.Value));
                        }

                    }
                }
            }
            catch (Exception e)
            {
                if (backup.Exists)
                {
                    backup.IsReadOnly = false;
                    backup.MoveTo(filename);
                }
                throw;
            }

            backup = new FileInfo(filename + ".bak");
            if (backup.Exists)
            {
                backup.IsReadOnly = false;
                backup.Delete();
            }
        }




    } // ResxHelper
}
