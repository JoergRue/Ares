using System;
using System.Collections.Generic;
using NUnit.Framework;

using Ares.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Ares.Serialization.Tests
{
    
    [TestFixture]
    public class BasicTests
    {
        private static readonly bool sTestXML = true;
        private static readonly bool sUseFile = false;
        
        public static IFormatter CreateFormatter()
        {
            if (sTestXML) return new XmlFormatter(); else return new BinaryFormatter();
        }
        
        public static System.IO.Stream CreateStream()
        {
            if (sUseFile) return new System.IO.FileStream(@"C:\Test.xml", System.IO.FileMode.Create); 
            else return new System.IO.MemoryStream();
        }
        
        private static T SerializationRoundtrip<T>(T t)
        {
            IFormatter formatter = CreateFormatter();
            System.IO.Stream stream = CreateStream();
            formatter.Serialize(stream, t);
            
            IFormatter formatter2 = CreateFormatter();
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            return formatter2.Deserialize<T>(stream);
        }
        
        [Serializable]
        class SimpleRS {
            public int i;
        }
        
        [Serializable]
        struct SimpleVS {
            public int i;
        }
        
        [Serializable]
        class SimpleRI : ISerializable {
            public int i;
            
            public SimpleRI() {}
            
            private SimpleRI(SerializationInfo info, StreamingContext context)
            {
                i = info.GetInt32("i");
            }
            
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("i", i);
            }
        }
        
        [Serializable]
        struct SimpleVI : ISerializable {
            public int i;
            
            public SimpleVI(int j) { i = j; }
            
            private SimpleVI(SerializationInfo info, StreamingContext context)
            {
                i = info.GetInt32("i");
            }
            
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("i", i);
            }
        }
        
        
        
        [Test]
        public void TestSimpleRS()
        {
            SimpleRS x = new SimpleRS();
            x.i = 5;
            SimpleRS y = SerializationRoundtrip(x);
            Assert.IsTrue(x.i == y.i);
        }

        [Test]
        public void TestSimpleVS()
        {
            SimpleVS x = new SimpleVS();
            x.i = 5;
            SimpleVS y = SerializationRoundtrip(x);
            Assert.IsTrue(x.i == y.i);
        }

        [Test]
        public void TestSimpleRI()
        {
            SimpleRI x = new SimpleRI();
            x.i = 5;
            SimpleRI y = SerializationRoundtrip(x);
            Assert.IsTrue(x.i == y.i);
        }

        [Test]
        public void TestSimpleVI()
        {
            SimpleVI x = new SimpleVI(0);
            x.i = 5;
            SimpleVI y = SerializationRoundtrip(x);
            Assert.IsTrue(x.i == y.i);
        }
        
        [Serializable]
        class BasicTypesRS
        {
            public Boolean b;
            public Byte b2;
            public Char c;
            public DateTime dt;
            public Decimal de;
            public Double d;
            public Int16 i1;
            public Int32 i2;
            public Int64 i3;
            public SByte sb;
            public Single s;
            public TimeSpan ts;
            public UInt16 ui1;
            public UInt32 ui2;
            public UInt64 ui3;
            public String st;
        }
        
        [Test]
        public void TestBasicTypesRS()
        {
            BasicTypesRS x = new BasicTypesRS();
            x.b = true;
            x.b2 = 7;
            x.c = 'f';
            x.dt = DateTime.Now;
            x.de = 12.3M;
            x.d = -32.1;
            x.i1 = 4;
            x.i2 = 302;
            x.i3 = -3234;
            x.sb = -2;
            x.s = 0.6F;
            x.ts = TimeSpan.FromHours(3.2);
            x.ui1 = 15;
            x.ui2 = 22;
            x.ui3 = 3203;
            x.st = "Ein <a selt </element> s&uuml; a\"mer String";
            BasicTypesRS y = SerializationRoundtrip(x);
            Assert.IsTrue(x.b == y.b);
            Assert.IsTrue(x.b2 == y.b2);
            Assert.IsTrue(x.c == y.c);
            Assert.IsTrue(x.dt == y.dt);
            Assert.IsTrue(x.i1 == y.i1);
            Assert.IsTrue(x.i2 == y.i2);
            Assert.IsTrue(x.i3 == y.i3);
            Assert.IsTrue(x.sb == y.sb);
            Assert.IsTrue(x.ts == y.ts);
            Assert.AreEqual(x.de, y.de);
            Assert.AreEqual(x.d, y.d);
            Assert.AreEqual(x.s, y.s);
            Assert.IsTrue(x.ui1 == y.ui1);
            Assert.IsTrue(x.ui2 == y.ui2);
            Assert.IsTrue(x.ui3 == y.ui3);
            Assert.IsTrue(x.st == y.st);
        }

    }
}