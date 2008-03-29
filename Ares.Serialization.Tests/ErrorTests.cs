using System;
using System.Collections.Generic;
using NUnit.Framework;

using Ares.Serialization;
using System.Runtime.Serialization;

namespace Ares.Serialization.Tests
{
    [TestFixtureAttribute]
    [Category("Errors")]
    public class ErrorTests
    {
        private static object GeneralSerializationRoundtrip(Object o)
        {
            IFormatter formatter = BasicTests.CreateFormatter();
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            formatter.Serialize(stream, o);
            
            IFormatter formatter2 = BasicTests.CreateFormatter();
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            return formatter2.Deserialize(stream);
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestTopLevelNull()
        {
            Object x = null;
            Object y = GeneralSerializationRoundtrip(x);
            Assert.IsNull(y);
        }
        
        class NonSerializable { public int i = 3; }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestNonSerializable()
        {
            NonSerializable x = new NonSerializable();
            Object y = GeneralSerializationRoundtrip(x);
        }
        
        [Serializable]
        class NestedNonSerializable { public NonSerializable i = new NonSerializable(); }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestNestedNonSerializable()
        {
            NestedNonSerializable x = new NestedNonSerializable();
            Object y = GeneralSerializationRoundtrip(x);
        }
        
        [Serializable] 
        class NestedNonSerializableNull { public NonSerializable i = null; }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestNestedNonSerializableNull()
        {
            NestedNonSerializableNull x = new NestedNonSerializableNull();
            Object y = GeneralSerializationRoundtrip(x);
        }
        
        [Serializable]
        class NestedNonSerializableP { public NonSerializable I { get; set; } }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestNestedNonSerializableP1()
        {
            NestedNonSerializableP x = new NestedNonSerializableP();
            Object y = GeneralSerializationRoundtrip(x);
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestNestedNonSerializableP2()
        {
            NestedNonSerializableP x = new NestedNonSerializableP();
            x.I = new NonSerializable();
            Object y = GeneralSerializationRoundtrip(x);
        }
        
        [Serializable]
        class NestedNonSerializableI : ISerializable
        {
            public NonSerializable x;
            
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("x", x);
            }
            
            public NestedNonSerializableI() {}
            
            private NestedNonSerializableI(SerializationInfo info, StreamingContext context)
            {
                info.GetValue("x", out x);
            }
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestNestedNonSerializableI1()
        {
            NestedNonSerializableI x = new NestedNonSerializableI();
            x.x = new NonSerializable();
            Object o = GeneralSerializationRoundtrip(x);
        }
        
        [Test]
        // [ExpectedException(typeof(SerializationException))] // no exception here!
        public void TestNestedNonSerializableI2()
        {
            NestedNonSerializableI x = new NestedNonSerializableI();
            x.x = null;
            Object o = GeneralSerializationRoundtrip(x);
            NestedNonSerializableI y = (NestedNonSerializableI) o;
            Assert.IsNull(y.x);
        }
        
        [Serializable]
        class MissingConstructor : ISerializable
        {
            public int i = 3;
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("i", i);
            }
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestMissingConstructor()
        {
            MissingConstructor x = new MissingConstructor();
            Object o = GeneralSerializationRoundtrip(x);
        }
        
        [Serializable]
        class WrongKey : ISerializable
        {
            public int i = 3;
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("i", i);
            }
            public WrongKey() {}
            private WrongKey(SerializationInfo info, StreamingContext context)
            {
                i = info.GetInt32("I");
            }
        }
        
        [Test]
        [ExpectedException(typeof(System.Reflection.TargetInvocationException))]
        public void TestWrongKey()
        {
            WrongKey x = new WrongKey();
            Object o = GeneralSerializationRoundtrip(x);
        }
        
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestWriteNullStream()
        {
            IFormatter formatter = BasicTests.CreateFormatter();
            formatter.Serialize(null, new WrongKey());
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReadNullStream()
        {
            IFormatter formatter = BasicTests.CreateFormatter();
            object o = formatter.Deserialize(null);
        }
    }
    
    [TestFixture]
    [Category("Error Files")]
    public class ErrorFilesTests
    {
        private static readonly string s_BasePath = @"..\..\ErrorFiles\";
        
