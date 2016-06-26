using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class CommandHandler
    {
        private readonly List<MessageReceiver> _messageReceivers = new List<MessageReceiver>();
        private const int FieldsPerEntry = 4;

        public event CommandReceivedDelegate CommandReceived;
        public delegate void CommandReceivedDelegate(object sender, CommandReceivedEventArgs e);
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
            _messageReceivers.Remove(messageReceiver);
        }

        private void MsgReceiver_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string sCommand;
            string sParameter;

            if (!ParseMessage(e.Message, out sCommand, out sParameter))
            {
                Console.WriteLine($"Could not parse command: ({e.Message})");
                return;
            }
            Command command;
            if (!Enum.TryParse(sCommand, out command))
            {
                Console.WriteLine($"Invalid command received: ({sCommand})");
                return;
            }
            OnCommandReceived(new CommandReceivedEventArgs(command, sParameter));

            var messageReceiver = sender as MessageReceiver;
            if (messageReceiver == null) return;

            Console.WriteLine($"Command:{command} received with parameter:{sParameter}");
            switch (command)
            {
                case Command.GETSPEED:
                    {
                        long serialNumber;
                        if (!long.TryParse(sParameter, out serialNumber))
                        {
                            Console.WriteLine("Invalid parameter at getspeed received");
                            return;
                        }
                        var speed = DatabaseWrapper.GetSpeedFromDb(serialNumber);
                        if (speed < 0)
                        {
                            Console.WriteLine("Speed is not in database");
                            OutgoingConnection.GetInstance()?.SendMessage(
                                $"{Command.SYNCDB}:{DatabaseWrapper.GetLatestTimestamp()}");
                        }
                        else
                        {
                            messageReceiver.SendMessage($"{Command.MAXSPEED}:{speed}");
                        }
                    }
                    break;
                case Command.SYNC:
                    {
                        if (OutgoingConnection.GetInstance()?.Connected == false)
                        {
                            Console.WriteLine("No connection with master-server");
                            return;
                        }
                        if (OutgoingConnection.GetInstance().RemoteEndPoint != messageReceiver.RemoteEndPoint)
                        {
                            Console.WriteLine("Not a message from the server");
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
                                Console.WriteLine("Serialnumber is not valid");
                                return;
                            }

                            if (!int.TryParse(sSpeed, out speed))
                            {
                                Console.WriteLine("Speed is not valid");
                                return;
                            }

                            if (!int.TryParse(sZone, out zone) || zone != OutgoingConnection.GetInstance()?.Zone)
                            {
                                Console.WriteLine("Zone is not valid");
                                return;
                            }
                            if (!long.TryParse(sTimeStamp, out timestamp))
                            {
                                Console.WriteLine("Timestamp is not valid");
                                return;
                            }
                            var entry = new DatabaseEntry(serialNumber, speed, zone, timestamp);
                            entriesToAdd.Add(entry);
                        }
                        var entriesAdded = DatabaseWrapper.AddOrUpdateEntries(entriesToAdd);

                        if (entriesAdded < 0)
                        {
                            entriesAdded = 0;
                        }
                        Console.WriteLine($"Added {entriesAdded} entries to the Database");
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
    }

    

    public class CommandReceivedEventArgs : EventArgs
    {
        public readonly Command Command;
        public readonly string Parameter;
        public CommandReceivedEventArgs(Command command, string parameter)
        {
            Command = command;
            Parameter = parameter;
        }
    }
}
