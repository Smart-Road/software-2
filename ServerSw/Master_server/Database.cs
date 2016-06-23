using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Master_server
{

    public static class Database
    {
        public const string TableName = "Rfids";
        public const string SerialNumber = "serialNumber";
        public const string Speed = "speed";
        public const string Zone = "zone";
        public const string Timestamp = "timestamp";

        // De bestandsnaam voor de database
        private static readonly string databaseFilename = "Database.sqlite";
        // Variabele voor het opzetten van de verbinding
        private static SQLiteConnection _connection;
        // Variable waar de SQL-commandos tijdelijk in opgeslagen worden
        private static SQLiteCommand _command;

        /// <summary>
        /// Stel de SQL query in die uitgevoerd moet gaan worden.
        /// </summary>
        public static string Query
        {
            set
            {
                // Zorg ervoor dat er een verbinding gemaakt kan worden
                PrepareConnection();
                // Stel het SQL commando in met de gegeven query
                _command = new SQLiteCommand(value, _connection);
            }
        }

        /// <summary>
        /// Haalt het command-object op waarmee queries uitgevoerd kunnen worden.
        /// </summary>
        public static SQLiteCommand Command
        {
            get { return _command; }
        }

        /// <summary>
        /// Haal de bestandsnaam op van de database.
        /// </summary>
        public static string DatabaseFilename
        {
            get { return databaseFilename; }
        }

        /// <summary>
        /// Open de verbinding met de database
        /// </summary>
        public static void OpenConnection()
        {
            // Controleer of de verbinding niet al open is
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        /// <summary>
        /// Sluit de verbinding met de database
        /// </summary>
        public static void CloseConnection()
        {
            // Controleer of de verbinding niet al gesloten is
            if (_connection.State != ConnectionState.Closed)
            {
                _connection.Close();
            }
        }

        /// <summary>
        /// Controleert of de database al bestaat. Zo niet, wordt deze aangemaakt
        /// en gevuld met wat dummy data. Daarnaast wordt altijd de connectie opgezet
        /// met de database indien deze nog niet opgezet was.
        /// </summary>
        public static void PrepareConnection()
        {
            // Controleer of we een nieuwe database met dummy data moeten aanmaken
            var createNew = !File.Exists(databaseFilename);

            // Bestand bestaat niet: maak een lege database aan
            if (createNew)
            {
                SQLiteConnection.CreateFile(databaseFilename);

            }

            // Zet een verbinding op met de database
            if (_connection == null)
            {
                _connection = new SQLiteConnection("Data Source=" + databaseFilename + ";Version=3");
            }

            // Als we een nieuwe database gemaakt hebben, voegen we alvast wat records toe.
            // We doen dit nu pas omdat we een connection nodig hebben om te communiceren met
            // de database: vandaar dat deze code niet boven bij de CreateFile functie staat.
            if (!createNew) return;

            CreateTable();
            //DatabaseWrapper.CreateDummyData();
        }

        private static void CreateTable()
        {
            OpenConnection();
            Query = $"CREATE TABLE {TableName} ({SerialNumber} LONG PRIMARY KEY, {Speed} INT, {Zone} INT, {Timestamp} LONG)";
            Command.ExecuteNonQuery();
            CloseConnection();
        }

        // do not forget to open and close the database when using this method
        public static bool InsertData(Rfid rfid, int zone, long timestamp = -1)
        {
            try
            {
                var longDate = timestamp < 0 ? DateTime.UtcNow.ToFileTimeUtc() : timestamp;
                Query = $"INSERT INTO {TableName} ({SerialNumber}, {Speed}, {Zone}, {Timestamp}) VALUES ({rfid.SerialNumber}, {rfid.Speed}, {zone}, {longDate})";
                Command.ExecuteNonQuery();
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex);
                MainGui.Main.AddInfoToLb($"SQL exception: {ex.Message}");
                return false;
            }
        }
    }
}

