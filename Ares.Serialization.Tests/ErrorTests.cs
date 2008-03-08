using System;
using System.Collections.Generic;
using NUnit.Framework;

using Ares.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Ares.Serialization.Tests
{
    [TestFixtureAttribute]
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
        
    }
}
