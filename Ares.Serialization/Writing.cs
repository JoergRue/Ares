using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Reflection;

namespace Ares.Serialization
{

    partial class XmlFormatter
    {
        private Dictionary<String, Dictionary<String, long>> m_WrittenTypes;

        private long m_NextTypeID;
        
        private System.Xml.XmlWriter m_Writer;

        private void DoSerialize(System.IO.Stream serializationStream, object graph)
        {
            m_WrittenTypes = new Dictionary<String, Dictionary<String, long>>();
            m_NextTypeID = 1;

            using (System.Xml.XmlWriter writer = CreateWriter(serializationStream))
            {
                m_Writer = writer;

                m_Writer.WriteStartDocument();
                m_Writer.WriteStartElement("ObjectGraph");

                Schedule(graph);
                while (m_objectQueue.Count > 0)
                {
                    long objectID;
                    object next = GetNext(out objectID);
                    DoSerialize(next, objectID);
                }

                m_Writer.WriteEndElement();
                m_Writer.WriteEndDocument();
                m_Writer.Flush();
            }
        }

        private static System.Xml.XmlWriter CreateWriter(System.IO.Stream serializationStream)
        {
            System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
            settings.CloseOutput = false;
            settings.CheckCharacters = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            settings.NewLineOnAttributes = false;
            return System.Xml.XmlWriter.Create(serializationStream, settings);
        }

        private void DoSerialize(Object obj, long objectID)
        {
            m_Writer.WriteStartElement("Object");
            WriteObject(obj, objectID);
        }

        private void WriteObject(Object obj, long objectID)
        {
            m_Writer.WriteAttributeString("ID", Convert.ToString(objectID, System.Globalization.CultureInfo.InvariantCulture));

            ISurrogateSelector sel;
            ISerializationSurrogate surrogate = (SurrogateSelector != null) ? SurrogateSelector.GetSurrogate(obj.GetType(), Context, out sel) : null;
            if (surrogate != null)
            {
                SerializationInfo info = new SerializationInfo(obj.GetType(), m_Converter);
                info.AssemblyName = GetAssemblyName(obj.GetType().Assembly);
                surrogate.GetObjectData(obj, info, Context);
                WriteTypeInfo(info.AssemblyName, info.FullTypeName, obj);
                WriteSerializationInfo(info);
                m_Writer.WriteEndElement();
                return;
            }

            if (obj == null)
            {
                m_Writer.WriteAttributeString("IsNull", "true");
                m_Writer.WriteEndElement();
                return;
            }

            if (!obj.GetType().IsSerializable) ThrowException(Resources.NotSerializable, obj.GetType().FullName);

            ISerializable serializableObj = obj as ISerializable;
            if (serializableObj != null)
            {
                SerializationInfo info = new SerializationInfo(obj.GetType(), m_Converter);
                info.AssemblyName = GetAssemblyName(obj.GetType().Assembly);
                serializableObj.GetObjectData(info, Context);

                WriteTypeInfo(info.AssemblyName, info.FullTypeName, obj);
                WriteSerializationInfo(info);
                m_Writer.WriteEndElement();
                return;
            }

            MemberInfo[] members = FormatterServices.GetSerializableMembers(obj.GetType(), Context);
            object[] data = FormatterServices.GetObjectData(obj, members);
            WriteTypeInfo(GetAssemblyName(obj.GetType().Assembly), obj.GetType().FullName, obj);
            WriteObjectData(members, data);
            m_Writer.WriteEndElement();
        }

        private bool GetTypeRef(String assemblyName, String typeName, out long typeRef)
        {
            if (m_WrittenTypes.ContainsKey(assemblyName))
            {
                Dictionary<String, long> typeDict = m_WrittenTypes[assemblyName];
                if (typeDict.ContainsKey(typeName))
                {
                    typeRef = typeDict[typeName];
                    return true;
                }
                else
                {
                    typeDict[typeName] = m_NextTypeID;
                }
            }
            else
            {
                Dictionary<String, long> typeDict = new Dictionary<string, long>();
                typeDict[typeName] = m_NextTypeID;
                m_WrittenTypes[assemblyName] = typeDict;
            }
            typeRef = m_NextTypeID;
            ++m_NextTypeID;
            return false;
        }

        private void WriteTypeInfo(String assemblyName, String typeName, object obj)
        {
            long typeID;
            if (GetTypeRef(assemblyName, typeName, out typeID))
            {
                m_Writer.WriteAttributeString("TypeRef", m_Converter.ToString(typeID));
            }
            else
            {
                object[] attrs = obj.GetType().GetCustomAttributes(typeof(SerializationVersionAttribute), false);
                m_Writer.WriteAttributeString("Assembly", assemblyName);
                m_Writer.WriteAttributeString("Type", typeName);
                m_Writer.WriteAttributeString("TypeID", m_Converter.ToString(typeID));
                if (attrs.Length > 0)
                {
                    m_Writer.WriteAttributeString("SerializationVersion", m_Converter.ToString(
                        ((SerializationVersionAttribute)attrs[0]).Version));
                }

            }
        }

