using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Data.SQLite;

namespace TCPlistener
{
    public class DatabaseWrapper
    {
        public static List<Rfid> LoadAllFromDatabase()
        {
            Database.Query = "SELECT * FROM Rfids";
            Database.OpenConnection();

            SQLiteDataReader reader = Database.Command.ExecuteReader();

            return readRfidsToList(reader);
        }

        public static List<Rfid> LoadZoneFromDb(int zone)
        {
            Database.Query = $"SELECT * FROM Rfids WHERE zone = {zone} ";
            Database.OpenConnection();

            SQLiteDataReader reader = Database.Command.ExecuteReader();

            return readRfidsToList(reader);
        }

        private static List<Rfid> readRfidsToList(SQLiteDataReader reader)
        {
            List<Rfid> Rfids = new List<Rfid>();
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
                Rfid rfid = new Rfid(serialNumber, speed); // TODO: error handling
                Rfids.Add(rfid);
                //Console.WriteLine($"zone:{zone}");
                //Console.WriteLine($"timestamp:{timestamp}");
            }
            return Rfids;
        }
    }
}