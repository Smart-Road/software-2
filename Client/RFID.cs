using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    /*
     * The Rfid object holds a serial number of a chip, and a speed that is associated with it.
     */
    [Serializable]
    public class Rfid
    {
        public const int MaxSpeed = 130;
        public const int MinSpeed = 5;
        public const long MinHexSerialNumber = 268435456;
        public const long MaxHexSerialNumber = 4294967295;
        public const byte SerialNumberStringLength = 8;

        private readonly long serialNumber;

        public long SerialNumber => serialNumber;

        private int speed;
        public int Speed
        {
            get { return speed; }
            set
            {
                if (value < MinSpeed || value > MaxSpeed)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                speed = value;
            }
        }

        public Rfid(long serialNumber, int speed)
        {
            if (serialNumber < MinHexSerialNumber || serialNumber > MaxHexSerialNumber)
            {
                throw new ArgumentException(nameof(serialNumber));
            }
            this.serialNumber = serialNumber;
            this.Speed = speed;
        }

        public Rfid(string serialNumberString, int speed)
        {
            if (string.IsNullOrWhiteSpace(serialNumberString) || serialNumberString.Length != SerialNumberStringLength)
            {
                throw new ArgumentException(nameof(serialNumberString));
            }
            long serialNumber;
            //throws formatexception if string is invalid
            serialNumber = long.Parse(serialNumberString, NumberStyles.AllowHexSpecifier);

            if (serialNumber < MinHexSerialNumber || serialNumber > MaxHexSerialNumber)
            {
                throw new ArgumentException(nameof(serialNumber));
            }
            this.serialNumber = serialNumber;
            this.Speed = speed;
        }

        public override string ToString()
        {
            return $"{serialNumber.ToString("X8")},{speed}";
        }
    }
}
