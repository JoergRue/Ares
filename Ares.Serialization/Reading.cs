using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Reflection;

namespace Ares.Serialization
{

    partial class XmlFormatter
    {
        private Dictionary<long, Type> m_ReadTypes;

        private Dictionary<Type, int> m_ReadVersions;

        private delegate object MemberReader();
        private Dictionary<string, MemberReader> m_MemberReaders;

        private System.Xml.XmlReader m_Reader;

        private ObjectManager m_ObjectManager;

        private void InitializeMemberReaders()
        {
            m_MemberReaders = new Dictionary<string, MemberReader>();
            m_MemberReaders["ValueType"] = ReadValueType;
            m_MemberReaders["Array"] = ReadArray;
            m_MemberReaders["ObjectReference"] = ReadReference;
            m_MemberReaders["Boolean"] = ReadBoolean;
            m_MemberReaders["Byte"] = ReadByte;
            m_MemberReaders["Char"] = ReadChar;
            m_MemberReaders["DateTime"] = ReadDateTime;
            m_MemberReaders["Decimal"] = ReadDecimal;
            m_MemberReaders["Double"] = ReadDouble;
            m_MemberReaders["Int16"] = ReadInt16;
            m_MemberReaders["Int32"] = ReadInt32;
            m_MemberReaders["Int64"] = ReadInt64;
            m_MemberReaders["SByte"] = ReadSByte;
            m_MemberReaders["Single"] = ReadSingle;
            m_MemberReaders["String"] = ReadString;
            m_MemberReaders["TimeSpan"] = ReadTimeSpan;
            m_MemberReaders["UInt16"] = ReadUInt16;
            m_MemberReaders["UInt32"] = ReadUInt32;
            m_MemberReaders["UInt64"] = ReadUInt64;
        }

        private object DoDeserialize(System.IO.Stream serializationStream)
        {
            m_ObjectManager = new ObjectManager(SurrogateSelector, Context);
            m_ReadTypes = new Dictionary<long, Type>();
            m_ReadVersions = new Dictionary<Type, int>();
            InitializeMemberReaders();

            using (System.Xml.XmlReader reader = CreateReader(serializationStream))
            {
                m_Reader = reader;

                try
                {
                    m_Reader.ReadToFollowing("ObjectGraph");
                    m_Reader.ReadStartElement("ObjectGraph");

                    object result = ReadTopLevelObject();

                    while (m_Reader.IsStartElement("Object"))
                    {
                        ReadTopLevelObject();
                    }

                    m_Reader.ReadEndElement();

                    m_ObjectManager.DoFixups();
                    m_ObjectManager.RaiseDeserializationEvent();

                    return result;

                }
                catch (System.Xml.XmlException ex)
                {
                    ThrowException(Resources.XmlDefect, ex);
                    // to make the compiler happy ...
                    return null;
                }
            }
        }

        private object ReadTopLevelObject()
        {
            long objID; SerializationInfo info;
            Object newObject = ReadObject(out objID, out info);
            if (newObject == null) return null;

            ISurrogateSelector sel;
            ISerializationSurrogate surrogate = (SurrogateSelector != null) ? SurrogateSelector.GetSurrogate(newObject.GetType(), Context, out sel) : null;
            if (surrogate != null || newObject is ISerializable)
            {
                m_ObjectManager.RegisterObject(newObject, objID, info);
                ProcessObjectReferences(objID, info);
            }
            else
            {
                m_ObjectManager.RegisterObject(newObject, objID);
                PopulateObjectMembers(newObject, objID, newObject.GetType(), info);
            }

            return newObject;
        }

        private object ReadObject(out long objID, out SerializationInfo members)
        {
            objID = m_Converter.ToInt64(m_Reader.GetAttribute("ID"));

            if (m_Reader.GetAttribute("IsNull") != null)
            {
                m_ObjectManager.RegisterObject(null, objID);
                members = null;
                return null;
            }

            Type objType = ReadTypeInfo();
            Object newObject = FormatterServices.GetSafeUninitializedObject(objType);
            members = new SerializationInfo(objType, m_Converter);

