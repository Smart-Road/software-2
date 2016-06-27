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
        private readonly CommandHandler _commandHandler = new CommandHandler();

        private const int PortNumber = 13;

        public MainGui()
        {
            InitializeComponent();
            _connectionAccepter = new ConnectionAccepter(PortNumber, _commandHandler);
            try
            {
                _connectionAccepter.StartAccepting();
                _connectionAccepter.ClientAccepted += _connectionAccepter_ClientAccepted;
                _connectionAccepter.ClientDisconnected += _connectionAccepter_ClientDisconnected;
                _commandHandler.CommandHandlerCallback += _commandHandler_CommandHandlerCallback;
                _commandHandler.CommandReceived += _commandHandler_CommandReceived;
            }
            catch (SocketException ex)
            {
                lbInfo.Items.Insert(0, $"SocketException:{ex.Message}");
            }

            nudRFIDSpeed.Minimum = Rfid.MinSpeed;
            nudRFIDSpeed.Maximum = Rfid.MaxSpeed;
            Database.PrepareDatabase();
        }

        private void _commandHandler_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            //AddToInfo($"Command received:{e.Command}:{e.Parameter}");
        }

        private void _commandHandler_CommandHandlerCallback(object sender, CommandHandledEventArgs e)
        {
            if (e.Valid)
            {
                AddToInfo(e.Message);
            }
        }

        private void _connectionAccepter_ClientDisconnected(object sender, ConnectionLostEventArgs e)
        {
            AddToInfo($"Client disconnected:{e.RemoteEndPoint}");
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

            OutgoingConnection.GetInstance()?.AskForSync();
        }

        private void btnGetListOfRFID_Click(object sender, EventArgs e)
        {
            OutgoingConnection.GetInstance()?.AskForSync();
        }


        private void BtnClear_Click(object sender, EventArgs e)
        {
            lbInfo.Items.Clear();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (OutgoingConnection.GetInstance()?.ConnectionState != ConnectionStatus.Connected)
            {
                try
                {
                    OutgoingConnection.CreateInstance(tbServerIp.Text, (int)nudPortnumber.Value, (int)nudZoneId.Value, _commandHandler);
                    OutgoingConnection.GetInstance().ConnectionUpdate += OutgoingConnection_ConnectionUpdate;
                    OutgoingConnection.GetInstance().MakeConnection();
                }
                catch (FormatException)
                {
                    AddToInfo("Invalid ip address entered");
                }
            } else
            {
                syncTimer.Stop();
                OutgoingConnection.GetInstance().Disconnect();
            }
        }

        private void OutgoingConnection_ConnectionUpdate(object sender, ConnectionUpdateEventArgs e)
        {
            var connected = e.ConnectionState == ConnectionStatus.Connected;
            switch (e.ConnectionState)
            {
                case ConnectionStatus.Disconnected:
                    AddToInfo("Disconnected");
                    break;
                case ConnectionStatus.Connected:
                    AddToInfo("Got connection");
                    break;
                case ConnectionStatus.ConnectionLost:
                    AddToInfo("Connection lost");
                    break;
                case ConnectionStatus.UnableToConnect:
                    AddToInfo("Could not connect");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            btnConnect.Invoke(new EventHandler(delegate
            {
                btnConnect.Text = connected ? "Disconnect" : "Connect";
                tbRFIDNumber_TextChanged(tbRFIDNumber, null);
                tbServerIp.Enabled = nudZoneId.Enabled = nudPortnumber.Enabled = !connected;
                btnGetListOfRFID.Enabled = btnUpdateRfid.Enabled = connected;
                if (connected)
                {
                    syncTimer.Start();
                }
                else
                {
                    syncTimer.Stop();
                }
            }));
        }

        private void tbRFIDNumber_TextChanged(object sender, EventArgs e)
        {
            var textBox = (TextBox) sender;
            var valid = Rfid.ValidateRfid(textBox.Text);
            lblCheckSerialString.Text = valid ? "✔" : "X";
            lblCheckSerialString.ForeColor = valid ? Color.Green : Color.Red;
            btnAddToDatabase.Enabled = valid && (OutgoingConnection.GetInstance()?.ConnectionState == ConnectionStatus.Connected);
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

        private void syncTimer_Tick(object sender, EventArgs e)
        {
            OutgoingConnection.GetInstance()?.AskForSync();
        }
    }
}
