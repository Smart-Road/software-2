﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace Server
{
    public class GetSpeedArgs
    {
        public readonly long Parameter;
        public readonly MessageReceiver MsgReceiver;
        public GetSpeedArgs(MessageReceiver msgReceiver, long parameter)
        {
            MsgReceiver = msgReceiver;
            Parameter = parameter;
        }
    }
    public class CommandHandler
    {
        private readonly List<MessageReceiver> _messageReceivers = new List<MessageReceiver>();
        private const int FieldsPerEntry = 4;
        private readonly BackgroundWorker _bwGetSpeed = new BackgroundWorker();

        public event CommandReceivedDelegate CommandReceived;
        public delegate void CommandReceivedDelegate(object sender, CommandReceivedEventArgs e);
        public event CommandHandlerCallbackDelegate CommandHandlerCallback;
        public delegate void CommandHandlerCallbackDelegate(object sender, CommandHandledEventArgs e);
        public event MessageReceiver.ClientDisconnectedDelegate ClientDisconnected;
        public void AddEntry(MessageReceiver msgReceiver)
        {
            _messageReceivers.Add(msgReceiver);
            msgReceiver.MessageReceived += MsgReceiver_MessageReceived;
            msgReceiver.ClientDisconnected += MsgReceiver_ClientDisconnected;
            msgReceiver.ListenForCommands();
        }

        public CommandHandler()
        {
            _bwGetSpeed.DoWork += _bwGetSpeed_DoWork;
        }

        private void _bwGetSpeed_DoWork(object sender, DoWorkEventArgs e)
        {
            var getSpeedArgs = e.Argument as GetSpeedArgs;
            if (getSpeedArgs == null) return;
            var speed = DatabaseWrapper.GetSpeedFromDb(getSpeedArgs.Parameter);
            if (speed < 0)
            {
                OnCommandHandlerCallback(new CommandHandledEventArgs(true, "Speed is not in database, syncing"));
                OutgoingConnection.GetInstance()?.AskForSync();
                Thread.Sleep(2000);
                // try to get it again
                speed = DatabaseWrapper.GetSpeedFromDb(getSpeedArgs.Parameter);
            }
            if (speed < 0)
            {
                getSpeedArgs.MsgReceiver.SendMessage($"{Command.ERROR}:{Command.SPEED_NOT_FOUND}");
                return;
            }
            getSpeedArgs.MsgReceiver.SendMessage($"{Command.MAXSPEED}:{speed}");
            OnCommandHandlerCallback(new CommandHandledEventArgs(true, $"Maxspeed sent to client:{speed}"));
        }

        private void MsgReceiver_ClientDisconnected(object sender, ConnectionLostEventArgs e)
        {
            var messageReceiver = sender as MessageReceiver;
            _messageReceivers.Remove(messageReceiver);
            OnClientDisconnected(e);
        }

        private void MsgReceiver_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string sCommand;
            string sParameter;

            if (!ParseMessage(e.Message, out sCommand, out sParameter))
            {
                OnCommandHandlerCallback(new CommandHandledEventArgs(false, $"Could not parse command: ({e.Message})"));
                return;
            }
            Command command;
            if (!Enum.TryParse(sCommand, out command))
            {
                OnCommandHandlerCallback(new CommandHandledEventArgs(false, $"Invalid command received: ({sCommand}"));
                return;
            }
            OnCommandReceived(new CommandReceivedEventArgs(command, sParameter));

            var messageReceiver = sender as MessageReceiver;
            if (messageReceiver == null) return;

            OnCommandReceived(new CommandReceivedEventArgs(command, sParameter));
            switch (command)
            {
                case Command.ERROR:
                    OnCommandHandlerCallback(new CommandHandledEventArgs(true, sParameter));
                    break;
                case Command.GETSPEED:
                    {
                        long serialNumber;
                        if (!long.TryParse(sParameter, out serialNumber))
                        {
                            OnCommandHandlerCallback(new CommandHandledEventArgs(false, $"Invalid parameter at getspeed received:{sParameter}"));
                            return;
                        }
                        _bwGetSpeed.RunWorkerAsync(new GetSpeedArgs(messageReceiver, serialNumber));
                    }
                    break;
                case Command.SYNC:
                    {
                        if (OutgoingConnection.GetInstance()?.ConnectionState != ConnectionStatus.Connected)
                        {
                            OnCommandHandlerCallback(new CommandHandledEventArgs(false,
                                "No connection with master-server"));
                            return;
                        }
                        if (OutgoingConnection.GetInstance().RemoteEndPoint.ToString() != messageReceiver.RemoteEndPoint)
                        {
                            OnCommandHandlerCallback(new CommandHandledEventArgs(false, "Not a message from the server"));
                            return;
                        }
                        var databaseEntries = sParameter.Split(';');
                        var entriesToAdd = new List<DatabaseEntry>();
                        foreach (var databaseEntry in databaseEntries)
                        {
                            var fields = databaseEntry.Split(',');
                            if (fields.Length != FieldsPerEntry) continue;
                            var sSerialNumber = fields[0];
                            var sSpeed = fields[1];
                            var sZone = fields[2];
                            var sTimeStamp = fields[3];
                            long serialNumber, timestamp;
                            int speed, zone;
                            if (!long.TryParse(sSerialNumber, out serialNumber))
                            {
                                OnCommandHandlerCallback(new CommandHandledEventArgs(false, "Serialnumber is not valid"));
                                return;
                            }

                            if (!int.TryParse(sSpeed, out speed))
                            {
                                OnCommandHandlerCallback(new CommandHandledEventArgs(false, "Speed is not valid"));
                                return;
                            }

                            if (!int.TryParse(sZone, out zone) || zone != OutgoingConnection.GetInstance()?.Zone)
                            {
                                OnCommandHandlerCallback(new CommandHandledEventArgs(false, "Zone is not valid"));
                                return;
                            }
                            if (!long.TryParse(sTimeStamp, out timestamp))
                            {
                                OnCommandHandlerCallback(new CommandHandledEventArgs(false, "Timestamp is not valid"));
                                return;
                            }
                            var entry = new DatabaseEntry(serialNumber, speed, zone, timestamp);
                            entriesToAdd.Add(entry);
                        }
                        var entriesAdded = DatabaseWrapper.AddOrUpdateEntries(entriesToAdd);

                        if (entriesAdded > 0)
                        {
                            OnCommandHandlerCallback(new CommandHandledEventArgs(true,
                                $"Added {entriesAdded} entries to the Database"));
                        }
                        else
                        {
                            OnCommandHandlerCallback(new CommandHandledEventArgs(false,
                                "No entries were added to the database"));
                        }
                    }
                    break;
            }
        }

        private static bool ParseMessage(string msg, out string command, out string parameter)
        {
            if (!msg.Contains(':'))
            {
                command = null;
                parameter = null;
                return false;
            }
            var msgs = msg.Split(':');

            if (msgs.Length != 2)
            {
                command = null;
                parameter = null;
                Console.WriteLine($"Invalid amount of parameters received:{msgs.Length}");
                return false;
            }
            command = msgs[0];
            parameter = msgs[1];
            return true;
        }

        protected void OnCommandReceived(CommandReceivedEventArgs e)
        {
            CommandReceived?.Invoke(this, e);
        }

        protected virtual void OnCommandHandlerCallback(CommandHandledEventArgs e)
        {
            CommandHandlerCallback?.Invoke(this, e);
        }

        protected virtual void OnClientDisconnected(ConnectionLostEventArgs e)
        {
            ClientDisconnected?.Invoke(this, e);
        }
    }
}
