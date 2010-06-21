/*
 Copyright (c) 2010 [Joerg Ruedenauer]
 
 This file is part of Ares.

 Ares is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 Ares is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with Ares; if not, write to the Free Software
 Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */
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
