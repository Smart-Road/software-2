using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class ConnectionAccepter
    {
        private readonly CommandHandler _commandHandler = new CommandHandler();
        private volatile bool _accepting;
        private readonly BackgroundWorker _bwConnectionAccepter = new BackgroundWorker();
        private readonly List<TcpClient> _clients = new List<TcpClient>();

        public event ClientAcceptedDelegate ClientAccepted;
        public delegate void ClientAcceptedDelegate(object sender, ConnectionUpdateEventArgs e);

        private readonly TcpListener _tcpListener;

        public ConnectionAccepter(int portNumber)
        {
            _bwConnectionAccepter.WorkerReportsProgress = true;
            _bwConnectionAccepter.DoWork += _bwConnectionAccepter_AcceptClients;
            _tcpListener = new TcpListener(IPAddress.Any, portNumber);
            _accepting = false;
        }

        private void _bwConnectionAccepter_AcceptClients(object sender, DoWorkEventArgs e)
        {
            while (_accepting)
            {
                if (!_tcpListener.Pending())
                {
                    Thread.Sleep(20);
                    continue;
                }

                var client = _tcpListener.AcceptTcpClient();
                _clients.Add(client);

                Console.WriteLine($"Client connected:{client.Client.RemoteEndPoint}");
                OnClientAccepted(new ConnectionUpdateEventArgs(true, client));

                var messageReceiver = new MessageReceiver(client);
                _commandHandler.AddEntry(messageReceiver);
            }
        }

        protected virtual void OnClientAccepted(ConnectionUpdateEventArgs e)
        {
            ClientAccepted?.Invoke(this, e);
        }

        public void StartAccepting()
        {
            _tcpListener.Start();
            _accepting = true;
            _bwConnectionAccepter.RunWorkerAsync();
        }
    }
}

