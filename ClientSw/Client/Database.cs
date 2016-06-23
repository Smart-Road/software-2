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
        private static readonly string dbFileName = "[rootlocation]Database.sqlite";
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

            if (createNew)
            {
                CreateDummyData();
            }
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
    }
}
