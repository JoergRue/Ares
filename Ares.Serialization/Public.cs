using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Reflection;

namespace Ares.Serialization
{
    
    /// <summary>
    /// The serialization version attribute can be applied to serializable classes. It is
    /// an int which determines the version of the serialized data. If you implement 
    /// ISerializable, you can retrieve the version in the constructor through the
    /// key "SerializationVersion". If you use default serialization, the Optional 
    /// attribute is considered at deserialization only if its VersionAdded property
    /// is higher than the stored SerializationVersion.
    /// </summary>
    /// <remarks>
    /// The SerializationVersion attribute works only in conjunction with the XML formatter.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    [Serializable]
    public sealed class SerializationVersionAttribute : Attribute
    {
        /// <summary>
        /// The current version of the serialized data.
        /// </summary>
        public Int32 Version { get; set; }
    }

    /// <summary>
    /// Provides some extension methods for ease-of-use of deserialization.
    /// </summary>
    public static class SerializationExtenders
    {
        /// <summary>
        /// Retrieves an object from a serialization info. The type given to
        /// SerializationInfo.GetValue is typeof(T); the returned object is 
        /// casted into T.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T GetValue<T>(this SerializationInfo info, String name)
        {
            Object value = info.GetValue(name, typeof(T));
            return (T)value;
        }

        /// <summary>
        /// Deserializes an object stream and casts the root object into T.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T Deserialize<T>(this IFormatter formatter, System.IO.Stream serializationStream)
        {
            return (T)formatter.Deserialize(serializationStream);
        }
    }

    /// <summary>
    /// A formatter similar to BinaryFormatter, but which uses XML format.
    /// </summary>
    /// <remarks>
    /// The formatter serializes all attributes, not only the public ones. Its XML
    /// format is generic, not requiring the client code to be attributed. It is 
    /// not as verbose as the Soap Formatter. It can use the SerializationVersion
    /// attribute.
    /// </remarks>
    [CLSCompliant(false)]
    public sealed partial class XmlFormatter : System.Runtime.Serialization.Formatter
    {
        /// <summary>
        /// The SerializationBinder.
        /// </summary>
        public override SerializationBinder Binder
        {
            get; set;
        }

        /// <summary>
        /// The StreamingContext.
        /// </summary>
        public override StreamingContext Context
        {
            get; set;
        }

        /// <summary>
        /// The AssemblyStyle used.
        /// </summary>
        public System.Runtime.Serialization.Formatters.FormatterAssemblyStyle
            AssemblyStyle { get; set; }

        
        /// <summary>
        /// Deserializes an object graph.
        /// </summary>
        public override object Deserialize(System.IO.Stream serializationStream)
        {
            if (serializationStream == null) throw new ArgumentNullException("serializationStream");

            return DoDeserialize(serializationStream);
        }

        /// <summary>
        /// Serializes an object graph.
        /// </summary>
        public override void Serialize(System.IO.Stream serializationStream, object graph)
        {
            if (serializationStream == null) throw new ArgumentNullException("serializationStream");
            if (graph == null) throw new ArgumentNullException("graph");

            DoSerialize(serializationStream, graph);
        }

        /// <summary>
        /// The SurrogateSelector.
        /// </summary>
        public override ISurrogateSelector SurrogateSelector
        {
            get; set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public XmlFormatter()
        {
            m_Converter = new FormatterConverter();
            AssemblyStyle = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full;
        }

        private IFormatterConverter m_Converter;
    }


}
