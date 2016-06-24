using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SQLite;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Master_server
{
    public static class DatabaseWrapper
    {
        public static List<DatabaseEntry> LoadAllFromDatabase()
        {
            Database.Query = $"SELECT * FROM {Database.TableName}";
            Database.OpenConnection();

            var reader = Database.Command.ExecuteReader();

            return ReadDataToList(reader);
        }

        public static List<DatabaseEntry> LoadZoneFromDb(int zone)
        {
            Database.Query = $"SELECT * FROM {Database.TableName} WHERE {Database.Zone} = {zone} ";
            Database.OpenConnection();

            var reader = Database.Command.ExecuteReader();

            return ReadDataToList(reader);
        }

        public static List<DatabaseEntry> LoadZoneAfterTimeStamp(int zone, long timestamp)
        {
            Database.Query = $"SELECT * FROM {Database.TableName} WHERE {Database.Zone} = {zone} AND {Database.Timestamp} > {timestamp} ";
            Database.OpenConnection();

            var reader = Database.Command.ExecuteReader();

            return ReadDataToList(reader);
        }

        public static bool AddEntry(DatabaseEntry entry)
        {
            if (entry == null || !entry.CheckData()) return false;
            Database.OpenConnection();
            var retval = Database.InsertData(new Rfid(entry.SerialNumber, entry.Speed), entry.Zone);
            Database.CloseConnection();
            return retval;
        }

        public static bool AddRfid(Rfid rfid, int zone)
        {
            if (zone < 1)
            {
                return false;
            }

            Database.OpenConnection();
            var retval = Database.InsertData(rfid, zone);
            Database.CloseConnection();
            return retval;
        }

        private static List<DatabaseEntry> ReadDataToList(IDataReader reader)
        {
            var databaseEntries = new List<DatabaseEntry>();
            while (reader.Read())
            {
                var serialNumber = (long)reader[Database.SerialNumber];
                var speed = (int)reader[Database.Speed];
                long timestamp = 0;
                var zone = (int) reader[Database.Zone];
                var timestampobj = reader[Database.Timestamp];
                if (timestampobj.GetType() != typeof(DBNull))
                {
                    timestamp = (long) timestampobj;
                }
                var entry = new DatabaseEntry(serialNumber, speed, zone, timestamp);
                databaseEntries.Add(entry);
            }
            return databaseEntries;
        }

        private static long LongRandom(long min, long max, Random rand)
        {
            var buf = new byte[8];
            rand.NextBytes(buf);
            var longRand = BitConverter.ToInt64(buf, 0);
            return (Math.Abs(longRand % (max - min)) + min);
        }

        /// <summary>
        /// Functie die een nieuwe tabel aanmaakt op een lege database, en deze vult met een
        /// aantal records.
        /// </summary>
        public static void CreateDummyData()
        {
            Database.OpenConnection();

            var rfidsAdded = new List<Rfid>();

            try
            {
                var random = new Random();
                const int amountOfRows = 1000;
                for (var i = 0; i < amountOfRows; i++)
                {
                    var serialNumber = LongRandom(Rfid.MinHexSerialNumber, Rfid.MaxHexSerialNumber, random);
                    var rfid = new Rfid(serialNumber, random.Next(Rfid.MinSpeed, Rfid.MaxSpeed));
                    var zone = random.Next(100, 500);
                    if (!rfidsAdded.Contains(rfid))
                    {
                        Database.InsertData(rfid, zone);
                    }
                    rfidsAdded.Add(rfid);
                }
            }
            catch (SQLiteException)
            {
                // Er is iets mis gegaan: waarschijnlijk bestond de tabel al. Voor nu is er
                // verder geen foutafhandeling nodig.
            }
            Console.WriteLine("Created dummy data");
            Database.CloseConnection();
        }
    }
}