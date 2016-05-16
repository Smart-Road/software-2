using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Client
{
    public partial class Form1 : Form
    {
        private bool connected;
        private MessageHandler messageHandler;

        public Form1()
        {
            InitializeComponent();
            messageHandler = new MessageHandler(); ;
            connected = false;
            nudRFIDSpeed.Minimum = Rfid.MinSpeed;
            nudRFIDSpeed.Maximum = Rfid.MaxSpeed;
        }

        private void btnAddToDatabase_Click(object sender, EventArgs e)
        {
            string serialNumber = tbRFIDNumber.Text;
            int speed = Convert.ToInt32(nudRFIDSpeed.Value);

            if (speed >= Rfid.MinSpeed && speed <= Rfid.MaxSpeed && !string.IsNullOrWhiteSpace(serialNumber) &&
                messageHandler != null)
            {
                Rfid rfid = new Rfid(serialNumber, speed);
                string message = $"ADD:{rfid}";
                try
                {
                    messageHandler.SendMessage(message);
                    lbInfo.Items.Add($"Sent message:{message}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex);
                    lbInfo.Items.Add("Network error");
                }
            }
            else if (messageHandler == null)
            {
                lbInfo.Items.Add("Not yet connected, could not send command.");
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
            if (!connected)
            {
                string serverIP = "localhost";
                int port = 80;
                try
                {
                    messageHandler.Connect(ipAddress: serverIP, port: port);
                    connected = true;
                }
                catch (InvalidOperationException invalidOperationException)
                {
                    Console.WriteLine(invalidOperationException.ToString());

                }
                catch (SocketException socketException)
                {
                    Console.WriteLine(socketException.ToString());
                }
                lbInfo.Items.Add(connected ? $"Connected to {serverIP}" : "Could not connect");
                btnConnect.Text = connected ? "Disconnect" : "Connect";
            }
            else
            {
                messageHandler.Disconnect();
                messageHandler = null;
                connected = false;
                btnConnect.Text = "Connect";
            }
        }

    private void tbRFIDNumber_TextChanged(object sender, EventArgs e)
    {
        bool valid = false;
        string input = tbRFIDNumber.Text;
        Console.WriteLine(input.Length);
        if (input.Length == Rfid.SerialNumberStringLength)
        {
            long parsed;
            CultureInfo provider = CultureInfo.CurrentCulture;
            valid = long.TryParse(input, NumberStyles.AllowHexSpecifier, provider, out parsed);
        }
        lblCheckSerialString.Text = valid ? "Valid input" : "Invalid input";
        lblCheckSerialString.ForeColor = valid ? Color.Green : Color.Red;
        btnAddToDatabase.Enabled = valid;
    }
}
}
