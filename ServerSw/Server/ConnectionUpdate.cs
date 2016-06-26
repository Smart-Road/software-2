using System;
using System.Net.Sockets;

namespace Server
{
    public class ConnectionUpdateEventArgs : EventArgs
    {
        public bool ConnectionState { get; }
        public TcpClient Client { get; }
        public ConnectionUpdateEventArgs(bool connectionState, TcpClient client)
        {
            ConnectionState = connectionState;
            Client = client;
        }
    }
}
