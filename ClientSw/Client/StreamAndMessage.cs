using System.Net.Sockets;

namespace Server
{
    class StreamAndMessage
    {

        public NetworkStream Stream
        {
            get;
        }
        public string Message
        {
            get;
        }
        public StreamAndMessage(NetworkStream stream, string message)
        {
            Stream = stream;
            Message = message;
        }
    }
}
