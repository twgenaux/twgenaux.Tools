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

        static FindResxStrings findResxStrings = new FindResxStrings();

        static void Main(string[] args)
        {
            if (!ParseArgs(args))
            {
                Usage();
            }
            else 
            {
                findResxStrings.FindStrings();

                findResxStrings.TbtStrings.MergeEquivalent();

                foreach (var tbt in findResxStrings.TbtStrings.ToBeTranslated)
                {
                    Console.WriteLine(tbt.ID);
                    Console.WriteLine(string.Join("\n", tbt.Sources));
                    Console.WriteLine();
                }

                TBTSerializer.SerializeToXml(findResxStrings.TbtStrings, findResxStrings.OutPathname);
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
            bool goodSoFar = true;

            // All files passed in are a list of program args
            // Add them to ProgramArgs list 
            foreach (var arg in  args) 
            { 
                if (File.Exists(arg))
                {
                    List<string> programArg = ReadProgramArgs(arg);
                    ProgramArgs.AddRange(programArg.ToList());
                }
                else
                {
                    ProgramArgs.Add(arg);
                }
            }

            foreach (var programArg in ProgramArgs)
            {
                if ((programArg.IndexOf("help", StringComparison.CurrentCultureIgnoreCase) >= 0) ||
                    (programArg.IndexOf("/help", StringComparison.CurrentCultureIgnoreCase) >= 0) ||
                    (programArg.IndexOf("/?", StringComparison.CurrentCultureIgnoreCase) >= 0)
                    )
                {
                        goodSoFar = false; // returns false to display usage.
                }
                else if (programArg.IndexOf("/allfiles:", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    var value = GetArgValue("/allfiles:", programArg);

                    if (!string.IsNullOrEmpty(value))
                    {
                        findResxStrings.AllResxFilePattern = value;
                    }
                }
                else if (programArg.IndexOf("/trans:", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    var value = GetArgValue("/trans:", programArg);

                    if (!string.IsNullOrEmpty(value))
                    {
                        findResxStrings.TranslatedFilePattern = value;
                    }
                }
                else if (programArg.IndexOf("/lang:", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    var value = GetArgValue("/lang:", programArg);

                    if (!string.IsNullOrEmpty(value))
                    {
                        findResxFiles.FilePatterns.Add(value);
                        findResxStrings.FilePattern = value;
                    }
                }
                else if (programArg.IndexOf("/src:", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    var value = GetArgValue("/src:", programArg);

                    if (!string.IsNullOrEmpty(value) && Directory.Exists(value))
                    {
                        findResxStrings.RootPathname = value;
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine($"***Source (src) does not exist: {value}");
                        Console.WriteLine();
                        goodSoFar = false;
                    }
                }
                else if (programArg.IndexOf("/out:", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    var value = GetArgValue("/out:", programArg);

                    if (!string.IsNullOrEmpty(value))
                    {
                        findResxStrings.OutPathname = value;
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine($"***Invalid arg: {value}");
                        Console.WriteLine();
                        goodSoFar = false;
                    }
                }
                else
                {
                    findResxStrings.Names.Add(programArg); // add search items
                }

                if (!goodSoFar)
                {
                    return goodSoFar;
                }
            }

            return findResxStrings.Ready();
        }

        static string GetArgValue(string argType, string programArg)
        {
            var value = programArg.Remove(0, argType.Length);
            char[] trim = { '"' };
            value.Trim(new char[] { '"' });

            return value;
        }

        static List<string> ReadProgramArgs(string pathname)
        {
            List<string> programArgs = new List<string>();

            if (File.Exists(pathname))
            {
                string[] lines = File.ReadAllLines(pathname);

                programArgs = ReadProgramArgs(lines);
            }

            return programArgs;
        }
        static List<string> ReadProgramArgs(string[] args)
        {
            List<string> programArgs = new List<string>();

            int lineNumber = 0;
            foreach (var arg in args)
            {
                string line = arg;

                lineNumber++;

                // Ingore Markdonwn code fence
                if (line.Contains("```"))
                {
                    continue;
                }

                // Remove comment
                if (line.Contains('#'))
                {
                    line = line.Substring(0, line.IndexOf("#"));
                }

                line = line.Trim();

                // Do not add blank lines
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                // Remove leading and trailing Markdonwn code
                line = line.Trim(new char[] { '`' });

                programArgs.Add(line);
            }
            return programArgs;
        }
    }
}
