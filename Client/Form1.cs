using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        private MessageHandler messageHandler;
        private Task connectionTask;
        private int messageNumber;

        public Form1()
        {
            InitializeComponent();
            messageHandler = new MessageHandler(); ;
            nudRFIDSpeed.Minimum = Rfid.MinSpeed;
            nudRFIDSpeed.Maximum = Rfid.MaxSpeed;
            tbRFIDNumber.MaxLength = Rfid.SerialNumberStringLength;
            connectionTask = null;
            messageNumber = 1;
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
                    AddToInfo($"Sent message:{message}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex);
                    AddToInfo("Network error");
                }
            }
            else if (messageHandler == null)
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
            messageNumber = 1;
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (connectionTask?.Status != TaskStatus.WaitingForActivation)
            {
                string serverIp = tbServerIp.Text.Trim();
                if (serverIp.Length != 0)
                {
                    if (!messageHandler.Connected)
                    {
                        AddToInfo("Trying to connect.. a moment please.");
                        messageHandler = new MessageHandler();
                        int port = 80;
                        string errorMessage = null;
                        try
                        {
                            connectionTask = messageHandler.Connect(ipAddress: serverIp, port: port);
                            await connectionTask;
                        }
                        catch (InvalidOperationException invalidOperationException)
                        {
                            errorMessage = invalidOperationException.Message;
                            Console.WriteLine(invalidOperationException.ToString());
                        }
                        catch (SocketException socketException)
                        {
                            errorMessage = socketException.Message;
                            Console.WriteLine(socketException.ToString());
                        }
                        errorMessage = errorMessage ?? "none";
                        AddToInfo(messageHandler.Connected ? $"Connected to {serverIp}" : $"Could not connect: {errorMessage}");
                    }
                    else
                    {
                        messageHandler.Disconnect();
                        AddToInfo("Disconnected");
                    }
                    btnConnect.Text = messageHandler.Connected ? "Disconnect" : "Connect";
                    RfidSerialNumberInputValidator();
                }
                else
                {
                    AddToInfo("Enter ip adress");
                }
            }
            else
            {
                AddToInfo("Please wait until the method is complete.");
            }
        }

        private void tbRFIDNumber_TextChanged(object sender, EventArgs e)
        {
            RfidSerialNumberInputValidator();
        }

        //check if serialnumber text is valid and messagehandler is connected, if so, make the send button available
        private void RfidSerialNumberInputValidator()
        {
            bool valid = false;
            string input = tbRFIDNumber.Text;
            if (input.Length >= Rfid.SerialNumberStringLengthMin && input.Length <= Rfid.SerialNumberStringLengthMax)
            {
                long parsed;
                CultureInfo provider = CultureInfo.CurrentCulture;
                valid = long.TryParse(input, NumberStyles.AllowHexSpecifier, provider, out parsed);
            }
            lblCheckSerialString.Text = valid ? "Valid input" : "Invalid input";
            lblCheckSerialString.ForeColor = valid ? Color.Green : Color.Red;
            btnAddToDatabase.Enabled = valid && messageHandler.Connected;
        }

        private void AddToInfo(string message)
        {
            lbInfo.Items.Insert(0, $"({messageNumber}) {message}");
            messageNumber++;
        }
    }
}
