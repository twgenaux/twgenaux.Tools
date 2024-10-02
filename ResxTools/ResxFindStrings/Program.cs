using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using tgenaux.ResxTools;

namespace ResxFindStrings
{
    internal class Program
    {
        static string rootPathname = "";

        static string outPathname = "";

        static List<string> names = new List<string>();

        static List<string> ProgramArgs = new List<string>();

        static FindResxFiles findResxFiles = new FindResxFiles();

        static void Main(string[] args)
        {
            if (!ParseArgs(args))
            {
                Usage();
            }
            else 
            {
                TBTs tBTs = new TBTs();                

                // Get list of all English Resx files under the root folder
                List<string> englishFiles = findResxFiles.FindAllEnglishResxFiles(new FileInfo(rootPathname).FullName);

                foreach (var resxFile in englishFiles)
                {
                    ResxStrings resxStrings = ResxHelper.ReadResxFile(resxFile);

                    foreach (var name in names)
                    {
                        if (resxStrings.ContainsKey(name))
                        {
                            TBT tbt = new TBT(); // To Be Translated

                            var resxString = resxStrings[name];

                            tbt.ID = resxString.Name;
                            tbt.Text = resxString.Value;
                            tbt.Comment = resxString.Comment;

                            FileInfo fileInfo = new FileInfo(resxFile);
                            DirectoryInfo di = fileInfo.Directory;
                            tbt.Sources.Add($"{di.Name}\\{fileInfo.Name}");

                            tBTs.Add( tbt );

                            Console.WriteLine(tbt.ID);
                            Console.WriteLine(tbt.Text);
                            Console.WriteLine(tbt.Comment);
                            Console.WriteLine(tbt.Sources);
                            Console.WriteLine();
                        }
                    }
                }

                tBTs.Merge();

                TBTSerializer.SerializeToXml(tBTs, outPathname);
            }
            Console.WriteLine();
        }



        static void Usage()
        {
            // Show usage
            string programName = Environment.GetCommandLineArgs()[0];
            FileInfo fi = new FileInfo(programName);

            Console.WriteLine("Compare Resx string files:");
            Console.WriteLine("Usage:");
            Console.WriteLine("  {0} [/show:Option] | ] [/out:reportfile] [/append] lPathname rPathname", fi.Name);
            Console.WriteLine();
            Console.WriteLine("   where lpathname and rPathname are both either directories or resx filenames.");
            Console.WriteLine("         lpathname - left path name");
            Console.WriteLine("         rPathname - right path name");
            Console.WriteLine("");
            Console.WriteLine("         /out:reportfile - write results to reportfile.");
            Console.WriteLine("         /append - append to report");
            Console.WriteLine("");
            Console.WriteLine("         /show:OnLeft  - Show string values found only on the left.");
            Console.WriteLine("         /show:OnRight - Show string values found only on the right.");
            Console.WriteLine("         /show:Unequal - Show string values that are different between left and right.");
            Console.WriteLine("         /show:Equal   - Show string values that are the same between left and right.");
            Console.WriteLine("         /show:FormatArgs  - Show string values that differ in format args.");
            Console.WriteLine("         /show:Dups    - Show string values for duplicated IDs.");
            Console.WriteLine("         /show:OnlyIDs - Show only string IDs (no values).");
            Console.WriteLine("         /show:English - Process only English files.");
            Console.WriteLine("");
        }

        static bool ParseArgs(string[] args)
        {
            // All files passed in are a list of program args
            // Add them to ProgramArgs list 
            foreach (var arg in  args) 
            { 
                if (File.Exists(arg))
                {
                    List<string> programArg = ReadProgramArgs(arg);
                    ProgramArgs.AddRange(programArg.ToList());
                }
            }

            foreach (var programArg in ProgramArgs)
            {
                if (programArg.IndexOf("help", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    return false; // returns false to display usage.
                }
                else if (File.Exists(programArg))
                {
                    // read list of search items from file and add to list of names
                }
                else if (programArg.IndexOf("/allfiles:", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    var temp = programArg.Remove(0, "/allfiles:".Length);
                    char[] trim = { '"' };
                    temp.Trim(new char[] { '"' });

                    if (!string.IsNullOrEmpty(temp))
                    {
                        findResxFiles.AllResxFilePatterns.Add(temp);
                    }
                }
                else if (programArg.IndexOf("/trans:", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    var temp = programArg.Remove(0, "/trans:".Length);
                    char[] trim = { '"' };
                    temp.Trim(new char[] { '"' });

                    if (!string.IsNullOrEmpty(temp))
                    {
                        findResxFiles.TranslatedFilePatterns.Add(temp);
                    }
                }
                else if (programArg.IndexOf("/lang:", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    var temp = programArg.Remove(0, "/lang:".Length);
                    char[] trim = { '"' };
                    temp.Trim(new char[] { '"' });

                    if (!string.IsNullOrEmpty(temp))
                    {
                        findResxFiles.EnglishFilePatterns.Add(temp);
                    }
                }
                else if (programArg.IndexOf("/src:", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    var temp = programArg.Remove(0, "/src:".Length);
                    char[] trim = { '"' };
                    temp.Trim(new char[] { '"' });

                    if (!string.IsNullOrEmpty(temp) && (File.Exists(temp) || Directory.Exists(temp)))
                    {
                        rootPathname = temp;
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine($"***Source (src) does not exist: {temp}");
                        Console.WriteLine();
                        return false;
                    }
                }
                else if (programArg.IndexOf("/out:", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    var temp = programArg.Remove(0, "/out:".Length);
                    char[] trim = { '"' };
                    temp.Trim(new char[] { '"' });

                    if (!string.IsNullOrEmpty(temp))
                    {
                        outPathname = temp;
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine($"***Invalid arg: {programArg}");
                        Console.WriteLine();
                        return false;
                    }
                }
                else
                {
                    names.Add(programArg); // add search items
                }
            }

            return true;
        }

        static List<string> ReadProgramArgs(string pathname)
        {
            List<string> programArgs = new List<string>();

            if (File.Exists(pathname))
            {
                int lineNumber = 0;
                string line;

                using (var sr = new StreamReader(pathname))
                {
                    while (null != (line = sr.ReadLine()))
                    {
                        lineNumber++;

                        // Remove comment
                        if (line.Contains('#'))
                        {
                            line = line.Substring(0, line.IndexOf("#"));
                        }

                        // Remove leading and trailing whitespace
                        line = line.Trim();

                        // Do not add blank lines
                        if (!string.IsNullOrEmpty(line))
                        {
                            programArgs.Add(line);
                        }
                    }
                }
            }
            return programArgs;
        }

    }
}
