using System;
using System.Xml;

namespace Ares.Data
{
    static class XmlHelpers
    {
        public static String GetNonEmptyAttribute(this XmlReader reader, String name)
        {
            String result = reader.GetAttribute(name);
            if (String.IsNullOrEmpty(result))
            {
                ThrowException(String.Format(StringResources.ExpectedAttribute, name), reader);
            }
            return result;
        }

        public static int GetIntegerAttribute(this XmlReader reader, String name)
        {
            Int32 value;
            String val = GetNonEmptyAttribute(reader, name);
            bool ret = Int32.TryParse(val, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value);
            if (!ret)
            {
                ThrowException(String.Format(StringResources.ExpectedInteger, name),  reader);
            }
            return value;
        }

        public static TimeSpan GetTimeSpanAttribute(this XmlReader reader, String name)
        {
            long value;
            String val = GetNonEmptyAttribute(reader, name);
            bool ret = Int64.TryParse(val, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value);
            if (!ret)
            {
                ThrowException(String.Format(StringResources.ExpectedInteger, name), reader);
            }
            return TimeSpan.FromTicks(value);
        }

        public static bool GetBooleanAttribute(this XmlReader reader, String name)
        {
            String val = GetNonEmptyAttribute(reader, name);
            return (val == "true");
        }

        public static bool GetBooleanAttributeOrDefault(this XmlReader reader, String name, bool defaultValue)
        {
            String result = reader.GetAttribute(name);
            if (String.IsNullOrEmpty(result))
                return defaultValue;
            else
                return (result == "true");
        }

        public static int GetIntegerAttributeOrDefault(this XmlReader reader, String name, int defaultValue)
        {
            String result = reader.GetAttribute(name);
            if (String.IsNullOrEmpty(result))
                return defaultValue;
            int value;
            bool ret = Int32.TryParse(result, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value);
            if (!ret)
            {
                ThrowException(String.Format(StringResources.ExpectedInteger, name), reader);
            }
            return value;
        }

        private static int GetLineNumber(XmlReader reader)
        {
            return (reader as IXmlLineInfo).LineNumber;
        }

        private static int GetLinePosition(XmlReader reader)
        {
            return (reader as IXmlLineInfo).LinePosition;
        }

        public static void ThrowException(String msg, XmlReader reader)
        {
            throw new XmlException(msg, null, GetLineNumber(reader), GetLinePosition(reader));
        }
    }
}
