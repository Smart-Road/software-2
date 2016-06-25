using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class MainGui : Form
    {
        private ConnectionAccepter connectionAccepter;
        public OutgoingConnection OutgoingConnection { get; private set; }

        private const int PortNumber = 13;

        public static MainGui Main { get; private set; }

        public MainGui()
        {
            InitializeComponent();
            connectionAccepter = new ConnectionAccepter(PortNumber);
            try
            {
                connectionAccepter.StartAccepting();
            }
            catch (SocketException ex)
            {
                lbInfo.Items.Insert(0, $"SocketException:{ex.Message}");
            }

            nudRFIDSpeed.Minimum = Rfid.MinSpeed;
            nudRFIDSpeed.Maximum = Rfid.MaxSpeed;
            Main = this;
            Database.PrepareDatabase();
        }

        private void messageHandler_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            AddToInfo(e.Message);
        }



        private void btnAddToDatabase_Click(object sender, EventArgs e)
        {
            var serialNumber = tbRFIDNumber.Text;
            var speed = Convert.ToInt32(nudRFIDSpeed.Value);

            if (speed >= Rfid.MinSpeed && speed <= Rfid.MaxSpeed && !string.IsNullOrWhiteSpace(serialNumber) &&
                OutgoingConnection != null)
            {
                var rfid = new Rfid(serialNumber, speed);
                string message = $"{Command.ADDRFID}:{rfid.ToNumberString()}";
                try
                {
                    OutgoingConnection.SendMessage(message);
                    AddToInfo($"Sent message:{message}, with RFID(hex):{rfid}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex);
                    AddToInfo("Network error");
                }
            }
            else if (OutgoingConnection == null)
            {
                AddToInfo("Not yet connected, could not send command.");
            }

            OutgoingConnection?.SendMessage($"{Command.SYNCDB}:{DatabaseWrapper.GetLatestTimestamp()}");
        }

        private void btnGetListOfRFID_Click(object sender, EventArgs e)
        {
            OutgoingConnection?.SendMessage($"{Command.SYNCDB}:{DatabaseWrapper.GetLatestTimestamp()}");
        }


        private void BtnClear_Click(object sender, EventArgs e)
        {
            lbInfo.Items.Clear();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!OutgoingConnection?.Connected ?? true)
            {
                try
                {
                    OutgoingConnection = new OutgoingConnection(tbServerIp.Text, (int)nudPortnumber.Value, (int)nudZoneId.Value);
                    OutgoingConnection.ConnectionUpdate += OutgoingConnection_ConnectionUpdate;
                    OutgoingConnection.MakeConnection();
                }
                catch (FormatException)
                {
                    AddToInfo("Invalid ip address entered");
                }
            } else
            {
                OutgoingConnection?.Disconnect();
                OutgoingConnection = null;
            }
        }

        private void OutgoingConnection_ConnectionUpdate(object sender, ConnectionUpdateEventArgs e)
        {
            AddToInfo(e.ConnectionState ? "Got connection!" : "Got no connection :(");
            btnConnect.Invoke(new EventHandler(delegate
            {
                btnConnect.Text = e.ConnectionState ? "Disconnect" : "Connect";
                tbRFIDNumber_TextChanged(tbRFIDNumber, null);
            }));
        }

        private void tbRFIDNumber_TextChanged(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            var valid = Rfid.ValidateRfid(textBox.Text);
            lblCheckSerialString.Text = valid ? "✔" : "X";
            lblCheckSerialString.ForeColor = valid ? Color.Green : Color.Red;
            btnAddToDatabase.Enabled = valid && (OutgoingConnection?.Connected ?? false);
        }

        public void AddToInfo(string message)
        {
            lbInfo.Invoke(new EventHandler(delegate
            {
                var msgNumber = lbInfo.Items.Count + 1;
                lbInfo.Items.Insert(0, $"({msgNumber}) {message}");
            }));
        }
    }
}
