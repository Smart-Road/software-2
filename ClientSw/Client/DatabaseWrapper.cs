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
        public static int GetSpeedFromDb(long serialNumber)
        {
            Database.Query = $"SELECT {Database.Speed} FROM {Database.TableName} WHERE {Database.SerialNumber} = {serialNumber}";
            Database.OpenConnection();
            IDataReader reader = Database.Command.ExecuteReader();

            int speed = -1;

            if (reader.Read())
            {
                if (reader[Database.Speed] is DBNull)
                {
                    // not found
                } else
                {
                    speed = (int)reader[Database.Speed];
                }
            }
            Database.CloseConnection();
            return speed;
        }

        public static List<DatabaseEntry> LoadAllFromDatabase()
        {
            Database.Query = "SELECT * FROM Rfids";
            Database.OpenConnection();

            IDataReader reader = Database.Command.ExecuteReader();

            return ReadDataToList(reader);
        }

        public static bool AddEntry(DatabaseEntry entry)
        {
            if (entry == null || !entry.CheckData())
            {
                return false;
            }
            Database.OpenConnection();
            var retval = Database.InsertData(new Rfid(entry.SerialNumber, entry.Speed), entry.Timestamp);
            Database.CloseConnection();
            return retval;
        }

        public static long GetLatestTimestamp()
        {
            Database.Query = $"SELECT MAX({Database.Timestamp}) FROM {Database.TableName}";
            Database.OpenConnection();
            IDataReader reader = Database.Command.ExecuteReader();

            long timestamp = 0;
            if (reader.Read() && reader.FieldCount == 1)
            {
                if (reader[0] is DBNull)
                {
                    // dont do anything
                } else
                {
                    timestamp = (long)reader[0];
                }
            }
            Database.CloseConnection();

            return timestamp;
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
            Database.CloseConnection();
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
