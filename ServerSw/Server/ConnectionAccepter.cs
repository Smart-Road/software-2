using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class ConnectionAccepter
    {
        private readonly CommandHandler _commandHandler;
        private volatile bool _accepting;
        private readonly BackgroundWorker _bwConnectionAccepter = new BackgroundWorker();
        private readonly List<TcpClient> _clients = new List<TcpClient>();

        public event ClientAcceptedDelegate ClientAccepted;
        public delegate void ClientAcceptedDelegate(object sender, ConnectionUpdateEventArgs e);
        public event MessageReceiver.ClientDisconnectedDelegate ClientDisconnected;

        private readonly TcpListener _tcpListener;

        public ConnectionAccepter(int portNumber, CommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
            _bwConnectionAccepter.WorkerReportsProgress = true;
            _bwConnectionAccepter.DoWork += _bwConnectionAccepter_AcceptClients;
            _tcpListener = new TcpListener(IPAddress.Any, portNumber);
            _accepting = false;
            _commandHandler.ClientDisconnected += _commandHandler_ClientDisconnected;
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

                OnClientAccepted(new ConnectionUpdateEventArgs(ConnectionStatus.Connected, client));

                var messageReceiver = new MessageReceiver(client);
                _commandHandler.AddEntry(messageReceiver);
            }
        }

        private void _commandHandler_ClientDisconnected(object sender, ConnectionLostEventArgs e)
        {
            var toRemove = _clients.FirstOrDefault(tcpClient => !tcpClient.Connected);
            _clients.Remove(toRemove);
            OnClientDisconnected(e);

        }
        protected virtual void OnClientDisconnected(ConnectionLostEventArgs e)
        {
            ClientDisconnected?.Invoke(this, e);
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

