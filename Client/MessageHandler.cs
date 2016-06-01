using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace Client
{
    class MessageHandler
    {
        private readonly TcpClient client;
        private NetworkStream stream;
        private State state;
        private readonly char beginDelimiter;
        private readonly char endDelimiter;
        private string received;
        public bool Connected { get; private set; }
        public string LastCommand { get; private set; }

        private const int MaxCommandLength = 1024;

        private enum State
        {
            waiting,
            receiving
        }

        public MessageHandler(char beginDelimiter = '%', char endDelimiter = '$')
        {
            this.client = new TcpClient();
            this.state = State.waiting;
            this.beginDelimiter = beginDelimiter;
            this.endDelimiter = endDelimiter;
            this.received = string.Empty;
            this.LastCommand = string.Empty;
        }

        public async Task Connect(string ipAddress, int port)
        {
            await client.ConnectAsync(ipAddress, port);
            stream = client.GetStream();
            Connected = true;
        }

        public bool Update()
        {
            while (stream.DataAvailable)
            {
                byte[] bytes = new byte[1];
                stream.Read(bytes, 0, bytes.Length);
                char[] incoming = Encoding.ASCII.GetChars(bytes);
                switch (state)
                {
                    case State.waiting:
                        if (incoming[0] == beginDelimiter)
                        {
                            received = string.Empty;
                            state = State.receiving;
                        }
                        break;
                    case State.receiving:
                        if (incoming[0] != endDelimiter)
                        {
                            received += incoming[0];
                        }
                        else
                        {
                            LastCommand = received;
                            received = string.Empty;
                            state = State.waiting;
                            return true;
                        }
                        if (received.Length > MaxCommandLength)
                        {
                            throw new LengthException();
                        }
                        break;
                    default:
                        throw new ArgumentException(nameof(state));
                }
            }
            return false;
        }

        public void SendMessage(string message)
        {
            if (stream == null || Connected == false)
            {
                throw new InvalidOperationException("can not send message when not connected");
            }
            string messageWithDelimiters = $"{beginDelimiter}{message}{endDelimiter}";
            if (messageWithDelimiters.Length > MaxCommandLength)
            {
                throw new LengthException();
            }
            byte[] messageBytes = Encoding.ASCII.GetBytes(messageWithDelimiters);
            stream.Write(messageBytes, 0, messageBytes.Length);
        }

        public void Disconnect()
        {
            stream.Close();
            client.Close();
            Connected = false;
        }
    }

    public class LengthException : Exception
    {

        public LengthException()
        {
        }

        public LengthException(string message) : base(message)
        {
        }

        public LengthException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

