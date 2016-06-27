using System;
using System.Net.Sockets;

namespace Server
{
    public class ConnectionUpdateEventArgs : EventArgs
    {
        public ConnectionStatus ConnectionState { get; }
        public TcpClient Client { get; }
        public ConnectionUpdateEventArgs(ConnectionStatus connectionState, TcpClient client)
        {
            ConnectionState = connectionState;
            Client = client;
        }
    }
}
