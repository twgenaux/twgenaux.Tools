using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.Linq;


namespace ResxFindStrings
{
    /// <summary>
    /// Container for To-Be-Translated Resx strings
    /// </summary>
    [XmlRoot("TBTs")]
    public class ToBeTranslatedStrings
    {
        /// <summary>
        /// List of To-Be-Translated Resx strings
        /// </summary>
        [XmlElement("TBT")]
        public List<ToBeTranslated> ToBeTranslated { get; set; }

        public ToBeTranslatedStrings()
        {
            ToBeTranslated = new List<ToBeTranslated>();
        }

        public void Add(ToBeTranslated tbt) 
        { 
            ToBeTranslated.Add(tbt);
        }

        /// <summary>
        /// Merge all Equivalent strings
        /// </summary>
        public void MergeEquivalent()
        {
            // Sort the TBTs, ignoring sources and instructions
            ToBeTranslatedStrings sortedTBTS = new ToBeTranslatedStrings();
            sortedTBTS.ToBeTranslated = 
                ToBeTranslated.OrderBy(x => x.ID).ThenBy(x => x.Text).ThenBy(x => x.Comment).ToList();

            ToBeTranslatedStrings mergedTBTS = new ToBeTranslatedStrings();  // new list after merging

            ToBeTranslated current = null; // the current merege canidate
            foreach (var tbt in sortedTBTS.ToBeTranslated)
            {
                // Compare the current merege canidate to tbt, ignoring sources
                if (current == null)
                {
                    // initialize the first one
                    current = tbt; 
                }
                else if (0 == Equivalent(current, tbt))
                {
                    current.Sources.AddRange(tbt.Sources);
                }
                else // move to the next canidate
                {
                    // add current to mergedTBTS
                    mergedTBTS.Add(current);
                    current = tbt;
                }
            }
            ToBeTranslated = mergedTBTS.ToBeTranslated;
        }

        /// <summary>
        /// Compares two TBT's for equivalency
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        static private int Equivalent(ToBeTranslated x, ToBeTranslated y)
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

    /// <summary>
    /// Container for a To-Be-Translated Resx string
    /// </summary>
    public class ToBeTranslated
    {
        /// <summary>
        /// Resx name
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Resx value
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Resx comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Additional instructions for the translator
        /// </summary>
        public string TranslationInstructions { get; set; }

        /// <summary>
        /// List of the source files
        /// </summary>
        public List<string> Sources { get; set; }

        public ToBeTranslated()
        {
            ID = string.Empty;
            Text = string.Empty;
            Comment = string.Empty;
            TranslationInstructions = string.Empty;
            Sources = new List<string> { };
        }
    }

    public static class TBTSerializer
    {
        public static void SerializeToXml(ToBeTranslatedStrings tbt, string filePath)
        {
            using (var writer = new XmlTextWriter(filePath, System.Text.Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                var serializer = new XmlSerializer(typeof(ToBeTranslatedStrings));
                serializer.Serialize(writer, tbt);
            }
        }
    }
}
