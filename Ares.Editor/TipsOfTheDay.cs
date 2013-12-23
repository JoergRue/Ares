using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Ares.Editor
{
    static class TipsOfTheDay
    {
        public static List<String> GetTipsOfTheDay()
        {
            try
            {
                String dir = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
                String file = StringResources.TipsFile;
                String tipsFilePath = System.IO.Path.Combine(dir, file);
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                settings.IgnoreProcessingInstructions = true;
                settings.IgnoreWhitespace = true;
                settings.ValidationType = ValidationType.None;
                using (System.IO.FileStream stream = new System.IO.FileStream(tipsFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    using (System.IO.StreamReader streamReader = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8))
                    {
                        using (XmlReader reader = XmlReader.Create(streamReader, settings))
                        {
                            return ReadFile(reader);
                        }
                    }
                }
            }
            catch (System.IO.IOException)
            {
                return new List<String>();
            }
            catch (System.InvalidOperationException) // often thrown by Xml Reader
            {
                return new List<String>();
            }
            catch (XmlException)
            {
                return new List<String>();
            }
        }

        private static List<String> ReadFile(XmlReader reader)
        {
            List<String> result = new List<string>();
            reader.Read();
            reader.MoveToElement();
            if (!reader.IsStartElement("TipsOfTheDay"))
            {
                return result;
            }
            if (reader.IsEmptyElement)
            {
                reader.Read(); // <TipsOfTheDay />
            }
            else
            {
                reader.Read(); // <TipsOfTheDay>
                while (reader.IsStartElement())
                {
                    if (reader.IsStartElement("Tip")) 
                    {
                        if (!reader.IsEmptyElement)
                        {
                            reader.Read(); // <Tip>
                            if (reader.Name != "Tip")
                                result.Add(reader.ReadInnerXml());
                            reader.ReadEndElement(); // </Tip>
                        }
                        else
                            reader.Read(); // <Tip />
                    }
                    else
                    {
                        reader.ReadOuterXml();
                    }
                }
                reader.ReadEndElement();

            }
            return result;
        }
    }
}
