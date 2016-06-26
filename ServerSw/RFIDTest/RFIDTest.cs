using System;
using Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClientTest
{
    [TestClass]
    public class RfidTest
    {
        private long serialNumber = 0xaabbccdd;
        [TestMethod]
        public void TestConstructorValidSpeed()
        {
            Rfid rfid = new Rfid(serialNumber, 50);
        }
        [TestMethod]
        public void TestConstructorMinSpeed()
        {
            Rfid rfid = new Rfid(serialNumber, Rfid.MinSpeed);
        }

        [TestMethod]
        public void TestConstructorMaxSpeed()
        {
            Rfid rfid = new Rfid(serialNumber, Rfid.MaxSpeed);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestConstructorTooHighSpeed()
        {
            Rfid rfid = new Rfid(serialNumber, Rfid.MaxSpeed + 1);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void TestConstructorTooLowSpeed()
        {
            Rfid rfid = new Rfid(serialNumber, Rfid.MinSpeed - 1);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void TestConstructorSerialNumberWhitespace()
        {
            Rfid rfid = new Rfid("      ", Rfid.MaxSpeed);
        }

        [TestMethod]
        [ExpectedException(typeof (FormatException))]
        public void TestConstructorSerialNumberRightLengthInvalidFormat()
        {
            Rfid rfid = new Rfid("abcdefga", Rfid.MaxSpeed);
        }
    }
}
