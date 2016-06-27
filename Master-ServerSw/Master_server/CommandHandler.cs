using System;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
    public class CommandHandler
    {
        private readonly List<MessageReceiver> _messageReceivers = new List<MessageReceiver>();

        public event CommandHandlerCallbackDelegate CommandHandlerCallback;
        public delegate void CommandHandlerCallbackDelegate(object sender, CommandHandledEventArgs e);

        public event CommandReceivedDelegate CommandReceived;
        public delegate void CommandReceivedDelegate(object sender, CommandReceivedEventArgs e);

        public event MessageReceiver.ClientDisconnectedDelegate ClientDisconnected;
        private const string SyncDelimiter = ";";

        public void AddEntry(MessageReceiver msgReceiver)
        {
            _messageReceivers.Add(msgReceiver);
            msgReceiver.MessageReceived += MsgReceiver_MessageReceived;
            msgReceiver.ClientDisconnected += MsgReceiver_ClientDisconnected;
            msgReceiver.ListenForCommands();
        }

        private void MsgReceiver_ClientDisconnected(object sender, ConnectionLostEventArgs e)
        {
            var messageReceiver = sender as MessageReceiver;
            if (messageReceiver != null)
            {
                messageReceiver.ClientDisconnected -= MsgReceiver_ClientDisconnected;
                messageReceiver.MessageReceived -= MsgReceiver_MessageReceived;
                messageReceiver.Disconnect();
                _messageReceivers.Remove(messageReceiver);
            }
            OnClientDisconnected(e);
        }

        private void MsgReceiver_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string sCommand;
            string sParameter;
            if (!ParseMessage(e.Message, out sCommand, out sParameter))
            {
                OnCommandHandled(new CommandHandledEventArgs(false, $"Could not parse command: ({e.Message})"));
                return;
            }
            Command command;
            if (!Enum.TryParse(sCommand, out command))
            {
                OnCommandHandled(new CommandHandledEventArgs(false, $"Invalid command received: ({sCommand})"));
                return;
            }
            OnCommandReceived(new CommandReceivedEventArgs(command, sParameter));
            var messageReceiver = sender as MessageReceiver;
            if (messageReceiver == null) return;
            switch (command)
            {
                case Command.SYNCDB:
                    var syncsucces = SyncDb(messageReceiver, sParameter);
                    OnCommandHandled(new CommandHandledEventArgs(syncsucces, "Syncing db " + (syncsucces ? "succeeded" : "failed")));
                    break;
                case Command.ADDRFID:
                case Command.CHANGERFID:
                    if (messageReceiver.Zone == 0)
                    {
                        OnCommandHandled(new CommandHandledEventArgs(false, "Zone is not set"));
                        messageReceiver.SendMessage($"{Command.ERROR}:{Command.NO_ZONE_SET}");
                        return;
                    }

                    // parse data
                    string[] parameters = sParameter.Split(',');
                    if (parameters.Length != 2)
                    {
                        OnCommandHandled(new CommandHandledEventArgs(false, "Invalid amount of parameters"));
                        messageReceiver.SendMessage($"{Command.ERROR}:{Command.INVALID_AMOUNT_OF_PARAMS}");
                        return;
                    }
                    long serialNumber;
                    int maxSpeed;
                    if (!long.TryParse(parameters[0], out serialNumber) || !int.TryParse(parameters[1], out maxSpeed))
                    {
                        OnCommandHandled(new CommandHandledEventArgs(false, "Could not parse serialNumber or maxSpeed"));
                        return;
                    }
                    Rfid rfid;
                    if (!Rfid.GetRfid(serialNumber, maxSpeed, out rfid))
                    {
                        OnCommandHandled(new CommandHandledEventArgs(false, "Invalid rfid received"));
                        return;
                    }

                    switch (command)
                    {
                        case Command.ADDRFID:
                            // add to database
                            if (!DatabaseWrapper.InsertData(rfid, messageReceiver.Zone))
                            {
                                messageReceiver.SendMessage($"{Command.ERROR}:{Command.ALREADY_IN_DB}");
                                OnCommandHandled(new CommandHandledEventArgs(false, "Could not add rfid to database"));
                                return;
                            }

                            OnCommandHandled(new CommandHandledEventArgs(true, $"Rfid added to database: ({rfid})"));
                            break;
                        case Command.CHANGERFID:
                            if (!DatabaseWrapper.UpdateEntry(new DatabaseEntry(rfid.SerialNumber, 
                                rfid.Speed,
                                messageReceiver.Zone, 
                                Database.ConvertToTimestamp(DateTime.UtcNow))))
                            {
                                messageReceiver.SendMessage($"{Command.ERROR}:{Command.UPDATE_FAILED}");
                                return;
                            }
                            messageReceiver.SendMessage($"{Command.CHANGERFID}:{Command.OK}");
                            OnCommandHandled(new CommandHandledEventArgs(true, $"Rfid updated: ({rfid})"));
                            break;
                    }
                    break;
                case Command.ZONE:
                    int zone;
                    if (!int.TryParse(sParameter, out zone))
                    {
                        OnCommandHandled(new CommandHandledEventArgs(false, $"Unparseable string with zone received: ({sParameter})"));
                        return;
                    }
                    messageReceiver.Zone = zone;
                    OnCommandHandled(new CommandHandledEventArgs(true, $"Zone set to ({messageReceiver.Zone})"));
                    break;
                case Command.ERROR:
                    OnCommandHandled(new CommandHandledEventArgs(true, $"ERROR:{sParameter}"));
                    break;
                default:
                    OnCommandHandled(new CommandHandledEventArgs(false, $"Invalid command received:{command}"));
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
                return false;
            }
            command = msgs[0];
            parameter = msgs[1];
            return true;
        }

        private bool SyncDb(MessageReceiver messageReceiver, string sParameter)
        {
            if (messageReceiver.Zone == 0)
            {
                OnCommandHandled(new CommandHandledEventArgs(false, "Zone is not set"));
                messageReceiver.SendMessage($"{Command.ERROR}:{Command.NO_ZONE_SET}");
                return false;
            }
            long timestamp;
            if (!long.TryParse(sParameter, out timestamp))
            {
                OnCommandHandled(new CommandHandledEventArgs(false, $"Could not parse timestamp {sParameter}"));
                return false;
            }
            var entriesAfterTimestamp = DatabaseWrapper.LoadZoneAfterTimeStamp(messageReceiver.Zone, timestamp);
            string syncmessage = $"{Command.SYNC}:";
            var length = entriesAfterTimestamp.Count;
            var counter = 0;
            foreach (var databaseEntry in entriesAfterTimestamp)
            {
                syncmessage += databaseEntry.ToString();
                if (counter != length - 1)
                {
                    syncmessage += SyncDelimiter;
                }
                counter++;
            }
            OnCommandHandled(new CommandHandledEventArgs(true, $"{counter} entries synced"));
            messageReceiver.SendMessage(syncmessage);
            return true;
        }

        protected virtual void OnCommandHandled(CommandHandledEventArgs e)
        {
            CommandHandlerCallback?.Invoke(this, e);
        }

        protected virtual void OnCommandReceived(CommandReceivedEventArgs e)
        {
            CommandReceived?.Invoke(this, e);
        }

        protected virtual void OnClientDisconnected(ConnectionLostEventArgs e)
        {
            ClientDisconnected?.Invoke(this, e);
        }
    }
}
