using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;


namespace CSharpExtractComments
{
    internal class Program
    {
        static void Main(string[] args)
        {
            foreach (var arg in args) 
            {
                if (File.Exists(arg))
                {
                    var comments = ExtractComments(arg);

                    List<string> filtered = new List<string>();

                    foreach (var comment in comments)
                    {
                        if (
                            string.IsNullOrEmpty(comment) ||
                            comment.Contains("<summary>") ||
                            comment.Contains("</summary>") ||
                            comment.Contains("<returns>") ||
                            comment.Contains("</returns>") 
                            )
                        {
                            continue;
                        }
                        filtered.Add(comment);
                    }

                    string lines = string.Join(Environment.NewLine, filtered);

                    File.WriteAllText(arg+".txt", lines.ToString());
                }
                else
                {
                    Console.WriteLine($"File does not exist: {arg}");
                }
            }
        }

        public static List<string> ExtractComments(string filePath)
        {
            List<string> comments = new List<string>();

            try
            {
                string fileContent = File.ReadAllText(filePath);

                // Regular expression to match both single-line and multi-line comments
                string pattern = @"(//.*)|(/\*.*?\*/)";

                foreach (Match match in Regex.Matches(fileContent, pattern))
                {
                    var text = match.Value.Trim();
                    text = text.Trim(new char[] { '/', '*' } );
                    comments.Add(text = text.Trim());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error extracting comments: " + ex.Message);
            }

            return comments;
        }

    }
}
