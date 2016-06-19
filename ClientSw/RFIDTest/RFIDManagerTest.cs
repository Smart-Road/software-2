using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Client;

namespace ClientTest
{
    [TestClass]
    public class RfidManagerTest
    {
        private readonly Rfid testRfid;

        public RfidManagerTest()
        {
            testRfid = new Rfid("abcdefaa", 50);
        }

        [TestMethod]
        public void TestConstructor()
        {
            RfidManager rfidManager = new RfidManager();
            IEnumerable<Rfid> rfids = rfidManager.Rfids;
            Assert.IsInstanceOfType(rfids, typeof(IEnumerable<Rfid>));
            Assert.IsNotNull(rfids);
        }

        [TestMethod]
        public void TestAddDoubleRfidShouldNotAddSecond()
        {
            RfidManager rfidManager = new RfidManager();
            Assert.IsTrue(rfidManager.AddRfid(testRfid));
            Assert.IsFalse(rfidManager.AddRfid(testRfid));
        }

        [TestMethod]
        public void TestAddSameSerialNumberShouldNotAddSecond()
        {
            long serialNumber = 0xFFFFFFFFFF;
            RfidManager rfidManager = new RfidManager();
            Assert.IsTrue(rfidManager.AddRfid(new Rfid(serialNumber, 10)));
            Assert.IsFalse(rfidManager.AddRfid(new Rfid(serialNumber, 50)));
        }

        [TestMethod]
        public void TestRemoveNullRfid()
        {
            RfidManager rfidManager = new RfidManager();
            Assert.IsFalse(rfidManager.RemoveRfid(null));
        }

        [TestMethod]
        public void TestRemoveRfid()
        {
            RfidManager rfidManager = new RfidManager();
            Assert.IsTrue(rfidManager.AddRfid(testRfid));
            CollectionAssert.Contains(rfidManager.Rfids, testRfid);
            Assert.IsTrue(rfidManager.RemoveRfid(testRfid));
            CollectionAssert.DoesNotContain(rfidManager.Rfids, testRfid);
        }
    }
}
