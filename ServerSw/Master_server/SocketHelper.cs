using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Master_server
{
    class SocketHelper
    {
        TcpClient mscClient;
        string mstrMessage;
        string mstrResponse;
        byte[] bytesSent;

        public String LastMessage { get; private set; }

        public void processMsg(TcpClient client, NetworkStream stream, byte[] bytesReceived)
        {
            // Handle the message received and  
            // send a response back to the client.
            mstrMessage = Encoding.ASCII.GetString(bytesReceived, 0, bytesReceived.Length);
            mscClient = client;
            mstrMessage = mstrMessage.Substring(0, 5);
            if (mstrMessage.Equals("%Hello$"))
            {
                mstrResponse = "#Goodbye$";
            }
            else
            {
                mstrResponse = "What?";
            }
            LastMessage = mstrMessage;
            bytesSent = Encoding.ASCII.GetBytes(mstrResponse);
            stream.Write(bytesSent, 0, bytesSent.Length);
        }

    }

}
