using System.ComponentModel;
using System.Net.Sockets;
using System.Net;
using System;

namespace Server
{
    /// <summary>
    /// Singleton class that handles the connection to the master-server
    /// </summary>
    public class OutgoingConnection
    {
        private readonly BackgroundWorker _bwConnect = new BackgroundWorker();
        private readonly TcpClient _client;
        private readonly IPAddress _ipAddress;
        private readonly int _portNumber;
        private CommandHandler _commandHandler;
        private MessageReceiver _messageReceiver;
        private static OutgoingConnection _outgoingConnection;


        public int Zone { get; }

        public EndPoint RemoteEndPoint => _client?.Client.RemoteEndPoint;

        public event ConnectionUpdateDelegate ConnectionUpdate;
        public delegate void ConnectionUpdateDelegate(object sender, ConnectionUpdateEventArgs e);

        private ConnectionStatus _connectionState;

        public ConnectionStatus ConnectionState
        {
            get { return _connectionState; }
            private set
            {
                _connectionState = value;
                OnConnectionUpdate(new ConnectionUpdateEventArgs(_connectionState, _client));
            }
        }

        private OutgoingConnection(string sIpAdress, int portNumber, int zone, CommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
            _bwConnect.DoWork += _bwConnect_DoWork;
            _bwConnect.RunWorkerCompleted += _bwConnect_RunWorkerCompleted;
            _client = new TcpClient();
            _ipAddress = IPAddress.Parse(sIpAdress);
            _portNumber = portNumber;
            _connectionState = ConnectionStatus.Disconnected;

            Zone = zone;
        }

        public static void CreateInstance(string sIpAddress, int portNumber, int zone, CommandHandler commandHandler)
        {
            _outgoingConnection = new OutgoingConnection(sIpAddress, portNumber, zone, commandHandler);
        }

        public static OutgoingConnection GetInstance()
        {
            return _outgoingConnection;
        }

        private void _bwConnect_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ConnectionState = (ConnectionStatus) e.Result;
            if (ConnectionState == ConnectionStatus.Connected)
            {
                SendMessage($"{Command.ZONE}:{Zone}");
            }
        }

        private void _bwConnect_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = Connect();
        }

        public void MakeConnection()
        {
            if (ConnectionState == ConnectionStatus.Connected) return;
            _bwConnect.RunWorkerAsync();
        }

        private ConnectionStatus Connect()
        {
            try
            {
                _client.Connect(_ipAddress, _portNumber);
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                return ConnectionStatus.UnableToConnect;
            }
            _messageReceiver = new MessageReceiver(_client);
            _messageReceiver.ClientDisconnected += _messageReceiver_ClientDisconnected;
            _commandHandler.AddEntry(_messageReceiver);
            return ConnectionStatus.Connected;
        }

        private void _messageReceiver_ClientDisconnected(object sender, ConnectionLostEventArgs e)
        {
            ConnectionState = ConnectionStatus.ConnectionLost;
            _messageReceiver = null;
            _commandHandler = null;
        }

        public void Disconnect()
        {
            _messageReceiver.ClientDisconnected -= _messageReceiver_ClientDisconnected;
            ConnectionState = ConnectionStatus.Disconnected;
            _messageReceiver = null;
            _commandHandler = null;
            _client.Close();
            _outgoingConnection = null;
        }

        public void SendMessage(string message)
        {
            _messageReceiver?.SendMessage(message);
        }

        protected virtual void OnConnectionUpdate(ConnectionUpdateEventArgs e)
        {
            ConnectionUpdate?.Invoke(this, e);
        }

        public void AskForSync()
        {
            SendMessage($"{Command.SYNCDB}:{DatabaseWrapper.GetLatestTimestamp()}");
        }
    }
}