        private void WriteSerializationInfo(SerializationInfo info)
        {
            SerializationInfoEnumerator enumerator = info.GetEnumerator();
            while (enumerator.MoveNext())
            {
                WriteMember(enumerator.Name, enumerator.Value);
            }
        }

        private void WriteObjectData(MemberInfo[] members, object[] data)
        {
            for (int i = 0; i < members.Length; ++i)
            {
                if (members[i].IsDefined(typeof(NonSerializedAttribute), true)) continue;
                WriteMember(members[i].Name, data[i]);
            }
        }

        /// <summary>
        /// Writes a value type.
        /// </summary>
        protected override void WriteValueType(object obj, string name, Type memberType)
        {
            m_Writer.WriteStartElement("Member");
            m_Writer.WriteAttributeString("Name", name);
            m_Writer.WriteAttributeString("MemberType", "ValueType");
            m_Writer.WriteStartElement("Content");
            
            bool dummy;
            WriteObject(obj, m_idGenerator.GetId(obj, out dummy));
            
            m_Writer.WriteEndElement();
        }

        private String GetAssemblyName(Assembly assembly)
        {
            if (AssemblyStyle == System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full)
            {
                return assembly.FullName;
            }
            else
            {
                return assembly.GetName().Name;
            }
        }

        private static void ThrowException(String message)
        {
            throw new SerializationException(message);
        }

        private static void ThrowException(String message, object value)
        {
            String formatted = String.Format(System.Globalization.CultureInfo.CurrentCulture, message,
                new object[] { value });
            ThrowException(formatted);
        }

        private static void ThrowException(String message, Exception inner)
        {
            throw new SerializationException(message, inner);
        }

        private static void ThrowException(String message, object value, Exception inner)
        {
            String formatted = String.Format(System.Globalization.CultureInfo.CurrentCulture, message,
                new object[] { value });
            ThrowException(formatted, inner);
        }

        /// <summary>
        /// Writes an array.
        /// </summary>
        protected override void WriteArray(object obj, string name, Type memberType)
        {
            bool firstTime;
            long objectID = m_idGenerator.GetId(obj, out firstTime);
            if (firstTime)
            {
                Array a = (Array)obj;
                m_Writer.WriteStartElement("Member");
                m_Writer.WriteAttributeString("Name", name);
                m_Writer.WriteAttributeString("MemberType", "Array");

                m_Writer.WriteStartElement("Array");
                m_Writer.WriteAttributeString("ID", m_Converter.ToString(objectID));
                WriteTypeInfo(GetAssemblyName(memberType.Assembly), memberType.FullName, obj);
                m_Writer.WriteAttributeString("Rank", m_Converter.ToString(a.Rank));
                m_Writer.WriteStartElement("Dimensions");
                for (int i = 0; i < a.Rank; ++i)
                {
                    m_Writer.WriteStartElement("Dimension");
                    m_Writer.WriteAttributeString("LowerBound", m_Converter.ToString(a.GetLowerBound(i)));
                    m_Writer.WriteAttributeString("UpperBound", m_Converter.ToString(a.GetUpperBound(i)));
                    m_Writer.WriteEndElement();
                }
                m_Writer.WriteEndElement();
                m_Writer.WriteStartElement("Elements");
                long count = 0;
                foreach (object element in a)
                {
                    WriteMember("Element" + count, element);
                    ++count;
                }
                m_Writer.WriteEndElement();
                m_Writer.WriteEndElement(); // </Array>
                m_Writer.WriteEndElement(); // </Member>
            }
            else
            {
                WriteReference(name, objectID);
            }
        }

