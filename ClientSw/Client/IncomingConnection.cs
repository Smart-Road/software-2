using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Client
{
    public class IncomingConnection
    {
        private BackgroundWorker bwConnectionAccepter;
        private TcpListener listener;
        private string data;
        private volatile bool acceptingClients;
        private State state;
        private readonly char beginDelimiter;
        private readonly char endDelimiter;
        private const int MaxCommandLength = 1024;

        private List<TcpClient> clients;
        private List<NetworkStream> streams;

        private enum State { waiting, receiving }
        public IncomingConnection(int portnumber)
        {
            bwConnectionAccepter = new BackgroundWorker();
            bwConnectionAccepter.DoWork += BwConnectionAccepter_DoWork;
            bwConnectionAccepter.ProgressChanged += BwConnectionAccepter_ProgressChanged;
            bwConnectionAccepter.WorkerReportsProgress = true;
            state = State.waiting;
            acceptingClients = false;
            data = "";
            listener = new TcpListener(IPAddress.Any, portnumber);

            clients = new List<TcpClient>();
            streams = new List<NetworkStream>();

            beginDelimiter = '%';
            endDelimiter = '$';
        }

        public void StartListening()
        {
            if (!acceptingClients)
            {
                acceptingClients = true;
                listener.Start();
                bwConnectionAccepter.RunWorkerAsync();
            }
        }

        private void BwConnectionAccepter_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            StreamAndMessage output = e.UserState as StreamAndMessage;
            if (output != null)
            {
                Console.WriteLine(output.Message);
            }
        }

        private void BwConnectionAccepter_DoWork(object sender, DoWorkEventArgs e)
        {
            while (acceptingClients)
            {
                CheckClients();
                ListenForMessage(sender);             
            }
        }
        private void CheckClients()
        {
            if (listener.Pending())
            {
                TcpClient client = listener.AcceptTcpClient();
                clients.Add(client);
                streams.Add(client.GetStream());
            }
        }

        private void ListenForMessage(object sender)
        {
            foreach (NetworkStream stream in streams)
            {
                if (stream != null)
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
                                    data = string.Empty;
                                    state = State.receiving;
                                }
                                break;
                            case State.receiving:
                                if (incoming[0] != endDelimiter)
                                {
                                    data += incoming[0];
                                }
                                else
                                {
                                    string lastCommand = data;
                                    data = string.Empty;
                                    state = State.waiting;
                                    BackgroundWorker worker = (BackgroundWorker)sender;
                                    worker.ReportProgress(0, new StreamAndMessage(stream, lastCommand));
                                }
                                if (data.Length > MaxCommandLength)
                                {
                                    throw new LengthException();
                                }
                                break;
                            default:
                                throw new ArgumentException(nameof(state));
                        }
                    }
                    Thread.Sleep(1);
                }
            }
        }
    }
}

