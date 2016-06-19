using System;
using System.Globalization;

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
        public const long MinHexSerialNumber = 0x10000000;
        public const long MaxHexSerialNumber = 0xFFFFFFFFFFFF;
        public const byte SerialNumberStringLengthMin = 8;
        public const byte SerialNumberStringLengthMax = 12;

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
            if (string.IsNullOrWhiteSpace(serialNumberString) || 
                serialNumberString.Length < SerialNumberStringLengthMin || 
                serialNumberString.Length > SerialNumberStringLengthMax)
            {
                throw new ArgumentException(nameof(serialNumberString));
            }
            long serialNumber;
            // method throws formatexception if string is invalid
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
            return $"{serialNumber.ToString("X8")},{speed}"; // for hexadecimal
        }

        public string ToNumberString()
        {
            return $"{serialNumber},{speed}";
        }

        public static bool ValidateRfid(string input)
        {
            bool valid = false;
            if (input.Length >= SerialNumberStringLengthMin && input.Length <= SerialNumberStringLengthMax)
            {
                long parsed;
                CultureInfo provider = CultureInfo.CurrentCulture;
                valid = long.TryParse(input, NumberStyles.AllowHexSpecifier, provider, out parsed);
                if (parsed < Rfid.MinHexSerialNumber || parsed > Rfid.MaxHexSerialNumber)
                {
                    valid = false;
                }
            }
            return valid;
        }
    }
}
