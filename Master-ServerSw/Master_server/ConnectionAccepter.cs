using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Master_server
{
    public class ConnectionAccepter
    {
        private readonly CommandHandler _commandHandler = new CommandHandler();
        private volatile bool _accepting = false;
        private readonly BackgroundWorker _bwConnectionAccepter = new BackgroundWorker();
        private readonly List<TcpClient> _clients = new List<TcpClient>();

        private readonly TcpListener _tcpListener;

        public ConnectionAccepter(int portNumber, MainGui main)
        {
            _bwConnectionAccepter.WorkerReportsProgress = true;
            _bwConnectionAccepter.DoWork += _bwConnectionAccepter_AcceptClients;
            _tcpListener = new TcpListener(IPAddress.Any, portNumber);
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

                MainGui.Main.AddInfoToLb($"Client connected:{client.Client.RemoteEndPoint}");

                var messageReceiver = new MessageReceiver(client);
                _commandHandler.AddEntry(messageReceiver);
            }
        }

        public void StartAccepting()
        {
            _tcpListener.Start();
            _accepting = true;
            _bwConnectionAccepter.RunWorkerAsync();
        }
    }
}
