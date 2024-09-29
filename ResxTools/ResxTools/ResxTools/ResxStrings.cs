using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Copyright @ 2015-2023 Theron W. Genaux
// See "ResX-Tools Readme.md" for license.

namespace tgenaux.ResxTools
{
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
