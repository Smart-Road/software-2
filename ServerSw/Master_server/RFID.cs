using System;
using System.Globalization;
using System.Linq.Expressions;

namespace Master_server
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

        private readonly long _serialNumber;

        public long SerialNumber => _serialNumber;

        private int _speed;
        public int Speed
        {
            get { return _speed; }
            set
            {
                if (value < MinSpeed || value > MaxSpeed)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _speed = value;
            }
        }

        public Rfid(long serialNumber, int speed)
        {
            if (serialNumber < MinHexSerialNumber || serialNumber > MaxHexSerialNumber)
            {
                throw new ArgumentException(nameof(serialNumber));
            }
            this._serialNumber = serialNumber;
            this.Speed = speed;
        }

        public Rfid(string serialNumberString, int speed)
        {
            if (string.IsNullOrWhiteSpace(serialNumberString))
            {
                throw new ArgumentException(nameof(serialNumberString));
            }
            // constructor throws formatexception if string is invalid
            var serialNr = long.Parse(serialNumberString, NumberStyles.AllowHexSpecifier);

            if (serialNr < MinHexSerialNumber || serialNr > MaxHexSerialNumber)
            {
                throw new ArgumentException(nameof(serialNr));
            }
            this._serialNumber = serialNr;
            this.Speed = speed;
        }

        public override string ToString()
        {
            return $"{_serialNumber.ToString("X8")},{_speed}"; // for hexadecimal
        }

        public string ToNumberString()
        {
            return $"{_serialNumber},{_speed}";
        }

        public static bool GetRfid(long serialNumber, int maxSpeed, out Rfid rfid)
        {
            try
            {
                rfid = new Rfid(serialNumber, maxSpeed);
            }
            catch (FormatException)
            {
                rfid = null;
                return false;
            }
            catch (ArgumentException)
            {
                rfid = null;
                return false;
            }
            return true;
        }

        public static bool ValidateRfid(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            long parsed;
            var provider = CultureInfo.InvariantCulture;
            var valid = long.TryParse(input, NumberStyles.AllowHexSpecifier, provider, out parsed);
            return valid && parsed >= MinHexSerialNumber && parsed <= MaxHexSerialNumber;
        }
    }
}
