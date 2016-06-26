using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;

namespace TcpListenerTest
{
    [TestClass]
    public class DatabaseEntryTest
    {
        [TestMethod]
        public void TestCheckingOfValidData()
        {
            long validSerialNumber = 0xFFFFFFAA15;
            int validSpeed = 10;
            int validZone = 50;
            long validTimestamp = DateTime.UtcNow.ToFileTimeUtc();
            var entry = new DatabaseEntry(validSerialNumber, validSpeed, validZone, validTimestamp);
            List<DatabaseEntry> entries = new List<DatabaseEntry>();
            for (int i = 0; i < 100; i++)
            {
                entries.Add(entry);
            }
            Assert.IsTrue(DatabaseEntry.CheckList(entries));
        }

        [TestMethod]
        public void TestCheckingOfInvalidData()
        {
            long validSerialNumber = 0xFFFFFFAA15;
            int validSpeed = 10;
            int validZone = 50;
            long validTimestamp = DateTime.UtcNow.ToFileTimeUtc();

            long invalidSerialnumber = 0xFF;
            int invalidSpeed = 3;
            int invalidZone = -1;
            long invalidTimestamp = -1;

            var entry = new DatabaseEntry(invalidSerialnumber, validSpeed, validZone, validTimestamp);
            var entry2 = new DatabaseEntry(validSerialNumber, invalidSpeed, validZone, validTimestamp);
            var entry3 = new DatabaseEntry(validSerialNumber, validSpeed, invalidZone, validTimestamp);
            var entry4 = new DatabaseEntry(validSerialNumber, validSpeed, validZone, invalidTimestamp);

            Assert.IsFalse(entry.CheckData());
            Assert.IsFalse(entry2.CheckData());
            Assert.IsFalse(entry3.CheckData());
            Assert.IsFalse(entry4.CheckData());

            List<DatabaseEntry> entries = new List<DatabaseEntry> {entry, entry2, entry3, entry4};

            Assert.IsFalse(DatabaseEntry.CheckList(entries));
        }
    }
}
