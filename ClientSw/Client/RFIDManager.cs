﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Client
{
    public class RfidManager
    {
        private readonly ObservableCollection<Rfid> rfids;
        public event CollectionChangedDelegate CollectionChanged;

        public delegate void CollectionChangedDelegate(object sender, EventArgs e);
        public ReadOnlyCollection<Rfid> Rfids
        {
            get { return new ReadOnlyCollection<Rfid>(rfids); }
        }

        public RfidManager()
        {
            rfids = new ObservableCollection<Rfid>();
            rfids.CollectionChanged += OnCollectionChanged; ;
        }

        protected virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }
        
        public bool AddRfid(Rfid rfid)
        {
            if (rfid == null)
            {
                throw new ArgumentException(nameof(rfid));
            }

            if (ContainsSerialNumber(rfid.SerialNumber))
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

        public bool ContainsSerialNumber(long serialNumber)
        {
            foreach (Rfid internalRfid in rfids)
            {
                if (serialNumber == internalRfid.SerialNumber)
                {
                    return true;
                }
            }
            return false;
        }
    }
}