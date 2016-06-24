using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;

namespace Client
{
    public static class Database
    {
        private static readonly string dbFileName = "Database.sqlite";
        private static SQLiteConnection connection;
        private static SQLiteCommand command;

        public static string Query
        {
            set
            {
                PrepareConnection();
                command = new SQLiteCommand(value, connection);
            }
        }

        public static SQLiteCommand Command
        {
            get { return command; }
        }

        public static string DbFileName
        {
            get { return dbFileName; }
        }

        public static void OpenConnection()
        {
            if(connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }
        }

        public static void CloseConnection()
        {
            if(connection.State != System.Data.ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        public static void PrepareConnection()
        {
            bool createNew = !File.Exists(@"[rootlocation]Database.sqlite");

            if(createNew)
            {
                SQLiteConnection.CreateFile(dbFileName);
            }

            if(connection == null)
            {
                connection = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3");
            }

            if (!createNew) return;

            CreateTable();
            //DatabaseWrapper met dummydata?
        }

        private static void CreateTable()
        {
            OpenConnection();
            Query = "CREATE TABLE Rfids (serialNumber LONG PRIMARY KEY, speed INT, zone INT, timestamp LONG)";
            Command.ExecuteNonQuery();
            CloseConnection();
        }

        private static void CreateDummyData()
        {
            OpenConnection();

            try
            {
                Query = "CREATE TABLE RIFDS (Timestamp INT PRIMARY KEY, Nummer varchar(30), Snelheid INT)";
                Command.ExecuteNonQuery();
            }
            catch(SQLiteException)
            {

            }

            CloseConnection();
        }

        public static bool InsertData(Rfid rfid, int zone, long timestamp = -1)
        {
            bool retval;

            try
            {
                var longDate = timestamp == -1 ? DateTime.UtcNow.ToFileTimeUtc() : timestamp;
                Query = $"INSERT INTO Rfids (serialNumber, speed, zone, timestamp) VALUES ({rfid.SerialNumber}, {rfid.Speed}, {zone}, {longDate})";
                Command.ExecuteNonQuery();
                retval = true;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex);
                retval = false;
            }

            return retval;
        }
    }
}
