using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class DatabaseEntry
    {
        public readonly long SerialNumber;
        public readonly int Speed;
        public readonly long Timestamp;

        public DatabaseEntry(long serialNumber, int speed, long timestamp)
        {
            SerialNumber = serialNumber;
            Speed = speed;
            Timestamp = timestamp;
        }

        public bool CheckData()
        {
            return SerialNumber >= Rfid.MinHexSerialNumber &&
                   SerialNumber <= Rfid.MaxHexSerialNumber &&
                   Speed >= Rfid.MinSpeed &&
                   Speed <= Rfid.MaxSpeed &&
                   Timestamp > 0;
        }

        public Rfid GetRfid()
        {
            return new Rfid(SerialNumber, Speed);
        }

        public static bool CheckList(List<DatabaseEntry> entries)
        {
            return entries.All(databaseEntry => databaseEntry.CheckData()); // check all the entries for validity
        }
    }
}
