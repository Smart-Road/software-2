using System;
using Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RFIDTest
{
    [TestClass]
    public class DatabaseEntryTest
    {
        [TestMethod]
        public void TestCheckData()
        {
            DatabaseEntry entry = new DatabaseEntry(4799212214943128, 30, 1, 500000);
            Assert.IsTrue(entry.CheckData());
        }
    }
}
