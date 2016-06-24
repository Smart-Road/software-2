using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;

namespace Client
{
    public static class DatabaseWrapper
    {

        public static List<DatabaseEntry> LoadAllFromDatabase()
        {
            Database.Query = "SELECT * FROM Rfids";
            Database.OpenConnection();

            IDataReader reader = Database.Command.ExecuteReader();

            return ReadDataToList(reader);
        }

        public static bool AddEntry(DatabaseEntry entry)
        {
            if (entry == null || !entry.CheckData()) return false;
            Database.OpenConnection();
            var retval = Database.InsertData(new Rfid(entry.SerialNumber, entry.Speed));
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
                var timestampobj = reader[Database.Timestamp];
                if (timestampobj.GetType() != typeof(DBNull))
                {
                    timestamp = (long)timestampobj;
                }
                var entry = new DatabaseEntry(serialNumber, speed, timestamp);
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
