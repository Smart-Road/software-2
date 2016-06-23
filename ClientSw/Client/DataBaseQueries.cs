using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Client
{
    static class DataBaseQueries
    {
        public static List<Rfid> LoadAllFromDataBase()
        {
            Database.Query = "SELECT * FROM RFIDS";
            Database.OpenConnection();
            SQLiteDataReader reader = Database.Command.ExecuteReader();

            List<Rfid> RFID = new List<Rfid>();
            while (reader.Read())
            {
                RFID.Add(new Rfid(Convert.ToInt32(reader["nummer"]), Convert.ToInt32(reader["snelheid"])));
            }
            Database.CloseConnection();
            return RFID;
        }

        public static int GetCountFromDataBase()
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=" + Database.DbFileName + ";Version=3");
            string sq1 = "SELECT COUNT(*) FROM RFIDS";
            SQLiteCommand command = new SQLiteCommand(sq1, connection);
            connection.Open();
            int count = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return count;
        }

        public static bool SaveToDatabase(int nummer, int snelheid, int zone)
        {
            DateTimeOffset dto = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            int timestamp = Convert.ToInt32(dto);
            Database.Query = "INSERT INTO RFIDS (Timestamp, nummer, snelheid) values(" + timestamp + ", " + nummer 
                + ", " + snelheid + "," + zone + ")";

            Database.OpenConnection();

            bool success = false;
            try
            {
                Database.Command.ExecuteNonQuery();
                success = true;
            }
            catch (SQLiteException e)
            {
                if(e.ErrorCode == 19)
                {
                    return success;
                }
            }

            Database.CloseConnection();
            return success;
        }
        public static bool ReplaceExistingdatabase(List<Rfid> replacList)
        {
            Database.Query = "DELETE * FROM RFIDS";
            Database.OpenConnection();
            bool success = false;

            try
            {
                Database.Command.ExecuteNonQuery();
            }
            catch (SQLiteException)
            {
                return success;
            }
            int count = DataBaseQueries.GetCountFromDataBase() + 1;
            foreach (Rfid r in replacList)
            {
                Database.Query = "INSERT INTO RFIDS (count,nummer,snelheid) values (" + count + ", " + r.SerialNumber
                                 + ", " + r.Speed + ")";

                Database.OpenConnection();


                try
                {
                    Database.Command.ExecuteNonQuery();
                    success = true;
                }
                catch (SQLiteException e)
                {
                   if (e.ErrorCode == 19)
                        {
                            return success;
                        }       
                }
            }
            Database.CloseConnection();
            return success;
        }
        public static bool DeleteSelectionFromDatabase(Rfid RF)
        {
            bool success = false;
            // Voer een select-query uit om alle kunsten uit te lezen
            Database.Query = "SELECT * FROM RFIDS";
            Database.OpenConnection();

            // De resultaten worden nu opgeslagen in een "reader": deze wordt in de while-loop
            // verderop gebruikt om nieuwe instanties van kunsten aan te maken
            SQLiteDataReader reader = Database.Command.ExecuteReader();

            // Onderstaande list bevat alle kunsten die uitgelezen worden
            List<Rfid> listRFID = new List<Rfid>();
            while (reader.Read())
            {
                listRFID.Add(new Rfid(Convert.ToInt32(reader["nummer"]), Convert.ToInt32("snelheid")));
            }
            Database.CloseConnection();
            foreach (Rfid rfid in listRFID)
            {
                if (rfid.SerialNumber == RF.SerialNumber)
                {
                    listRFID.Remove(rfid);
                }
            }
            foreach (Rfid r in listRFID)
            {
                Database.Query = "INSERT INTO RFIDS (nummer,snelheid) values (" + r.SerialNumber
                                 + "', " + r.Speed + ")";

                Database.OpenConnection();


                try
                {
                    // ExecuteNonQuery wordt gebruikt als we geen gegevens verwachten van de query
                    Database.Command.ExecuteNonQuery();
                    success = true;
                }
                catch (SQLiteException e)
                {
                    if (e.ErrorCode == 19) //niet voor ons project zo??
                    {
                        return success;
                    }
                }
            }
            Database.CloseConnection();
            return success;
        }
    }
}
