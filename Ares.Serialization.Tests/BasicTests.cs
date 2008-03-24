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
        private static readonly bool sUseFile = true;
        
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
            using (System.IO.Stream stream = CreateStream())
            {
                formatter.Serialize(stream, t);
                
                IFormatter formatter2 = CreateFormatter();
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                return formatter2.Deserialize<T>(stream);
            }
        }
        
        [Serializable]
        class SimpleRS {
            public int i;
            
            public override bool Equals(object obj)
            {
                if (obj is SimpleRS) return ((SimpleRS)obj).i == i;
                else return false;
            }
        }
        
        [Serializable]
        struct SimpleVS {
            public int i;
            
            public override bool Equals(object obj)
            {
                if (obj is SimpleVS) return Equals((SimpleVS) obj);
                else return false;
            }
            
            public bool Equals(SimpleVS obj)
            {
                return obj.i == i;
            }
            
            public override string ToString()
            {
                return "SimpleVS: i = " + i;
            }
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
            
            public override bool Equals(object obj)
            {
                if (obj is SimpleRI) return ((SimpleRI)obj).i == i;
                else return false;
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
            
            public override bool Equals(object obj)
            {
                if (obj is SimpleVI) return Equals((SimpleVI)obj);
                else return false;
            }
            
            public bool Equals(SimpleVI obj)
            {
                return obj.i == i;
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

        [Serializable]
        class BasicTypesRI : ISerializable
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
            
            public BasicTypesRI() {}
            private BasicTypesRI(SerializationInfo info, StreamingContext context)
            {
                b = info.GetBoolean("b");
                b2 = info.GetByte("b2");
                c = info.GetChar("c");
                dt = info.GetDateTime("dt");
                de = info.GetDecimal("de");
                d = info.GetDouble("d");
                i1 = info.GetInt16("i1");
                i2 = info.GetInt32("i2");
                i3 = info.GetInt64("i3");
                sb = info.GetSByte("sb");
                s = info.GetSingle("s");
                ts = info.GetValue<TimeSpan>("ts");
                ui1 = info.GetUInt16("ui1");
                ui2 = info.GetUInt32("ui2");
                ui3 = info.GetUInt64("ui3");
                st = info.GetString("st");
            }
            
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("b", b);
                info.AddValue("b2", b2);
                info.AddValue("c", c);
                info.AddValue("dt", dt);
                info.AddValue("de", de);
                info.AddValue("d", d);
                info.AddValue("i1", i1);
                info.AddValue("i2", i2);
                info.AddValue("i3", i3);
                info.AddValue("sb", sb);
                info.AddValue("s", s);
                info.AddValue("ts", ts);
                info.AddValue("ui1", ui1);
                info.AddValue("ui2", ui2);
                info.AddValue("ui3", ui3);
                info.AddValue("st", st);
            }
        }
        
        [Test]
        public void TestBasicTypesRI()
        {
            BasicTypesRI x = new BasicTypesRI();
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
            BasicTypesRI y = SerializationRoundtrip(x);
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
        
        [Serializable] class RS { public int a; }
        [Serializable] struct SS { public int b; }
        [Serializable] class RinRS { public RS r = new RS(); }
        [Serializable] class SinRS { public SS s = new SS(); }
        [Serializable] struct SinSS { public SS s; public int c; public SinSS(int j) { s = new SS(); c = 1; } }
        [Serializable] struct RinSS { public RS r; public RinSS(int j) { r = new RS(); } }
        
        [Serializable] 
        class OuterRS {
            public RinRS m1 = new RinRS();
            public RinRS m2 = new RinRS();
            public SinRS m3 = new SinRS();
            public SinRS m4 = new SinRS();
            public SinSS m5 = new SinSS(2);
            public SinSS m6 = new SinSS(2);
            public RinSS m7 = new RinSS(2);
            public RinSS m8 = new RinSS(2);
        }
        
        [Serializable]
        struct OuterSS {
            public RinRS m1;
            public RinRS m2;
            public SinRS m3;
            public SinRS m4;
            public SinSS m5;
            public SinSS m6;
            public RinSS m7;
            public RinSS m8;
            
            public OuterSS(int j)
            {
                m1 = new RinRS();
                m2 = new RinRS();
                m3 = new SinRS();
                m4 = new SinRS();
                m5 = new SinSS(3);
                m6 = new SinSS(3);
                m7 = new RinSS(3);
                m8 = new RinSS(3);
            }
        }
        
        [Test]
        public void TestOuterRS()
        {
            OuterRS x = new OuterRS();
            x.m1.r.a = 1;
            x.m2.r.a = 2;
            x.m3.s.b = 3;
            x.m4.s.b = 4;
            x.m5.s.b = 5;
            x.m6.s.b = 6;
            x.m7.r.a = 7;
            x.m8.r.a = 8;
            OuterRS y = SerializationRoundtrip(x);
            Assert.IsTrue(x.m1.r.a == y.m1.r.a);
            Assert.IsTrue(x.m2.r.a == y.m2.r.a);
            Assert.IsTrue(x.m3.s.b == y.m3.s.b);
            Assert.IsTrue(x.m4.s.b == y.m4.s.b);
            Assert.IsTrue(x.m5.s.b == y.m5.s.b);
            Assert.IsTrue(x.m6.s.b == y.m6.s.b);
            Assert.IsTrue(x.m7.r.a == y.m7.r.a);
            Assert.IsTrue(x.m8.r.a == y.m8.r.a);
        }

        [Test]
        public void TestOuterSS()
        {
            OuterSS x = new OuterSS(3);
            x.m1.r.a = 1;
            x.m2.r.a = 2;
            x.m3.s.b = 3;
            x.m4.s.b = 4;
            x.m5.s.b = 5;
            x.m6.s.b = 6;
            x.m7.r.a = 7;
            x.m8.r.a = 8;
            OuterSS y = SerializationRoundtrip(x);
            Assert.IsTrue(x.m1.r.a == y.m1.r.a);
            Assert.IsTrue(x.m2.r.a == y.m2.r.a);
            Assert.IsTrue(x.m3.s.b == y.m3.s.b);
            Assert.IsTrue(x.m4.s.b == y.m4.s.b);
            Assert.IsTrue(x.m5.s.b == y.m5.s.b);
            Assert.IsTrue(x.m6.s.b == y.m6.s.b);
            Assert.IsTrue(x.m7.r.a == y.m7.r.a);
            Assert.IsTrue(x.m8.r.a == y.m8.r.a);
        }

        [Serializable]
        class OuterRI : ISerializable {
            public RinRS m1;
            public SinRS m3;
            public SinSS m5;
            public RinSS m7;
            
            public OuterRI()
            {
                m1 = new RinRS();
                m3 = new SinRS();
                m5 = new SinSS(3);
                m7 = new RinSS(3);
            }
            
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("m1", m1);
                info.AddValue("m3", m3);
                info.AddValue("m5", m5);
                info.AddValue("m7", m7);
            }
            
            private OuterRI(SerializationInfo info, StreamingContext context)
            {
                m1 = info.GetValue<RinRS>("m1");
                m3 = info.GetValue<SinRS>("m3");
                m5 = info.GetValue<SinSS>("m5");
                m7 = info.GetValue<RinSS>("m7");
            }
        }
        
        [Test]
        public void TestOuterRI()
        {
            OuterRI x = new OuterRI();
            x.m1.r.a = 1;
            x.m3.s.b = 3;
            x.m5.s.b = 5;
            x.m5.c = 6;
            x.m7.r.a = 7;
            OuterRI y = SerializationRoundtrip(x);
            Assert.IsTrue(x.m1.r.a == y.m1.r.a);
            Assert.IsTrue(x.m3.s.b == y.m3.s.b);
            Assert.IsTrue(x.m5.c == y.m5.c);
            Assert.IsTrue(x.m5.s.b == y.m5.s.b);
            Assert.IsTrue(x.m7.r.a == y.m7.r.a);
        }
        
        [Serializable] class RIInRS { public SimpleRI ri = new SimpleRI(); }
        [Serializable] class VIInRS { public SimpleVI vi = new SimpleVI(3); }
        
        [Serializable]
        class RIInRI : ISerializable {
            public SimpleRI ri = new SimpleRI();
            public RIInRI() {}
            private RIInRI(SerializationInfo info, StreamingContext context)
            {
                ri = info.GetValue<SimpleRI>("ri");
            }
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("ri", ri);
            }
        }
        
        [Serializable]
        class VIInRI : ISerializable {
            public SimpleVI vi = new SimpleVI();
            public VIInRI() {}
            private VIInRI(SerializationInfo info, StreamingContext context)
            {
                vi = info.GetValue<SimpleVI>("vi");
            }
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("vi", vi);
            }
        }
        
        [Serializable]
        class RSInRI : ISerializable {
            public SimpleRS rs = new SimpleRS();
            public RSInRI() {}
            private RSInRI(SerializationInfo info, StreamingContext context)
            {
                rs = info.GetValue<SimpleRS>("rs");
            }
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("rs", rs);
            }
        }

        [Serializable]
        class VSInRI : ISerializable {
            public SimpleVS vs = new SimpleVS();
            public VSInRI() {}
            private VSInRI(SerializationInfo info, StreamingContext context)
            {
                vs = info.GetValue<SimpleVS>("vs");
            }
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("vs", vs);
            }
        }
        
        [Serializable]
        struct ComplexVI : ISerializable {
            public SimpleVS vs;
            public SimpleRS rs;
            public SimpleRI ri;
            public SimpleVI vi;
            public ComplexVI(int j)
            {
                vs = new SimpleVS();
                rs = new SimpleRS();
                ri = new SimpleRI();
                vi = new SimpleVI();
            }
            private ComplexVI(SerializationInfo info, StreamingContext context)
            {
                vs = info.GetValue<SimpleVS>("vs");
                rs = info.GetValue<SimpleRS>("rs");
                ri = info.GetValue<SimpleRI>("ri");
                vi = info.GetValue<SimpleVI>("vi");
            }
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("vs", vs);
                info.AddValue("rs", rs);
                info.AddValue("ri", ri);
                info.AddValue("vi", vi);
            }
        }
        
        [Serializable]
        struct ComplexVS {
            public SimpleVS vs;
            public SimpleRS rs;
            public SimpleRI ri;
            public SimpleVI vi;
            public ComplexVS(int j)
            {
                vs = new SimpleVS();
                rs = new SimpleRS();
                ri = new SimpleRI();
                vi = new SimpleVI();
            }
        }
        
        [Serializable]
        class ComplexRS 
        {
            public VSInRI m1 = new VSInRI();
            public RSInRI m2 = new RSInRI();
            public RIInRI m3 = new RIInRI();
            public VIInRI m4 = new VIInRI();
            public RIInRS m5 = new RIInRS();
            public VIInRS m6 = new VIInRS();
            public ComplexVI m7 = new ComplexVI(3);
            public ComplexVS m8 = new ComplexVS(3);
        }
        
        [Test]
        public void TestComplex()
        {
            ComplexRS x = new ComplexRS();
            x.m1.vs.i = 1;
            x.m2.rs.i = 2;
            x.m3.ri.i = 3;
            x.m4.vi.i = 4;
            x.m5.ri.i = 5;
            x.m6.vi.i = 6;
            x.m7.ri.i = 7;
            x.m7.rs.i = 8;
            x.m7.vi.i = 9;
            x.m7.vs.i = 10;
            x.m8.ri.i = 11;
            x.m8.rs.i = 12;
            x.m8.vi.i = 13;
            x.m8.vs.i = 14;
            ComplexRS y = SerializationRoundtrip(x);
            Assert.IsTrue(x.m1.vs.i == y.m1.vs.i );
            Assert.IsTrue(x.m2.rs.i == y.m2.rs.i );
            Assert.IsTrue(x.m3.ri.i == y.m3.ri.i );
            Assert.IsTrue(x.m4.vi.i == y.m4.vi.i );
            Assert.IsTrue(x.m5.ri.i == y.m5.ri.i );
            Assert.IsTrue(x.m6.vi.i == y.m6.vi.i );
            Assert.IsTrue(x.m7.ri.i == y.m7.ri.i );
            Assert.IsTrue(x.m7.rs.i == y.m7.rs.i );
            Assert.IsTrue(x.m7.vi.i == y.m7.vi.i );
            Assert.IsTrue(x.m7.vs.i == y.m7.vs.i );
            Assert.IsTrue(x.m8.ri.i == y.m8.ri.i );
            Assert.IsTrue(x.m8.rs.i == y.m8.rs.i );
            Assert.IsTrue(x.m8.vi.i == y.m8.vi.i );
            Assert.IsTrue(x.m8.vs.i == y.m8.vs.i );
        }
        
        [Serializable]
        class Base
        {
            public int i;
            public Base(int j) { i = j; }
        }
        
        [Serializable]
        class Derived : Base
        {
            public int j;
            public Derived(int i, int k) : base(i) { j = k; }
        }
        
        [Test]
        public void TestDerived()
        {
            Derived x = new Derived(3, 4);
            Derived y = SerializationRoundtrip(x);
            Assert.IsTrue(x.i == y.i);
            Assert.IsTrue(x.j == y.j);
        }
        
        [Serializable]
        class BaseI : ISerializable
        {
            public int i;
            public BaseI(int j) { i = j; }
            
            public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("i", i);
            }
            
            protected BaseI(SerializationInfo info, StreamingContext context)
            {
                i = info.GetInt32("i");
            }
        }
        
        [Serializable]
        class DerivedI : BaseI
        {
            public int j;
            public DerivedI(int i, int k) : base(i) { j = k; }
            
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue("j", j);
            }
            
            private DerivedI(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
                j = info.GetInt32("j");
            }
        }
        
        [Test]
        public void TestDerivedI()
        {
            DerivedI x = new DerivedI(3, 4);
            DerivedI y = SerializationRoundtrip(x);
            Assert.IsTrue(x.i == y.i);
            Assert.IsTrue(x.j == y.j);
        }
        
        [Serializable]
        class Graph1
        {
          public int i;
          public Graph2 g2;
          public Graph3 g3;
        }
        
        [Serializable]
        class Graph2
        {
          public int j;
          public Graph3 g3;
        }
        
        [Serializable]
        class Graph3
        {
            public int k;
            public Graph2 g2;
            public Graph1 g1;
        }
        
        [Test]
        public void TestGraph()
        {
            Graph1 g1 = new Graph1();
            g1.i = 3;
            g1.g2 = new Graph2();
            g1.g2.j = 4;
            g1.g3 = new Graph3();
            g1.g3.k = 5;
            g1.g2.g3 = g1.g3;
            g1.g3.g2 = g1.g2;
            g1.g3.g1 = g1;
            Graph1 y = SerializationRoundtrip(g1);
            Assert.IsTrue(g1.i == y.i);
            Assert.IsTrue(g1.g2.j == y.g2.j);
            Assert.IsTrue(g1.g3.k == y.g3.k);
            Assert.IsTrue(y.g2.g3 == y.g3);
            Assert.IsTrue(y.g3.g2 == y.g2);
            Assert.IsTrue(y.g3.g1 == y);
            Assert.IsTrue(y.g3 != g1.g3);
            Assert.IsTrue(y.g2 != g1.g2);
        }
        
        [Serializable]
        class NullRef
        {
            public int i;
            public Base x = null;
            public Nullable<SimpleVS> v = null;
        }
        
        [Test]
        public void TestNullRef()
        {
            NullRef x = new NullRef();
            x.i = 3;
            NullRef y = SerializationRoundtrip(x);
            Assert.IsTrue(x.i == y.i);
            Assert.IsTrue(x.x == y.x);
            Assert.IsTrue(x.v.HasValue == y.v.HasValue);
        }
        
        [Serializable]
        class NullRefS : ISerializable
        {
            public int i;
            public Base x = null;
            public Nullable<SimpleVS> v = null;
            
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("i", i);
                info.AddValue("x", x);
                info.AddValue("v", v);
            }
            
            public NullRefS() {}
            
            private NullRefS(SerializationInfo info, StreamingContext context)
            {
                i = info.GetInt32("i");
                x = (Base) info.GetValue("x", typeof(Base));
                // x = info.GetValue<Base>("x");
                v = null;
            }
        }
        
        [Test]
        public void TestNullRefS()
        {
            NullRefS x = new NullRefS();
            x.i = 3;
            NullRefS y = SerializationRoundtrip(x);
            Assert.IsTrue(x.i == y.i);
            Assert.IsTrue(x.x == y.x);
            Assert.IsTrue(x.v.HasValue == y.v.HasValue);
        }
        
        [Serializable]
        class WithArray
        {
            public int[] a = new int[3];
            public String[] b = new String[2];
            public SimpleVS[] c = new SimpleVS[2];
            public SimpleRS[] d = new SimpleRS[4];
            public SimpleVI[] e = new SimpleVI[1];
            public SimpleRI[] f = new SimpleRI[3];
        }
        
        private void TestArrayEquality<T>(T[] x, T[] y)
        {
            Assert.IsTrue(x.Rank == y.Rank);
            Assert.IsTrue(x.Length == y.Length);
            for (int i = 0; i < x.Rank; ++i)
            {
                Assert.IsTrue(x.GetLowerBound(i) == y.GetLowerBound(i));
                Assert.IsTrue(x.GetUpperBound(i) == y.GetUpperBound(i));
                Assert.IsTrue(x.GetLength(i) == y.GetLength(i));
            }
            for (int i = 0; i < x.Length; ++i)
            {
                Assert.AreEqual(x[i], y[i]);
            }
        }
        
        [Test]
        public void TestWithArray()
        {
            WithArray x = new WithArray();
            x.a[0] = 3;
            x.a[1] = 4;
            x.a[2] = 5;
            x.b[0] = "One";
            x.b[1] = "Two";
            x.c[0] = new SimpleVS();
            x.c[0].i = 6;
            x.c[1] = new SimpleVS();
            x.c[1].i = 7;
            x.d[0] = new SimpleRS();
            x.d[0].i = 8;
            x.d[1] = x.d[0];
            x.d[2] = null;
            x.d[3] = new SimpleRS();
            x.d[3].i = 9;
            x.e[0].i = 10;
            x.f[0] = new SimpleRI();
            x.f[0].i = 11;
            x.f[1] = x.f[0];
            x.f[2] = null;
            WithArray y = SerializationRoundtrip(x);
            TestArrayEquality(x.a, y.a);
            TestArrayEquality(x.b, y.b);
            TestArrayEquality(x.c, y.c);
            TestArrayEquality(x.d, y.d);
            TestArrayEquality(x.e, y.e);
            TestArrayEquality(x.f, y.f);
        }
        
        [Serializable]
        class ListsS
        {
            public List<int> l1 = new List<int>();
            public List<String> l2 = new List<string>();
            public List<Base> l3 = null;
        }
        
        [Test]
        public void TestListsS()
        {
            ListsS x = new ListsS();
            x.l1.Add(3);
            x.l1.Add(4);
            x.l2.Add("Eins");
            x.l2.Add("Zwei");
            x.l2.Add("Drei");
            ListsS y = SerializationRoundtrip(x);
            Assert.AreEqual(x.l1, y.l1);
            Assert.AreEqual(x.l2, y.l2);
            Assert.AreEqual(x.l3, y.l3);
        }
        
   }
}