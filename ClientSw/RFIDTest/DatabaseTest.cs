using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;

namespace RFIDTest
{
    [TestClass]
    public class DatabaseTest
    {
        public DatabaseTest()
        {
            if (File.Exists(Database.DatabaseFilename))
            {
                File.Delete(Database.DatabaseFilename);
            }
            Database.PrepareDatabase();
        }

        [TestMethod]
        public void TestAddEntry()
        {
            Assert.IsTrue(Database.InsertData(new Rfid(Rfid.MinHexSerialNumber, 30)));
            Assert.IsTrue(DatabaseWrapper.AddEntry(new DatabaseEntry(Rfid.MinHexSerialNumber + 1, 110, Database.ConvertToTimestamp(DateTime.UtcNow))));
            Assert.IsTrue(Database.InsertData(new Rfid(Rfid.MinHexSerialNumber + 2, 40)));
        }
    }
}
