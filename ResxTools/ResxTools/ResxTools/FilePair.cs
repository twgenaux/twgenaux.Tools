using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Copyright @ 2015-2023 Theron W. Genaux
// See "ResX-Tools Readme.md" for license.

namespace tgenaux.ResxTools
{
    /// <summary>
    /// Associated files to be compared
    /// </summary>
    public class FilePair : IComparable
    {
        public string LeftFilename { get; set; }
        public string RightFilename { get; set; }
       
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            FilePair fp = (FilePair)obj;
            if (fp != null)
            {
                return String.Compare(this.ToString(), fp.ToString(), true);
            }
            else
            {
                throw new ArgumentException("Object is not a FilePair");
            }
        }

        public override string ToString()
        {
            return LeftFilename + "," + RightFilename;
        }

    }
}
