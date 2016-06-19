﻿using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class s : Form
    {
        private MessageHandler messageHandler;
        private Task connectionTask;
        private RfidManager rfidManager;

        public s()
        {
            InitializeComponent();
            messageHandler = new MessageHandler();
            nudRFIDSpeed.Minimum = Rfid.MinSpeed;
            nudRFIDSpeed.Maximum = Rfid.MaxSpeed;
            connectionTask = null;
            messageHandler.MessageReceived += messageHandler_MessageReceived;
            rfidManager = new RfidManager();
            rfidManager.CollectionChanged += RfidManagerOnCollectionChanged;
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
                messageHandler != null)
            {
                Rfid rfid = new Rfid(serialNumber, speed);
                string message = $"ADDRFID:{rfid.ToNumberString()}";
                try
                {
                    messageHandler.SendMessage(message);
                    AddToInfo($"Sent message:{message}, with RFID(hex):{rfid.SerialNumber}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex);
                    AddToInfo("Network error");
                }
                // for now, just add. TODO: Wait for server response before adding to own db
                rfidManager.AddRfid(rfid);
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
                        messageHandler.MessageReceived += messageHandler_MessageReceived;
                        int port = 13;
                        string errorMessage = null;
                        try
                        {
                            connectionTask = messageHandler.Connect(ipAddress: serverIp, port: port, zoneId: (int)nudZoneId.Value);
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
                        AddToInfo(messageHandler.Connected ? $"Connected to {serverIp}" : $"Could not connect: {errorMessage ?? "none"}");
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
            bool valid = Rfid.ValidateRfid(tbRFIDNumber.Text);

            lblCheckSerialString.Text = valid ? "Valid input" : "Invalid input";
            lblCheckSerialString.ForeColor = valid ? Color.Green : Color.Red;
            btnAddToDatabase.Enabled = valid && messageHandler.Connected;
        }

        private void AddToInfo(string message)
        {
            int msgNumber = lbInfo.Items.Count + 1;
            lbInfo.Items.Insert(0, $"({msgNumber}) {message}");
        }
    }
}