            m_Reader.ReadStartElement();

            ReadMembers(members);

            m_Reader.ReadEndElement();

            return newObject;
        }

        private void ReadMembers(SerializationInfo info)
        {
            while (m_Reader.IsStartElement())
            {
                if (m_Reader.LocalName != "Member")
                {
                    m_Reader.ReadOuterXml();
                }
                else
                {
                    string name = m_Reader.GetAttribute("Name");
                    string typeInfo = m_Reader.GetAttribute("MemberType");
                    if (name == null || typeInfo == null)
                    {
                        ThrowException(Resources.MemberInfoMissing);
                    }
                    if (!m_MemberReaders.ContainsKey(typeInfo))
                    {
                        ThrowException(Resources.MemberTypeUnknown, typeInfo);
                    }

                    m_Reader.ReadStartElement();
                    object value = m_MemberReaders[typeInfo]();
                    m_Reader.ReadEndElement();

                    info.AddValue(name, value);
                }
            }
        }

        private void FillObject(ObjectReference reference)
        {
            if (reference.Fixup == FixupType.Normal) return;

            object newObject = reference.RealObject;
            if (newObject == null) return;

            ISurrogateSelector sel;
            ISerializationSurrogate surrogate = (SurrogateSelector != null) ? SurrogateSelector.GetSurrogate(newObject.GetType(), Context, out sel) : null;
            if (surrogate != null || newObject is ISerializable)
            {
                ProcessObjectReferences(reference.RefID, reference.Info);
            }
            else
            {
                PopulateObjectMembers(newObject, reference.RefID, newObject.GetType(), reference.Info);
            }
        }

