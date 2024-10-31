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
using tgenaux.ResxTools;

namespace ResxFindStrings
{
    /// <summary>
    /// Command line program for finding Resx strings for translation
    /// </summary>
    internal class ResxFindStringsProgram
    {
        /// <summary>
        /// Program arguments
        /// </summary>
        static List<string> ProgramArgs = new List<string>();

        /// <summary>
        /// Resx file patterns
        /// </summary>
        static FindResxFiles findResxFiles = new FindResxFiles();

        /// <summary>
        /// The tool that finds the strings
        /// </summary>
        static FindResxStrings findResxStrings = new FindResxStrings();

        static void Main(string[] args)
        {
            // Parsing args configures the findResxStrings tool
            if (!ParseArgs(args))
            {
                Usage();
            }
            else 
            {
                // Find the strings
                findResxStrings.FindStrings();

                // Merge equivalent strings
                findResxStrings.TbtStrings.MergeEquivalent();

                // List the strings on the console
                foreach (var tbt in findResxStrings.TbtStrings.ToBeTranslated)
                {
                    Console.WriteLine(tbt.ID);
                    Console.WriteLine(string.Join("\n", tbt.Sources));
                    Console.WriteLine();
                }

                foreach (var id in findResxStrings.Names)
                {
                    if (!findResxStrings.TbtStrings.ToBeTranslated.Any(tbt => tbt.ID == id))
                    {
                        Console.WriteLine($"*** Did not find: {id}");
                    }
                }
                Console.WriteLine();

                // Save the To-Be-Translated stings
                TBTSerializer.SerializeToXml(findResxStrings.TbtStrings, findResxStrings.OutPathname);
            }
            Console.WriteLine();
        }

        static void Usage()
        {
            string programName = Environment.GetCommandLineArgs()[0];
            FileInfo fi = new FileInfo(programName);

            Console.WriteLine("Finds Resx strings for translation");
            Console.WriteLine("Usage:");
            Console.WriteLine("  {0} args", fi.Name);
            Console.WriteLine("");
            Console.WriteLine("  Program args:");
            Console.WriteLine("");
            Console.WriteLine("/? - This help");
            Console.WriteLine("");
            Console.WriteLine("/allfiles:patern - File pattern  for finding all Resx files (*.resx)");
            Console.WriteLine("");
            Console.WriteLine("/trans:patern - File pattern for finding all translated Resx files (*.??*.resx)");
            Console.WriteLine("");
            Console.WriteLine("/lang:patern - A file pattern for language-specific codes for finding target files (*.en.resx)");
            Console.WriteLine("");
            Console.WriteLine("/src:folder - Root folder where all Resx files reside");
            Console.WriteLine("");
            Console.WriteLine("/out:pathname - Output path for the To-Be-Translated XML report file");
            Console.WriteLine("");
            Console.WriteLine("ID - One or more unique Resx string IDs (name)");
            Console.WriteLine("");
            Console.WriteLine("Argslist.txt - A program arguments file that lists one or more command line arguments.");
            Console.WriteLine(" - The file name can be any name as long as it does not have a Resx file extension. ");
            Console.WriteLine(" - Each line is treated as a command line arg.");
            Console.WriteLine(" - Comment (#) and blank lines are ignored.");
            Console.WriteLine(" - End-of-Line comments are removed.");
            Console.WriteLine(" - All lines are trimmed to remove leading and trailing spaces.");
            Console.WriteLine("");
            Console.WriteLine("Windows Wildcard File Search");
            Console.WriteLine(" - Asterisk (*): Matches any number of characters, including zero.");
            Console.WriteLine(" - Question mark (?): Matches a single character.");
            Console.WriteLine("");
        }

        /// <summary>
        /// Parse command line arguments
        /// </summary>
        /// <param name="args">command line arguments</param>
        /// <returns>Returns true if args are valid</returns>
        static bool ParseArgs(string[] args)
        {
            bool goodSoFar = true;

            // All files passed in are a list of program args
            // Add them to ProgramArgs list 
            foreach (var arg in  args) 
            { 
                if (File.Exists(arg))
                {
                    List<string> programArg = ReadProgramArgsFile(arg);
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
                        Console.WriteLine($"***Source (src) does not exist: {programArg}");
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
                        Console.WriteLine($"***Invalid arg: {programArg}");
                        Console.WriteLine();
                        goodSoFar = false;
                    }
                }
                else if (programArg.IndexOf("/id:", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    var value = GetArgValue("/id:", programArg);

                    if (!string.IsNullOrEmpty(value))
                    {
                        findResxStrings.Names.Add(value); // add search item
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine($"***Invalid arg: {programArg}");
                        Console.WriteLine();
                        goodSoFar = false;
                    }
                }
                else if (File.Exists(programArg))
                {
                    // Ignore program args file
                }
                else
                {
                    //findResxStrings.Names.Add(programArg); // add search items
                    Console.WriteLine();
                    Console.WriteLine($"***Invalid arg: {programArg}");
                    Console.WriteLine();
                    goodSoFar = false;
                }

                if (!goodSoFar)
                {
                    return goodSoFar;
                }
            }

            return findResxStrings.Ready();
        }

        /// <summary>
        /// Returns a program argument's value.
        /// </summary>
        /// <param name="argType"></param>
        /// <param name="programArg"></param>
        /// <returns></returns>
        static string GetArgValue(string argType, string programArg)
        {
            var value = programArg.Remove(0, argType.Length);
            char[] trim = { '"' };
            value.Trim(new char[] { '"' });

            return value;
        }

        /// <summary>
        /// Reads a program args file
        /// </summary>
        /// <param name="pathname"></param>
        /// <returns>List of program args</returns>
        static List<string> ReadProgramArgsFile(string pathname)
        {
            List<string> programArgs = new List<string>();

            if (File.Exists(pathname))
            {
                string[] lines = File.ReadAllLines(pathname);

                programArgs = ParseProgramArgs(lines);
            }

            return programArgs;
        }

        /// <summary>
        /// Reads program args
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static List<string> ParseProgramArgs(string[] args)
        {
            List<string> programArgs = new List<string>();

            int lineNumber = 0;
            foreach (var arg in args)
            {
                string line = arg;

                lineNumber++;

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

                programArgs.Add(line);
            }
            return programArgs;
        }
    }
}
