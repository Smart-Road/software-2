using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Client
{
    public class RfidManager
    {
        private readonly List<Rfid> rfids;
        public ReadOnlyCollection<Rfid> Rfids
        {
            get { return new ReadOnlyCollection<Rfid>(rfids); }
        }

        public RfidManager()
        {
            rfids = new List<Rfid>();
        }
        
        public bool AddRfid(Rfid rfid)
        {
            if (rfid == null)
            {
                throw new ArgumentException(nameof(rfid));
            }
            if (rfids.Contains(rfid))
            {
                return false;
            }
            rfids.Add(rfid);
            return true;
        }

        public bool RemoveRfid(Rfid rfid)
        {
            return rfids.Remove(rfid);
        }
    }
}
