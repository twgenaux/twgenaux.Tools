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
using System.Collections;
using System.ComponentModel.Design;
using System.IO;
using System.Resources;

namespace tgenaux.ResxTools
{
    public static class ResxHelper
    {
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

                    if (node.GetValue(typeres) is string)
                    {
                        String name = node.Name;
                        String value = (string)node.GetValue(typeres);
                        String comment = node.Comment;

                        resxDict.AddOrReplace(name, value, comment);
                    }
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
    }
}
