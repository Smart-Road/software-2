using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Server
{
    public partial class MainGui : Form
    {
        private ConnectionAccepter _connectionAccepter;

        private const int PortNumber = 13;

        public MainGui()
        {
            InitializeComponent();
            _connectionAccepter = new ConnectionAccepter(PortNumber);
            try
            {
                _connectionAccepter.StartAccepting();
                _connectionAccepter.ClientAccepted += _connectionAccepter_ClientAccepted;
            }
            catch (SocketException ex)
            {
                lbInfo.Items.Insert(0, $"SocketException:{ex.Message}");
            }

            nudRFIDSpeed.Minimum = Rfid.MinSpeed;
            nudRFIDSpeed.Maximum = Rfid.MaxSpeed;
            Database.PrepareDatabase();
        }

        private void _connectionAccepter_ClientAccepted(object sender, ConnectionUpdateEventArgs e)
        {
            AddToInfo($"{e.Client.Client.RemoteEndPoint} has connected");
        }

        private void btnAddToDatabase_Click(object sender, EventArgs e)
        {
            var serialNumber = tbRFIDNumber.Text;
            var speed = Convert.ToInt32(nudRFIDSpeed.Value);

            if (speed >= Rfid.MinSpeed && speed <= Rfid.MaxSpeed && !string.IsNullOrWhiteSpace(serialNumber) &&
                OutgoingConnection.GetInstance() != null)
            {
                var rfid = new Rfid(serialNumber, speed);
                string message = $"{Command.ADDRFID}:{rfid.ToNumberString()}";
                try
                {
                    OutgoingConnection.GetInstance().SendMessage(message);
                    AddToInfo($"Sent message:{message}, with RFID(hex):{rfid}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex);
                    AddToInfo("Network error");
                }
            }
            else if (OutgoingConnection.GetInstance() == null)
            {
                AddToInfo("Not yet connected, could not send command.");
            }

            OutgoingConnection.GetInstance()?.SendMessage($"{Command.SYNCDB}:{DatabaseWrapper.GetLatestTimestamp()}");
        }

        private void btnGetListOfRFID_Click(object sender, EventArgs e)
        {
            OutgoingConnection.GetInstance()?.SendMessage($"{Command.SYNCDB}:{DatabaseWrapper.GetLatestTimestamp()}");
        }


        private void BtnClear_Click(object sender, EventArgs e)
        {
            lbInfo.Items.Clear();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!OutgoingConnection.GetInstance()?.Connected ?? true)
            {
                try
                {
                    OutgoingConnection.CreateInstance(tbServerIp.Text, (int)nudPortnumber.Value, (int)nudZoneId.Value);
                    OutgoingConnection.GetInstance().ConnectionUpdate += OutgoingConnection_ConnectionUpdate;
                    OutgoingConnection.GetInstance().MakeConnection();
                }
                catch (FormatException)
                {
                    AddToInfo("Invalid ip address entered");
                }
            } else
            {
                OutgoingConnection.GetInstance().Disconnect();
            }
        }

        private void OutgoingConnection_ConnectionUpdate(object sender, ConnectionUpdateEventArgs e)
        {
            AddToInfo(e.ConnectionState ? "Got connection!" : "Got no connection :(");
            btnConnect.Invoke(new EventHandler(delegate
            {
                btnConnect.Text = e.ConnectionState ? "Disconnect" : "Connect";
                tbRFIDNumber_TextChanged(tbRFIDNumber, null);
                tbServerIp.Enabled = nudZoneId.Enabled = nudPortnumber.Enabled = !e.ConnectionState;
                btnGetListOfRFID.Enabled = btnUpdateRfid.Enabled = e.ConnectionState;
            }));
        }

        private void tbRFIDNumber_TextChanged(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            var valid = Rfid.ValidateRfid(textBox.Text);
            lblCheckSerialString.Text = valid ? "✔" : "X";
            lblCheckSerialString.ForeColor = valid ? Color.Green : Color.Red;
            btnAddToDatabase.Enabled = valid && (OutgoingConnection.GetInstance()?.Connected ?? false);
        }

        public void AddToInfo(string message)
        {
            lbInfo.Invoke(new EventHandler(delegate
            {
                var msgNumber = lbInfo.Items.Count + 1;
                lbInfo.Items.Insert(0, $"({msgNumber}) {message}");
            }));
        }

        private void btnUpdateRfid_Click(object sender, EventArgs e)
        {
            var updateRfid = new ChangeRfid(OutgoingConnection.GetInstance());
            updateRfid.Show();
        }

        private void btnEmptyDatabase_Click(object sender, EventArgs e)
        {
            var deletedEntries = DatabaseWrapper.DeleteAllEntries();
            var amountstring = deletedEntries != 1 ? "entries" : "entry";
            AddToInfo($"Deleted {deletedEntries} {amountstring} from database");
        }
    }
}
