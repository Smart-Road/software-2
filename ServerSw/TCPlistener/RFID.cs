using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPlistener
{
    class RFID
    {
        private int nummer { get; set; }
        private int snelheid { get; set; }

        public RFID(int Nummer, int Snelheid)
        {
            nummer = Nummer;
            snelheid = Snelheid;
        }
          public static List<RFID> LoadAllFromDatabase()
        {
                // Voer een select-query uit om alle kunsten uit te lezen
            Database.Query = "SELECT * FROM kunsten ORDER BY nummer";
            Database.OpenConnection();

            // De resultaten worden nu opgeslagen in een "reader": deze wordt in de while-loop
            // verderop gebruikt om nieuwe instanties van kunsten aan te maken
            SQLiteDataReader reader = Database.Command.ExecuteReader();

            // Onderstaande list bevat alle kunsten die uitgelezen worden
            List<RFID> kunsten = new List<RFID>();
            while (reader.Read())
            {
        
          }
    }
}