        private void ProcessObjectReferences(long objectID, SerializationInfo info)
        {
            SerializationInfoEnumerator enumerator = info.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Value is ObjectReference)
                {
                    ObjectReference reference = enumerator.Value as ObjectReference;
                    m_ObjectManager.RecordDelayedFixup(objectID, enumerator.Name, reference.RefID);
                    if (reference.Fixup == FixupType.ValueType) FillObject(reference);
                }
            }
        }

        private bool IsDataNeeded(MemberInfo info, Type objType)
        {
            if (info.IsDefined(typeof(NonSerializedAttribute), true)) 
            {
                return false;
            }

            if (info.IsDefined(typeof(OptionalFieldAttribute), true))
            {
                object[] attrs = info.GetCustomAttributes(typeof(OptionalFieldAttribute), true);
                int sinceVersion = ((OptionalFieldAttribute) attrs[0]).VersionAdded;
                if (m_ReadVersions.ContainsKey(objType))
                {
                    if (sinceVersion > m_ReadVersions[objType])
                    {
                        return false;
                    }
                }
                else
                {
                    // unconditionally if no version is set ...
                    return false;
                }
            }

            return true;
        }

        private void PopulateObjectMembers(Object obj, long objectID, Type objType, SerializationInfo info)
        {
            if (!objType.IsSerializable) ThrowException(Resources.NotSerializable, objType.FullName);

            MemberInfo[] members = FormatterServices.GetSerializableMembers(objType);
            object[] data = new object[members.Length];
            for (int i = 0; i < members.Length; ++i)
            {
                if (IsDataNeeded(members[i], objType))
                {
                    object value = info.GetValue(members[i].Name, typeof(object));
                    ObjectReference reference = value as ObjectReference;
                    if (reference != null)
                    {
                        if (reference.Fixup == FixupType.ValueType)
                        {
                            m_ObjectManager.RegisterObject(reference.RealObject, reference.RefID, reference.Info, objectID, members[i]);
                            FillObject(reference);
                        }
                        m_ObjectManager.RecordFixup(objectID, members[i], reference.RefID);
                    }
                    else
                    {
                        data[i] = value;
                    }
                }
            }

            FormatterServices.PopulateObjectMembers(obj, members, data);
        }

        private Type ReadTypeInfo()
        {
            String typeRef = m_Reader.GetAttribute("TypeRef");
            if (typeRef != null)
            {
                long typeID = m_Converter.ToInt64(typeRef);
                if (!m_ReadTypes.ContainsKey(typeID))
                {
                    ThrowException(Resources.TypeNotYetRead, typeID);
                }
                return m_ReadTypes[typeID];
            }
            else
            {
                String assemblyName = m_Reader.GetAttribute("Assembly");
                String typeName = m_Reader.GetAttribute("Type");
                String typeIDString = m_Reader.GetAttribute("TypeID");
                String version = m_Reader.GetAttribute("SerializationVersion");
                if (assemblyName == null || typeName == null || typeIDString == null)
                {
                    ThrowException(Resources.TypeInfoMissing);
                }

                long typeID = m_Converter.ToInt64(typeIDString);

                if (Binder != null)
                {
                    Type type = Binder.BindToType(assemblyName, typeName);
                    if (type != null)
                    {
                        m_ReadTypes[typeID] = type;
                        if (version != null)
                        {
                            m_ReadVersions[type] = m_Converter.ToInt32(version);
                        }
                        return type;
                    }
                }

                AssemblyName an = null;
                if (AssemblyStyle == System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full)
                {
                    an = new AssemblyName(assemblyName);
                }
                else
                {
                    an = new AssemblyName();
                    an.Name = assemblyName;
                }
                try
                {
                    Assembly assembly = Assembly.Load(an);
                    Type type = assembly.GetType(typeName);
                    if (type == null)
                    {
                        ThrowException(Resources.TypeNotFound, typeName);
                    }
                    m_ReadTypes[typeID] = type;
                    if (version != null)
                    {
                        m_ReadVersions[type] = m_Converter.ToInt32(version);
                    }
                    return type;
                }
                catch (System.IO.FileNotFoundException ex)
                {
                    ThrowException(Resources.AssemblyNotFound, assemblyName, ex);
                }
                catch (System.IO.FileLoadException ex)
                {
                    ThrowException(Resources.AssemblyNotFound, assemblyName, ex);
                }
                catch (BadImageFormatException ex)
                {
                    ThrowException(Resources.AssemblyNotFound, assemblyName, ex);
                }
            }
            // to make the compiler happy ...
            System.Diagnostics.Debug.Assert(false, "Is never reached");
            return null;
        }

        private enum FixupType { Normal, ValueType };

        private class ObjectReference
        {
            public FixupType Fixup { get; set; }
            public long RefID { get; set; } // all fixup types
            // only for value types
            public object RealObject { get; set; }
            public SerializationInfo Info { get; set; }
        }

        private object ReadValueType()
        {
            long objID; SerializationInfo info;
            Object newObject = ReadObject(out objID, out info);

            ObjectReference reference = new ObjectReference();
            reference.Fixup = FixupType.ValueType;
            reference.RefID = objID;
            reference.RealObject = newObject;
            reference.Info = info;

            return reference;
        }

        private object ReadArray()
        {
            long objID = m_Converter.ToInt64(m_Reader.GetAttribute("ID"));
            int rank = m_Converter.ToInt32(m_Reader.GetAttribute("Rank"));
            Type memberType = ReadTypeInfo();
            m_Reader.ReadStartElement("Array");
            int[] lowerBounds = new int[rank];
            int[] upperBounds = new int[rank];
            int[] lengths = new int[rank];
            long count = 1;
 
            m_Reader.ReadStartElement("Dimensions");
            for (int i = 0; i < rank; ++i)
            {
                lowerBounds[i] = m_Converter.ToInt32(m_Reader.GetAttribute("LowerBound"));
                upperBounds[i] = m_Converter.ToInt32(m_Reader.GetAttribute("UpperBound"));
                if (upperBounds[i] < lowerBounds[i])
                {
                    ThrowException(Resources.InvalidArrayBounds);
                }
                lengths[i] = upperBounds[i] - lowerBounds[i] + 1;
                count *= lengths[i];
                m_Reader.ReadOuterXml();
            }
            m_Reader.ReadEndElement();

            Array array = Array.CreateInstance(memberType, lengths, lowerBounds);
            SerializationInfo info = new SerializationInfo(typeof(Array), m_Converter);
            ReadMembers(info);

            object[] arrayElements = new object[count];
            m_Reader.ReadStartElement("Elements");
            for (long i = 0; i < count; ++i)
            {
                arrayElements[i] = info.GetValue("Element" + count, typeof(Object));
            }
            m_Reader.ReadEndElement(); // </Elements>
            m_Reader.ReadEndElement(); // </Array>

            m_ObjectManager.RegisterObject(array, objID);

            int[] indices = new int[rank];
            for (int i = 0; i < rank; ++i) indices[i] = lowerBounds[i];
            for (long i = 0; i < count; ++i)
            {
                if (arrayElements[i] is ObjectReference)
                {
                    ObjectReference reference = arrayElements[i] as ObjectReference;
                    if (reference.Fixup == FixupType.ValueType)
                    {
                        m_ObjectManager.RegisterObject(reference.RealObject, reference.RefID, reference.Info, objID, null, indices);
                        FillObject(reference);
                    }
                    m_ObjectManager.RecordArrayElementFixup(objID, indices, reference.RefID);
                }
                else
                {
                    array.SetValue(arrayElements[i], indices);
                }

                int currentDim = rank - 1;
                ++indices[currentDim];
                while (indices[currentDim] > upperBounds[currentDim])
                {
                    indices[currentDim] = lowerBounds[currentDim];
                    --currentDim;
                    ++indices[currentDim];
                }
            }

            return array;
        }

        private object ReadReference()
        {
            ObjectReference reference = new ObjectReference();
            reference.Fixup = FixupType.Normal;
            reference.RefID = m_Converter.ToInt64(m_Reader.ReadString());
            return reference;
        }

        private object ReadBoolean()
        {
            return m_Converter.ToBoolean(m_Reader.ReadString());
        }

        private object ReadByte()
        {
            return m_Converter.ToByte(m_Reader.ReadString());
        }

        private object ReadChar()
        {
            return m_Converter.ToChar(m_Reader.ReadString());
        }

        private object ReadDateTime()
        {
            Int64 data = m_Converter.ToInt64(m_Reader.ReadString());
            return DateTime.FromBinary(data);
        }

        private object ReadDecimal()
        {
            return m_Converter.ToDecimal(m_Reader.ReadString());
        }

        private object ReadDouble()
        {
            return m_Converter.ToDouble(m_Reader.ReadString());
        }

        private object ReadInt16()
        {
            return m_Converter.ToInt16(m_Reader.ReadString());
        }

        private object ReadInt32()
        {
            return m_Converter.ToInt32(m_Reader.ReadString());
        }

        private object ReadInt64()
        {
            return m_Converter.ToInt64(m_Reader.ReadString());
        }

        private object ReadSByte()
        {
            return m_Converter.ToSByte(m_Reader.ReadString());
        }

        private object ReadSingle()
        {
            return m_Converter.ToSingle(m_Reader.ReadString());
        }
        
        private object ReadString()
        {
            return m_Reader.ReadString();
        }

        private object ReadTimeSpan()
        {
            long ticks = m_Converter.ToInt64(m_Reader.ReadString());
            return TimeSpan.FromTicks(ticks);
        }

        private object ReadUInt16()
        {
            return m_Converter.ToUInt16(m_Reader.ReadString());
        }

        private object ReadUInt32()
        {
            return m_Converter.ToUInt32(m_Reader.ReadString());
        }

        private object ReadUInt64()
        {
            return m_Converter.ToUInt64(m_Reader.ReadString());
        }

        private static System.Xml.XmlReader CreateReader(System.IO.Stream stream)
        {
            System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings();
            settings.CheckCharacters = true;
            settings.CloseInput = false;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            settings.ValidationType = System.Xml.ValidationType.None;
            return System.Xml.XmlReader.Create(stream, settings);
        }
    }

}
