/*
 Copyright (c) 2013 [Joerg Ruedenauer]
 
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
using System.Collections.Generic;

namespace Ares.Data
{
    class MusicByTags : ElementBase, IMusicByTags
    {
        internal MusicByTags(int elementId, String title)
            : base(elementId)
        {
            Title = title;
            m_Tags = new Dictionary<int, HashSet<int>>();
            TagCategoryCombination = Data.TagCategoryCombination.UseOneTagOfEachCategory;
            FadeTime = 0;
        }

        public IDictionary<int, HashSet<int>> GetTags()
        {
            return m_Tags;
        }

        public HashSet<int> GetAllTags()
        {
            HashSet<int> tags = new HashSet<int>();
            foreach (int category in m_Tags.Keys)
            {
                tags.UnionWith(m_Tags[category]);
            }
            return tags;
        }

        public void AddTag(int category, int tag)
        {
            if (!m_Tags.ContainsKey(category))
            {
                m_Tags.Add(category, new HashSet<int>());
            }
            m_Tags[category].Add(tag);
        }

        public void RemoveTag(int category, int tag)
        {
            if (m_Tags.ContainsKey(category))
            {
                m_Tags[category].Remove(tag);
                if (m_Tags[category].Count == 0)
                {
                    m_Tags.Remove(category);
                }
            }
        }

        public TagCategoryCombination TagCategoryCombination { get; set; }

        public int FadeTime { get; set; }

        public override void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("MusicByTags");
            DoWriteToXml(writer);
            writer.WriteAttributeString("IsOperatorAnd",  TagCategoryCombination == Data.TagCategoryCombination.UseOneTagOfEachCategory ? "true" : "false");
            writer.WriteAttributeString("TagCategoryCombination", ((int)TagCategoryCombination).ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("FadeTime", FadeTime.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteStartElement("Tags");
            foreach (int category in m_Tags.Keys)
            {
                foreach (int tag in m_Tags[category])
                {
                    writer.WriteStartElement("Tag");
                    writer.WriteAttributeString("Category", category.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("Tag", tag.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        public override void Visit(IElementVisitor visitor)
        {
            visitor.VisitMusicByTags(this);
        }

        internal MusicByTags(System.Xml.XmlReader reader)
            : base(reader)
        {
            m_Tags = new Dictionary<int, HashSet<int>>();
            bool isOperatorAnd = reader.GetBooleanAttributeOrDefault("IsOperatorAnd", true);
            int tagCategoryCombination = reader.GetIntegerAttributeOrDefault("TagCategoryCombination", -1);
            if (tagCategoryCombination >= (int)Data.TagCategoryCombination.UseAnyTag && tagCategoryCombination <= (int)Data.TagCategoryCombination.UseAllTags)
            {
                TagCategoryCombination = (Data.TagCategoryCombination)tagCategoryCombination;
            }
            else
            {
                TagCategoryCombination = isOperatorAnd ? Data.TagCategoryCombination.UseOneTagOfEachCategory : Data.TagCategoryCombination.UseAnyTag;
            }
            FadeTime = reader.GetIntegerAttributeOrDefault("FadeTime", 0);
            if (reader.IsEmptyElement)
            {
                XmlHelpers.ThrowException(StringResources.ExpectedContent, reader);
            }
            reader.Read();
            while (reader.IsStartElement())
            {
                if (reader.IsStartElement("Tags") && !reader.IsEmptyElement)
                {
                    reader.Read();
                    while (reader.IsStartElement())
                    {
                        if (reader.IsStartElement("Tag"))
                        {
                            int category = reader.GetIntegerAttribute("Category");
                            int tag = reader.GetIntegerAttribute("Tag");
                            AddTag(category, tag);
                            reader.ReadOuterXml();
                        }
                        else
                        {
                            reader.ReadOuterXml();
                        }
                    }
                    reader.ReadEndElement();
                }
                else
                {
                    reader.ReadOuterXml();
                }
            }
            reader.ReadEndElement();
        }

        private Dictionary<int, HashSet<int>> m_Tags;
        
    }
}
