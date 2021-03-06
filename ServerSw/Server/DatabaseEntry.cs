﻿using System.Collections.Generic;
using System.Linq;

namespace Server
{
    public class DatabaseEntry
    {
        public readonly long SerialNumber;
        public readonly int Speed;
        public readonly int Zone;
        public readonly long Timestamp;

        public DatabaseEntry(long serialNumber, int speed, int zone, long timestamp)
        {
            SerialNumber = serialNumber;
            Speed = speed;
            Zone = zone;
            Timestamp = timestamp;
        }

        public bool CheckData()
        {
            return SerialNumber >= Rfid.MinHexSerialNumber &&
                   SerialNumber <= Rfid.MaxHexSerialNumber &&
                   Speed >= Rfid.MinSpeed &&
                   Speed <= Rfid.MaxSpeed &&
                   Zone > -1 &&
                   Timestamp > -1;
        }

        public Rfid GetRfid()
        {
            return new Rfid(SerialNumber, Speed);
        }

        public static bool CheckList(List<DatabaseEntry> entries)
        {
            return entries.All(databaseEntry => databaseEntry.CheckData()); // check all the entries for validity
        }

        public override string ToString() => $"{SerialNumber},{Speed},{Zone},{Timestamp}";
    }
}