        private void WriteReference(String name, long objectID)
        {
            m_Writer.WriteStartElement("Member");
            m_Writer.WriteAttributeString("Name", name);
            m_Writer.WriteAttributeString("MemberType", "ObjectReference");
            m_Writer.WriteString(m_Converter.ToString(objectID));
            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a boolean.
        /// </summary>
        protected override void WriteBoolean(bool val, string name)
        {
            m_Writer.WriteStartElement("Member");
            m_Writer.WriteAttributeString("Name", name);
            m_Writer.WriteAttributeString("MemberType", "Boolean");
            m_Writer.WriteString(m_Converter.ToString(val));
            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes an 8 bit unsigned integer.
        /// </summary>
        protected override void WriteByte(byte val, string name)
        {
            m_Writer.WriteStartElement("Member");
            m_Writer.WriteAttributeString("Name", name);
            m_Writer.WriteAttributeString("MemberType", "Byte");
            m_Writer.WriteString(m_Converter.ToString(val));
            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a character.
        /// </summary>
        protected override void WriteChar(char val, string name)
        {
            m_Writer.WriteStartElement("Member");
            m_Writer.WriteAttributeString("Name", name);
            m_Writer.WriteAttributeString("MemberType", "Char");
            m_Writer.WriteString(m_Converter.ToString(val));
            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a date.
        /// </summary>
        protected override void WriteDateTime(DateTime val, string name)
        {
            m_Writer.WriteStartElement("Member");
            m_Writer.WriteAttributeString("Name", name);
            m_Writer.WriteAttributeString("MemberType", "DateTime");
            m_Writer.WriteString(m_Converter.ToString(val.ToBinary()));
            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a decimal.
        /// </summary>
        protected override void WriteDecimal(decimal val, string name)
        {
            m_Writer.WriteStartElement("Member");
            m_Writer.WriteAttributeString("Name", name);
            m_Writer.WriteAttributeString("MemberType", "Decimal");
            m_Writer.WriteString(m_Converter.ToString(val));
            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a double.
        /// </summary>
        protected override void WriteDouble(double val, string name)
        {
            m_Writer.WriteStartElement("Member");
            m_Writer.WriteAttributeString("Name", name);
            m_Writer.WriteAttributeString("MemberType", "Double");
            m_Writer.WriteString(m_Converter.ToString(val));
            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a 16 bit signed integer.
        /// </summary>
        protected override void WriteInt16(short val, string name)
        {
            m_Writer.WriteStartElement("Member");
            m_Writer.WriteAttributeString("Name", name);
            m_Writer.WriteAttributeString("MemberType", "Int16");
            m_Writer.WriteString(m_Converter.ToString(val));
            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a 32 bit signed integer.
        /// </summary>
        protected override void WriteInt32(int val, string name)
        {
            m_Writer.WriteStartElement("Member");
            m_Writer.WriteAttributeString("Name", name);
            m_Writer.WriteAttributeString("MemberType", "Int32");
            m_Writer.WriteString(m_Converter.ToString(val));
            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a 64 bit signed integer.
        /// </summary>
        protected override void WriteInt64(long val, string name)
        {
            m_Writer.WriteStartElement("Member");
            m_Writer.WriteAttributeString("Name", name);
            m_Writer.WriteAttributeString("MemberType", "Int64");
            m_Writer.WriteString(m_Converter.ToString(val));
            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes an object reference.
        /// </summary>
        protected override void WriteObjectRef(object obj, string name, Type memberType)
        {
            String st = obj as String;
            if (st != null)
            {
                WriteString(st, name);
            }
            else
            {
                long objectID = Schedule(obj);
                WriteReference(name, objectID);
            }
        }
        
        private void WriteString(String val, String name)
        {
            m_Writer.WriteStartElement("Member");
            m_Writer.WriteAttributeString("Name", name);
            m_Writer.WriteAttributeString("MemberType", "String");
            m_Writer.WriteString(val);
            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes an 8 bit signed integer.
        /// </summary>
        [CLSCompliant(false)]
        protected override void WriteSByte(sbyte val, string name)
        {
            m_Writer.WriteStartElement("Member");
            m_Writer.WriteAttributeString("Name", name);
            m_Writer.WriteAttributeString("MemberType", "SByte");
            m_Writer.WriteString(m_Converter.ToString(val));
            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a single-precision floating point number.
        /// </summary>
        protected override void WriteSingle(float val, string name)
        {
            m_Writer.WriteStartElement("Member");
            m_Writer.WriteAttributeString("Name", name);
            m_Writer.WriteAttributeString("MemberType", "Single");
            m_Writer.WriteString(m_Converter.ToString(val));
            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a time span.
        /// </summary>
        protected override void WriteTimeSpan(TimeSpan val, string name)
        {
            m_Writer.WriteStartElement("Member");
            m_Writer.WriteAttributeString("Name", name);
            m_Writer.WriteAttributeString("MemberType", "TimeSpan");
            m_Writer.WriteString(m_Converter.ToString(val));
            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a 16 bit unsigned integer.
        /// </summary>
        [CLSCompliant(false)]
        protected override void WriteUInt16(ushort val, string name)
        {
            m_Writer.WriteStartElement("Member");
            m_Writer.WriteAttributeString("Name", name);
            m_Writer.WriteAttributeString("MemberType", "UInt16");
            m_Writer.WriteString(m_Converter.ToString(val));
            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a 32 bit unsigned integer.
        /// </summary>
        [CLSCompliant(false)]
        protected override void WriteUInt32(uint val, string name)
        {
            m_Writer.WriteStartElement("Member");
            m_Writer.WriteAttributeString("Name", name);
            m_Writer.WriteAttributeString("MemberType", "UInt32");
            m_Writer.WriteString(m_Converter.ToString(val));
            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a 64 bit unsigned integer.
        /// </summary>
        [CLSCompliant(false)]
        protected override void WriteUInt64(ulong val, string name)
        {
            m_Writer.WriteStartElement("Member");
            m_Writer.WriteAttributeString("Name", name);
            m_Writer.WriteAttributeString("MemberType", "UInt64");
            m_Writer.WriteString(m_Converter.ToString(val));
            m_Writer.WriteEndElement();
        }
    }
}