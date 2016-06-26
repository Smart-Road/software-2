using System.ComponentModel;
using System.Net.Sockets;
using System.Net;
using System;

namespace Server
{
    public class OutgoingConnection
    {
        private BackgroundWorker _bwConnect = new BackgroundWorker();
        private CommandHandler _commandHandler = new CommandHandler();
        private MessageReceiver _messageReceiver;
        private TcpClient _client;
        private IPAddress _ipAddress;
        private int _portNumber;
        private int _zone;

        public event ConnectionUpdateDelegate ConnectionUpdate;
        public delegate void ConnectionUpdateDelegate(object sender, ConnectionUpdateEventArgs e);

        private bool _connected = false;
        public bool Connected
        {
            get
            {
                return _connected;
            }
            private set
            {
                _connected = value;
                OnConnectionUpdate(new ConnectionUpdateEventArgs(_connected));
            }
        }
        public OutgoingConnection(string sIpAdress, int portNumber, int zone)
        {
            _bwConnect.DoWork += _bwConnect_DoWork;
            _bwConnect.RunWorkerCompleted += _bwConnect_RunWorkerCompleted;
            _client = new TcpClient();
            _ipAddress = IPAddress.Parse(sIpAdress);
            _portNumber = portNumber;

            _zone = zone;
        }

        private void _bwConnect_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Connected = (bool)e.Result;
            if (Connected)
            {
                SendMessage($"{Command.ZONE}:{_zone}");
            }
            
        }

        private void _bwConnect_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = Connect();
            return;
        }

        public void MakeConnection()
        {
            if (Connected) return;
            _bwConnect.RunWorkerAsync();
        }

        private bool Connect()
        {
            try
            {
                _client.Connect(_ipAddress, _portNumber);
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            _messageReceiver = new MessageReceiver(_client);
            _messageReceiver.ClientDisconnected += _messageReceiver_ClientDisconnected;
            _commandHandler.AddEntry(_messageReceiver);
            return true;
        }

        private void _messageReceiver_ClientDisconnected(object sender, ConnectionLostEventArgs e)
        {
            Connected = false;
            _messageReceiver = null;
            _commandHandler = null;
        }

        public void Disconnect()
        {
            Connected = false;
            _messageReceiver = null;
            _commandHandler = null;
            _client.Close();
        }

        public void SendMessage(string message)
        {
            Console.WriteLine(message);
            _messageReceiver?.SendMessage(message);
        }

        protected virtual void OnConnectionUpdate(ConnectionUpdateEventArgs e)
        {
            ConnectionUpdate?.Invoke(this, e);
        }
    }

    public class ConnectionUpdateEventArgs : EventArgs
    {
        public bool ConnectionState { get; }
        public ConnectionUpdateEventArgs(bool connectionState)
        {
            ConnectionState = connectionState;
        }
    }
}