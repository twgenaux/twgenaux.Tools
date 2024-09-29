using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.Linq;


namespace ResxFindStrings
{
    [XmlRoot("TBTs")]
    public class TBTs
    {
        [XmlElement("TBT")]
        public List<TBT> ToBeTranslated { get; set; }

        public TBTs()
        {
            ToBeTranslated = new List<TBT>();
        }

        public void Add(TBT tbt) 
        { 
            ToBeTranslated.Add(tbt);
        }

        public void Merge()
        {
            TBTs sortedTBTS = new TBTs();
            sortedTBTS.ToBeTranslated = 
                ToBeTranslated.OrderBy(x => x.ID).ThenBy(x => x.Text).ThenBy(x => x.Comment).ToList();

            TBTs mergedTBTS = new TBTs();

            TBT current = null;
            foreach (var tbt in sortedTBTS.ToBeTranslated)
            {
                // Compare current to tbt, ignoring sources
                if (current == null)
                {
                    current = tbt;
                }
                else if (0 == Equivalent(current, tbt))
                {
                    current.Sources.AddRange(tbt.Sources);
                }
                else
                {
                    // add current to mergedTBTS
                    mergedTBTS.Add(current);
                    current = tbt;
                }
            }
            ToBeTranslated = mergedTBTS.ToBeTranslated;
        }

        static private int Equivalent(TBT x, TBT y)
        {
            int result = 0;

            result = x.ID.CompareTo(y.ID);
            if (result != 0)
            {
                return result;
            }
            result = x.Text.CompareTo(y.Text);
            if (result != 0)
            {
                return result;
            }
            result = x.Comment.CompareTo(y.Comment);

            return result;
        }

    }

    public class TBT
    {
        public string ID { get; set; }
        public string Text { get; set; }
        public string Comment { get; set; }
        public List<string> Sources { get; set; }

        public TBT()
        {
            ID = string.Empty;
            Text = string.Empty;
            Comment = string.Empty;
            Sources = new List<string> { };
        }
    }

    public static class TBTSerializer
    {
        public static void SerializeToXml(TBTs tbt, string filePath)
        {
            using (var writer = new XmlTextWriter(filePath, System.Text.Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                var serializer = new XmlSerializer(typeof(TBTs));
                serializer.Serialize(writer, tbt);
            }
        }
    }
}
