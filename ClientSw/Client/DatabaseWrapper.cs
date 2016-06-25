using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Threading;

namespace Client
{
    public static class DatabaseWrapper
    {
        public static int GetSpeedFromDb(long serialNumber)
        {
            var speed = -1;
            using (var dbConnection = new SQLiteConnection(Database.ConnectionString))
            {
                dbConnection.Open();
                using (
                    var sqlCommand =
                        new SQLiteCommand(
                            $"SELECT {Database.Speed} FROM {Database.TableName} WHERE {Database.SerialNumber} = {serialNumber};",
                            dbConnection))
                {
                    IDataReader reader = sqlCommand.ExecuteReader();

                    if (!reader.Read()) return speed;
                    if (!(reader[Database.Speed] is DBNull))
                    {
                        speed = (int)reader[Database.Speed];
                    }
                    else
                    {
                        // not found
                    }
                }
                dbConnection.Close();
            }

            return speed;
        }

        public static List<DatabaseEntry> LoadAllFromDatabase()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                connection.Open();
                Console.WriteLine("Open db");
                List<DatabaseEntry> allDatabaseEntries;
                using (var command = new SQLiteCommand($"SELECT * FROM {Database.TableName}", connection))
                {
                    IDataReader reader = command.ExecuteReader();
                    allDatabaseEntries = ReadDataToList(reader);
                }
                connection.Close();
                Console.WriteLine("Close db");
                return allDatabaseEntries;
            }
        }

        public static bool AddEntry(DatabaseEntry entry)
        {
            if (entry == null || !entry.CheckData())
            {
                return false;
            }

            var retval = Database.InsertData(new Rfid(entry.SerialNumber, entry.Speed), entry.Timestamp);
            return retval;
        }

        public static int AddEntries(List<DatabaseEntry> entries)
        {
            if (entries == null || entries.Count < 1 || !DatabaseEntry.CheckList(entries))
            {
                return -1;
            }

            
            var results = new List<int>();
            try
            {
                using (var cn = new SQLiteConnection(Database.ConnectionString))
                {
                    Console.WriteLine("Open db");
                    cn.Open();
                    using (var command = cn.CreateCommand())
                    {
                        using (var transaction = cn.BeginTransaction())
                        {
                            command.Transaction = transaction;
                            command.CommandText =
                                $"INSERT INTO {Database.TableName} " +
                                $"({Database.SerialNumber},{Database.Speed},{Database.Timestamp}) " +
                                "VALUES (@SerialNumber,@Speed,@Timestamp);";
                            command.Prepare();

                            foreach (var databaseEntry in entries)
                            {
                                command.Parameters.AddWithValue("@SerialNumber", databaseEntry.SerialNumber);
                                command.Parameters.AddWithValue("@Speed", databaseEntry.Speed);
                                command.Parameters.AddWithValue("@Timestamp", databaseEntry.Timestamp);
                                results.Add(command.ExecuteNonQuery());
                            }
                            transaction.Commit();
                            // TODO: fix the bug where the db is locked
                        }
                    }
                    cn.Close();
                    Console.WriteLine("Close db");
                }
            }
            catch (SQLiteException ex)
            {
                MainGui.Main.AddToInfo($"SQL exception: {ex.Message}");
            }
            return results.Sum();
        }

        public static long GetLatestTimestamp()
        {
            long timestamp = 0;
            var sqlQuery = $"SELECT MAX({Database.Timestamp}) FROM {Database.TableName};";
            try
            {
                using (var cn = new SQLiteConnection(Database.ConnectionString))
                {
                    cn.Open();
                    Console.WriteLine("Open db");
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = sqlQuery;
                        var obj = cmd.ExecuteScalar();
                        if (!(obj is DBNull))
                        {
                            timestamp = (long) obj;
                        }
                        
                        
                        //while (reader.Read()) ; // read everything, regardless if we care about the information
                    }
                    cn.Close();
                    Console.WriteLine("Close db");
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex);
            }

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
            var databaseEntries = new List<DatabaseEntry>();

            var random = new Random();
            const int amountOfRows = 1000;
            for (var i = 0; i < amountOfRows; i++)
            {
                var serialNumber = LongRandom(Rfid.MinHexSerialNumber, Rfid.MaxHexSerialNumber, random);
                var rfid = new Rfid(serialNumber, random.Next(Rfid.MinSpeed, Rfid.MaxSpeed));
                var entry = new DatabaseEntry(rfid.SerialNumber, rfid.Speed,
                    Database.ConvertToTimestamp(DateTime.Now));
                if (!databaseEntries.Contains(entry))
                {
                    databaseEntries.Add(entry);
                }
            }
            var addedEntries = AddEntries(databaseEntries);
            Console.WriteLine(addedEntries > 0 ? "Created dummy data" : "Dummy data creation failed");
        }
    }
}
