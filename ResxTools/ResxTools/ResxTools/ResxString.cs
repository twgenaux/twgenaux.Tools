using System;

// Copyright @ 2015-2023 Theron W. Genaux
// See "ResX-Tools Readme.md" for license.

namespace tgenaux.ResxTools
{
    public class ResxString
    {
        public string Name { get; set; }
        public string Value { get; set; }
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
            bool equals = 
                !(obj == null || this.GetType() != obj.GetType());
            
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
