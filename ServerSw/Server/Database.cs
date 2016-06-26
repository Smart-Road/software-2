using System;
using System.Data.SQLite;
using System.IO;

namespace Server
{
    public static class Database
    {
        public const string TableName = "Rfids";
        public const string SerialNumber = "serialNumber";
        public const string Speed = "speed";
        public const string Timestamp = "timestamp";
        public const string Zone = "zone";
        public const string DatabaseFilename = "Rfid-db.sqlite";
        public const string ConnectionString = "Data Source = " + DatabaseFilename + ";Version=3";


        public static void PrepareDatabase()
        {
            var createNew = !File.Exists(DatabaseFilename);
            if (createNew)
            {
                SQLiteConnection.CreateFile(DatabaseFilename);
            }
            
            if (!createNew) return;

            CreateTable();
            //DatabaseWrapper.CreateDummyData();
        }

        private static void CreateTable()
        {
            using (var cn = new SQLiteConnection(ConnectionString))
            {
                cn.Open();
                using (var sqlCommand = cn.CreateCommand())
                {
                    sqlCommand.CommandText =
                        $"CREATE TABLE {TableName} ({SerialNumber} LONG PRIMARY KEY, {Speed} INT, {Zone} INT, {Timestamp} LONG)";
                    sqlCommand.ExecuteNonQuery();
                }
                cn.Close();
            }
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