        private static object ReadFromFile(String fileName)
        {
            IFormatter formatter = BasicTests.CreateFormatter();
            using (System.IO.FileStream stream = new System.IO.FileStream(s_BasePath + fileName, System.IO.FileMode.Open))
            {
                return formatter.Deserialize(stream);
            }
        }
        
        [Test]
        public void TestNoError()
        {
            ReadFromFile("NoError.xml");
        }

        [Test]
        public void TestNoError2()
        {
            ReadFromFile("NoError2.xml");
        }

        [Test]
        public void TestNoObjects()
        {
            Object o = ReadFromFile("NoObjects.xml");
            Assert.IsNull(o);
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestDefectXML()
        {
            ReadFromFile("DefectXML.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongAssembly()
        {
            ReadFromFile("WrongAssembly.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongType()
        {
            ReadFromFile("WrongType.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongVersion()
        {
            ReadFromFile("WrongVersion.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongReference()
        {
            ReadFromFile("WrongReference.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongMemberType()
        {
            ReadFromFile("WrongMemberType.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongMemberName()
        {
            ReadFromFile("WrongMemberName.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongMemberContent()
        {
            ReadFromFile("WrongMemberContent.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongCulture()
        {
            ReadFromFile("WrongCulture.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongKey()
        {
            ReadFromFile("WrongKey.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongObjectID()
        {
            ReadFromFile("WrongObjectID.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongObjectID2()
        {
            ReadFromFile("WrongObjectID2.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongTypeRef()
        {
            ReadFromFile("WrongTypeRef.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongTypeRef2()
        {
            ReadFromFile("WrongTypeRef2.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongTypeID()
        {
            ReadFromFile("WrongTypeID.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongTypeID2()
        {
            ReadFromFile("WrongTypeID2.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongTypeID3()
        {
            ReadFromFile("WrongTypeID3.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void MissingTypeID()
        {
            ReadFromFile("MissingTypeID.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void MissingType()
        {
            ReadFromFile("MissingType.xml");
        }
        
        [Test]
        // [ExpectedException(typeof(SerializationException))]
        public void MissingKey()
        {
            ReadFromFile("MissingKey.xml");
        }
        
        [Test]
        // [ExpectedException(typeof(SerializationException))]
        public void MissingVersion()
        {
            ReadFromFile("MissingVersion.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void MissingAssembly()
        {
            ReadFromFile("MissingAssembly.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void MissingAssembly2()
        {
            ReadFromFile("MissingAssembly2.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void MissingObjectID()
        {
            ReadFromFile("MissingObjectID.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void MissingTypeRef()
        {
            ReadFromFile("MissingTypeRef.xml");
        }

        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void MissingMemberName()
        {
            ReadFromFile("MissingMemberName.xml");
        }

        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void MissingMemberContent()
        {
            ReadFromFile("MissingMemberContent.xml");
        }

        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void MissingMemberType()
        {
            ReadFromFile("MissingMemberType.xml");
        }

        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void MissingMember()
        {
            ReadFromFile("MissingMember.xml");
        }

        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void MissingObject()
        {
            ReadFromFile("MissingObject.xml");
        }

        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongArrayBounds()
        {
            ReadFromFile("WrongArrayBounds.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongArrayBounds2()
        {
            ReadFromFile("WrongArrayBounds2.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongArrayBounds3()
        {
            ReadFromFile("WrongArrayBounds3.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongArrayBounds4()
        {
            ReadFromFile("WrongArrayBounds4.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongArrayBounds5()
        {
            ReadFromFile("WrongArrayBounds5.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongArrayBounds6()
        {
            ReadFromFile("WrongArrayBounds6.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestMissingDimension()
        {
            ReadFromFile("MissingDimension.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestMissingDimensions()
        {
            ReadFromFile("MissingDimensions.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestMissingElement()
        {
            ReadFromFile("MissingElement.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestMissingElements()
        {
            ReadFromFile("MissingElements.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestMissingRank()
        {
            ReadFromFile("MissingRank.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongRank()
        {
            ReadFromFile("WrongRank.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestWrongRank2()
        {
            ReadFromFile("WrongRank2.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestMissingArray()
        {
            ReadFromFile("MissingArray.xml");
        }
        
        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void TestMissingValueType()
        {
            ReadFromFile("MissingValueType.xml");
        }
        
    }
}
