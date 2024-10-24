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

namespace tgenaux.ResxTools
{
    /// <summary>
    /// Container for a Resx string attributes
    /// </summary>
    public class ResxString
    {
        /// <summary>
        /// Name Identifier
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// String value (text)
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Comment
        /// </summary>
        public string Comment { get; set; }

        public override string ToString()
        {
            return string.Format("{0}{1}{2}", Name, Value, Comment);
        }

        public static bool operator ==(ResxString a, ResxString b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ResxString a, ResxString b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            bool equals = // is obj a ResxString type?
                !(obj == null || this.GetType() != obj.GetType());

            // Are the two ResxString equal?
            if (equals)
            {
                ResxString rs = (ResxString)obj;

                equals =
                    String.Equals(this.Name, rs.Name, StringComparison.Ordinal) &&
                    String.Equals(this.Value, rs.Value, StringComparison.Ordinal) &&
                    String.Equals(this.Comment, rs.Comment, StringComparison.Ordinal);
            }

            return equals;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}
