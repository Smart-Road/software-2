using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class CommandHandler
    {
        private readonly List<MessageReceiver> _messageReceivers = new List<MessageReceiver>();

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

        private static void MsgReceiver_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string sCommand;
            string sParameter;

            if (!ParseMessage(e.Message, out sCommand, out sParameter))
            {
                MainGui.Main.AddToInfo($"Could not parse command: ({e.Message})");
                return;
            }
            Console.WriteLine(sParameter);
            Command command;
            if (!Enum.TryParse(sCommand, out command))
            {
                MainGui.Main.AddToInfo($"Invalid command received: ({sCommand})");
                return;
            }
            var messageReceiver = sender as MessageReceiver;
            if (messageReceiver == null) return;
            switch (command)
            {
                case Command.GETSPEED:
                    {
                        long serialNumber;
                        if (!long.TryParse(sParameter, out serialNumber))
                        {
                            MainGui.Main.AddToInfo("Invalid parameter at getspeed received");
                        }
                        int speed = DatabaseWrapper.GetSpeedFromDb(serialNumber);
                        if (speed < 0)
                        {
                            MainGui.Main.OutgoingConnection?.SendMessage($"{Command.SYNCDB}:{DatabaseWrapper.GetLatestTimestamp()}");
                        }
                        else
                        {
                            messageReceiver.SendMessage($"{Command.MAXSPEED}:{speed}");
                        }
                    }
                    break;
                case Command.SYNC:
                    {
                        string[] databaseEntries = sParameter.Split(';');
                        int entriesAdded = 0;
                        foreach (string databaseEntry in databaseEntries)
                        {
                            string[] fields = databaseEntry.Split(',');
                            if (fields.Length == 3)
                            {
                                string sSerialNumber = fields[0];
                                string sSpeed = fields[1];
                                string sTimeStamp = fields[2];
                                long serialNumber;
                                int speed;
                                long timestamp;
                                if (!long.TryParse(sSerialNumber, out serialNumber))
                                {
                                    MainGui.Main.AddToInfo("Serialnumber is not valid");
                                    return;
                                }

                                if (!int.TryParse(sSpeed, out speed))
                                {
                                    MainGui.Main.AddToInfo("Speed is not valid");
                                    return;
                                }

                                if (!long.TryParse(sTimeStamp, out timestamp))
                                {
                                    MainGui.Main.AddToInfo("Timestamp is not valid");
                                    return;
                                }
                                DatabaseEntry entry = new DatabaseEntry(serialNumber, speed, timestamp);
                                DatabaseWrapper.AddEntry(entry);
                                if (DatabaseWrapper.AddEntry(entry))
                                {
                                    entriesAdded += 1;
                                }
                            }
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
                MainGui.Main.AddToInfo($"Invalid amount of parameters received:{msgs.Length}");
                return false;
            }
            command = msgs[0];
            parameter = msgs[1];
            return true;
        }
    }
}
