﻿using System;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class MessageReceiver
    {
        private const int MaxCommandLength = 1024;
        private string _received = string.Empty;
        private volatile bool _receiving = false;
        private string _lastCommand = string.Empty;
        private BackgroundWorker _bwMessageListener = new BackgroundWorker();

        private NetworkStream _stream;
        private TcpClient _client;
        private State _state = State.Waiting;
        private readonly char _beginDelimiter;
        private readonly char _endDelimiter;

        public delegate void MessageReceivedDelegate(object sender, MessageReceivedEventArgs e);

        public delegate void ClientDisconnectedDelegate(object sender, ConnectionLostEventArgs e);

        public event MessageReceivedDelegate MessageReceived;
        public event ClientDisconnectedDelegate ClientDisconnected;

        public int Zone { get; set; } = 0;
        public readonly string RemoteEndPoint;

        private enum State
        {
            Waiting,
            Receiving
        }

        public MessageReceiver(TcpClient client, char beginDelimiter = '%', char endDelimiter = '$')
        {
            _client = client;
            _beginDelimiter = beginDelimiter;
            _endDelimiter = endDelimiter;

            _stream = client.GetStream();
            _bwMessageListener.WorkerReportsProgress = true;

            _bwMessageListener.DoWork += ListenForCommandsBw;
            _bwMessageListener.ProgressChanged += bwMessageListener_ReportProgress;
            _bwMessageListener.RunWorkerCompleted += _bwMessageListener_RunWorkerCompleted;

            RemoteEndPoint = _client?.Client.RemoteEndPoint.ToString();
        }

        private void _bwMessageListener_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!(e.Result is ConnectionLostEventArgs)) return;
            var eventArgs = (ConnectionLostEventArgs) e.Result;
            OnConnectionLost(eventArgs);
        }

        private void bwMessageListener_ReportProgress(object sender, ProgressChangedEventArgs e)
        {
            var args = e.UserState as MessageReceivedEventArgs;
            if (args == null) return;
            var eventArgs = args;
            OnMessageReceived(eventArgs);
        }

        public void ListenForCommands()
        {
            _receiving = true;
            _bwMessageListener.RunWorkerAsync();
        }

        private void ListenForCommandsBw(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker) sender;
            while (_receiving)
            {
                try
                {
                    var byteIn = _stream.ReadByte();
                    if (byteIn < 0)
                    {
                        e.Result = new ConnectionLostEventArgs(RemoteEndPoint);
                        return;
                    }
                    byte[] bytes = {(byte) byteIn};
                    var incoming = Encoding.ASCII.GetChars(bytes);
                    switch (_state)
                    {
                        case State.Waiting:
                            if (incoming[0] == _beginDelimiter)
                            {
                                _received = string.Empty;
                                _state = State.Receiving;
                            }
                            break;
                        case State.Receiving:
                            if (incoming[0] != _endDelimiter)
                            {
                                _received += incoming[0];
                            }
                            else
                            {
                                _lastCommand = _received;
                                _received = string.Empty;
                                _state = State.Waiting;
                                worker.ReportProgress(0,
                                    new MessageReceivedEventArgs(_lastCommand, _stream, Zone));
                            }
                            if (_received.Length > MaxCommandLength)
                            {
                                throw new LengthException();
                            }
                            break;
                        default:
                            throw new ArgumentException(nameof(_state));
                    }
                }
                catch (IOException ex)
                {
                    _receiving = false;
                    Console.WriteLine(ex);
                    e.Result = new ConnectionLostEventArgs(RemoteEndPoint);
                    return;
                }
                catch (ObjectDisposedException ex)
                {
                    _receiving = false;
                    Console.WriteLine(ex);
                    e.Result = new ConnectionLostEventArgs(RemoteEndPoint);
                    return;
                }
                catch (NotSupportedException ex)
                {
                    _receiving = false;
                    Console.WriteLine(ex);
                    e.Result = new ConnectionLostEventArgs(RemoteEndPoint);
                    return;
                }
            }
        }

        public void SendMessage(string message)
        {
            if (!_client.Connected)
            {
                Console.WriteLine("can not send message when not connected");
                return;
            }
            string messageWithDelimiters = $"{_beginDelimiter}{message}{_endDelimiter}";
            if (messageWithDelimiters.Length > MaxCommandLength)
            {
                throw new LengthException();
            }
            var messageBytes = Encoding.ASCII.GetBytes(messageWithDelimiters);
            _stream.Write(messageBytes, 0, messageBytes.Length);
        }

        public void Disconnect()
        {
            _stream.Close();
            _client.Close();
            _receiving = false;
            _client = null;
            _stream = null;
            _bwMessageListener = null;
            ClientDisconnected = null;
            MessageReceived = null;
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }

        protected virtual void OnConnectionLost(ConnectionLostEventArgs e)
        {
            ClientDisconnected?.Invoke(this, e);
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

    public class MessageReceivedEventArgs : EventArgs
    {
        public string Message { get; set; }
        public NetworkStream NetworkStream { get; set; }
        public int Zone { get; set; }

        public MessageReceivedEventArgs(string message, NetworkStream stream, int zone)
        {
            Message = message;
            NetworkStream = stream;
            Zone = zone;
        }
    }

    public class ConnectionLostEventArgs : EventArgs
    {
        public readonly string RemoteEndPoint;

        public ConnectionLostEventArgs(string remoteEndPoint)
        {
            RemoteEndPoint = remoteEndPoint;
        }
    }
}
