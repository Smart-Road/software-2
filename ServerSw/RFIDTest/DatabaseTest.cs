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
            int zone = 30;
            int speed = 120;
            Assert.IsTrue(DatabaseWrapper.InsertData(new Rfid(Rfid.MinHexSerialNumber, 30), zone));
            Assert.IsTrue(DatabaseWrapper.AddEntry(new DatabaseEntry(Rfid.MinHexSerialNumber + 1, speed, zone, Database.ConvertToTimestamp(DateTime.UtcNow))));
            Assert.IsTrue(DatabaseWrapper.InsertData(new Rfid(Rfid.MinHexSerialNumber + 2, speed), zone));
        }
    }
}
