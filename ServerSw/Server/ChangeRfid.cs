using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class ChangeRfid : Form
    {
        private readonly OutgoingConnection _outgoingConnection;
        public ChangeRfid(OutgoingConnection outgoingConnection)
        {
            InitializeComponent();
            _outgoingConnection = outgoingConnection;
            _outgoingConnection.ConnectionUpdate += _outgoingConnection_ConnectionUpdate;
            nudNewSpeed.Minimum = Rfid.MinSpeed;
            nudNewSpeed.Maximum = Rfid.MaxSpeed;
        }

        private void _outgoingConnection_ConnectionUpdate(object sender, ConnectionUpdateEventArgs e)
        {
            if (e.ConnectionState != ConnectionStatus.Connected)
            {
                this.Invoke(new EventHandler(delegate
                {
                    Close();
                }));
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            lbRfids.DataSource = DatabaseWrapper.LoadZoneFromDb(_outgoingConnection.Zone);
            lbRfids.Format += LbRfids_Format;
        }

        private void LbRfids_Format(object sender, ListControlConvertEventArgs e)
        {
            var item = (DatabaseEntry) e.ListItem;
            e.Value = item.SerialNumber.ToString("X2");
        }

        private void lbRfids_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedRfid = (DatabaseEntry)((ListBox) sender).SelectedItem;
            lblCurSpeed.Text = selectedRfid.Speed.ToString();
            lblSerialnumber.Text = selectedRfid.SerialNumber.ToString();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (lbRfids.SelectedItem == null)
            {
                return;
            }
            var entry = ((DatabaseEntry) lbRfids.SelectedItem);
            var serialNumber = entry.SerialNumber;
            var maxSpeed = (int) nudNewSpeed.Value;
            OutgoingConnection.GetInstance()?.SendMessage($"{Command.CHANGERFID}:{serialNumber},{maxSpeed}");
            OutgoingConnection.GetInstance()?.AskForSync();
        }
    }
}
