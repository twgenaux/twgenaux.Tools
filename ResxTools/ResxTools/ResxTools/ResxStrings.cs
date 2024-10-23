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

namespace tgenaux.ResxTools
{
    /// <summary>
    /// Collection of ResxString(s)
    /// </summary>
    public class ResxStrings : Dictionary<String, ResxString>
    {
        public ResxStrings(ResxStrings rss)
            : base(rss)
        {
        }

        public ResxStrings()
        {
        }


        public void AddOrReplace(ResxStrings src)
        {
            foreach (var rs in src)
            {
                this[rs.Key] = src[rs.Key];
            }
        }

        public void AddOrReplace(List<string> ids, ResxStrings src)
        {
            foreach (var id in ids)
            {
                this[id] = src[id];
            }
        }


        public void AddOrReplace(string name, string value, string comment="")
        {
            AddOrReplace(new ResxString() { Name = name, Value = value, Comment = comment });
        }

        public ResxStrings AddOrReplace(params ResxString[] strings)
        {
            foreach (var rs in strings)
            {
                this[rs.Name] = rs;
            }

            return this;
        }
    }
}
