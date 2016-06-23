using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Data.SQLite;

namespace Master_server
{
    public static class DatabaseWrapper
    {

        public static List<DatabaseEntry> LoadAllFromDatabase()
        {
            Database.Query = "SELECT * FROM Rfids";
            Database.OpenConnection();

            SQLiteDataReader reader = Database.Command.ExecuteReader();

            return readDataToList(reader);
        }

        public static List<DatabaseEntry> LoadZoneFromDb(int zone)
        {
            Database.Query = $"SELECT * FROM Rfids WHERE zone = {zone} ";
            Database.OpenConnection();

            SQLiteDataReader reader = Database.Command.ExecuteReader();

            return readDataToList(reader);
        }

        private static List<DatabaseEntry> readDataToList(SQLiteDataReader reader)
        {
            List<DatabaseEntry> databaseEntries = new List<DatabaseEntry>();
            while (reader.Read())
            {
                long serialNumber = (long)reader["serialNumber"];
                int speed = (int)reader["speed"];
                long timestamp = 0;
                int zone = (int) reader["zone"];
                object timestampobj = reader["timestamp"];
                if (timestampobj.GetType() != typeof(DBNull))
                {
                    timestamp = (long)reader["timestamp"];
                }
                var entry = new DatabaseEntry(serialNumber, speed, zone, timestamp);
                databaseEntries.Add(entry);
            }
            return databaseEntries;
        }

        private static long LongRandom(long min, long max, Random rand)
        {
            byte[] buf = new byte[8];
            rand.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);
            return (Math.Abs(longRand % (max - min)) + min);
        }

        /// <summary>
        /// Functie die een nieuwe tabel aanmaakt op een lege database, en deze vult met een
        /// aantal records.
        /// </summary>
        public static void CreateDummyData()
        {
            Database.OpenConnection();

            List<Rfid> rfidsAdded = new List<Rfid>();

            try
            {
                Random random = new Random();
                const int amountOfRows = 1000;
                for (int i = 0; i < amountOfRows; i++)
                {
                    long serialNumber = LongRandom(Rfid.MinHexSerialNumber, Rfid.MaxHexSerialNumber, random);
                    Rfid rfid = new Rfid(serialNumber, random.Next(Rfid.MinSpeed, Rfid.MaxSpeed));
                    int zone = random.Next(100, 500);
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