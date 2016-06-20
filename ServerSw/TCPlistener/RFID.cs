using System;
using System.Globalization;

namespace TCPlistener
{
    /*
     * The Rfid object holds a serial number of a chip, and a speed that is associated with it.
     */
    [Serializable]
    public class Rfid
    {
        public const int MaxSpeed = 130;
        public const int MinSpeed = 5;
        public const long MinHexSerialNumber = 0x10000000; // min length = 8
        public const long MaxHexSerialNumber = 0xFFFFFFFFFFFFFF; // max length = 14


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
            if (string.IsNullOrWhiteSpace(serialNumberString))
            {
                throw new ArgumentException(nameof(serialNumberString));
            }
            // method throws formatexception if string is invalid
            long serialNr = long.Parse(serialNumberString, NumberStyles.AllowHexSpecifier);

            if (serialNr < MinHexSerialNumber || serialNr > MaxHexSerialNumber)
            {
                throw new ArgumentException(nameof(serialNr));
            }
            this.serialNumber = serialNr;
            this.Speed = speed;
        }

        public override string ToString()
        {
            return $"{serialNumber.ToString("X8")},{speed}"; // for hexadecimal
        }

        public string ToNumberString()
        {
            return $"{serialNumber},{speed}";
        }

        public static bool ValidateRfid(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            long parsed;
            CultureInfo provider = CultureInfo.InvariantCulture;
            bool valid = long.TryParse(input, NumberStyles.AllowHexSpecifier, provider, out parsed);
            return valid && parsed >= MinHexSerialNumber && parsed <= MaxHexSerialNumber;
        }
    }
}
