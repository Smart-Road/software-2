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
        private RfidManager rfidManager;
        private OutgoingConnection outgoingConnection;

        private const int portnumber = 13;

        public static MainGui Main { get; private set; }

        public MainGui()
        {
            InitializeComponent();
            connectionAccepter = new ConnectionAccepter(portnumber);
            connectionAccepter.StartAccepting();

            nudRFIDSpeed.Minimum = Rfid.MinSpeed;
            nudRFIDSpeed.Maximum = Rfid.MaxSpeed;
            rfidManager = new RfidManager();
            rfidManager.CollectionChanged += RfidManagerOnCollectionChanged;
            Main = this;
        }

        private void RfidManagerOnCollectionChanged(object sender, EventArgs eventArgs)
        {
            lbRfids.Items.Clear();
            foreach (var rfid in rfidManager.Rfids)
            {
                lbRfids.Items.Add(rfid);
            }
        }

        private void messageHandler_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            AddToInfo(e.Message);
        }



        private void btnAddToDatabase_Click(object sender, EventArgs e)
        {
            string serialNumber = tbRFIDNumber.Text;
            int speed = Convert.ToInt32(nudRFIDSpeed.Value);

            if (speed >= Rfid.MinSpeed && speed <= Rfid.MaxSpeed && !string.IsNullOrWhiteSpace(serialNumber) &&
                outgoingConnection != null)
            {
                Rfid rfid = new Rfid(serialNumber, speed);
                string message = $"ADDRFID:{rfid.ToNumberString()}";
                try
                {
                    outgoingConnection.SendMessage(message);
                    AddToInfo($"Sent message:{message}, with RFID(hex):{rfid}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex);
                    AddToInfo("Network error");
                }
                // for now, just add. TODO: Wait for server response before adding to own db
                rfidManager.AddRfid(rfid);
            }
            else if (outgoingConnection == null)
            {
                AddToInfo("Not yet connected, could not send command.");
            }
        }

        private void btnGetListOfRFID_Click(object sender, EventArgs e)
        {
            RfidManager rfidManager = new RfidManager();
        }


        private void BtnClear_Click(object sender, EventArgs e)
        {
            lbInfo.Items.Clear();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!outgoingConnection?.Connected ?? true)
            {
                try
                {
                    outgoingConnection = new OutgoingConnection(tbServerIp.Text, portnumber, (int)nudZoneId.Value);
                    outgoingConnection.ConnectionUpdate += OutgoingConnection_ConnectionUpdate;
                    outgoingConnection.MakeConnection();
                }
                catch (FormatException)
                {
                    AddToInfo("Invalid ip address entered");
                }
            } else
            {
                outgoingConnection?.Disconnect();
                outgoingConnection = null;
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
            lblCheckSerialString.Text = valid ? "Valid input" : "Invalid input";
            lblCheckSerialString.ForeColor = valid ? Color.Green : Color.Red;
            btnAddToDatabase.Enabled = valid && (outgoingConnection?.Connected ?? false);
        }

        public void AddToInfo(string message)
        {
            lbInfo.Invoke(new EventHandler(delegate
            {
                int msgNumber = lbInfo.Items.Count + 1;
                lbInfo.Items.Insert(0, $"({msgNumber}) {message}");
            }));
        }
    }
}
