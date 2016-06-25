using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Client
{

    public static class Database
    {
        public const string TableName = "Rfids";
        public const string SerialNumber = "serialNumber";
        public const string Speed = "speed";
        public const string Timestamp = "timestamp";

        // De bestandsnaam voor de database
        // Variabele voor het opzetten van de verbinding
        private static SQLiteConnection _connection;
        // Variable waar de SQL-commandos tijdelijk in opgeslagen worden
        private static SQLiteCommand _command;

        /// <summary>
        /// Haal de bestandsnaam op van de database.
        /// </summary>
        public static string DatabaseFilename { get; } = "Rfid-db.sqlite";

        public static string ConnectionString { get; } = $"Data Source = {DatabaseFilename}; Version=3";


        public static void PrepareDatabase()
        {
            var createNew = !File.Exists(DatabaseFilename);
            if (createNew)
            {
                SQLiteConnection.CreateFile(DatabaseFilename);
            }
            
            if (!createNew) return;

            CreateTable();
        }

        private static void CreateTable()
        {
            using (var cn = new SQLiteConnection(ConnectionString))
            {
                cn.Open();
                Console.WriteLine("Open db");
                using (var sqlCommand = cn.CreateCommand())
                {
                    sqlCommand.CommandText =
                        $"CREATE TABLE {TableName} ({SerialNumber} LONG PRIMARY KEY, {Speed} INT, {Timestamp} LONG)";
                    sqlCommand.ExecuteNonQuery();
                }
                cn.Close();
                Console.WriteLine("Close db");
            }
        }
        
        public static bool InsertData(Rfid rfid, long timestamp = -1)
        {
            int written = 0;
            try
            {
                using (var cn = new SQLiteConnection(ConnectionString))
                {
                    cn.Open();
                    Console.WriteLine("Open db");
                    var longDate = timestamp < 0 ? ConvertToTimestamp(DateTime.UtcNow) : timestamp;
                    using (var sqlCommand = cn.CreateCommand())
                    {
                        sqlCommand.CommandText =
                            $"INSERT INTO {TableName} ({SerialNumber}, {Speed}, {Timestamp}) VALUES ({rfid.SerialNumber}, {rfid.Speed}, {longDate})";
                        written = sqlCommand.ExecuteNonQuery();
                    }
                    cn.Close();
                    Console.WriteLine("Close db");
                }
            }
            catch (SQLiteException e)
            {
                return false;
            }
            return written > 0;
        }

        public static long ConvertToTimestamp(DateTime value)
        {
            //create Timespan by subtracting the value provided from
            //the Unix Epoch
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());

            //return the total seconds (which is a UNIX timestamp)
            return (long)span.TotalSeconds;
        }
    }
}

