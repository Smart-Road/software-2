using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Master_server
{

    public class CommandHandler
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
                MainGui.Main.AddInfoToLb($"Could not parse command: ({e.Message})");
                return;
            }
            Command command;
            if (!Enum.TryParse(sCommand, out command))
            {
                MainGui.Main.AddInfoToLb($"Invalid command received: ({sCommand})");
                return;
            }
            var messageReceiver = sender as MessageReceiver;
            if (messageReceiver == null) return;
            switch (command)
            {
                case Command.SYNCDB:
                    if (messageReceiver.Zone == 0)
                    {
                        Console.WriteLine("Zone is not set");
                        messageReceiver.SendMessage($"{Command.ERROR}:{Command.NO_ZONE_SET}");
                        return;
                    }
                    break;
                case Command.ADDRFID:
                    if (messageReceiver.Zone == 0)
                    {
                        Console.WriteLine("Zone is not set");
                        messageReceiver.SendMessage($"{Command.ERROR}:{Command.NO_ZONE_SET}");
                        return;
                    }
                    
                    // parse data
                    string[] parameters = sParameter.Split(',');
                    if (parameters.Length != 2)
                    {
                        messageReceiver.SendMessage($"{Command.ERROR},{Command.INVALID_AMOUNT_OF_PARAMS}");
                        return;
                    }
                    long serialNumber;
                    int maxSpeed;
                    if (!long.TryParse(parameters[0], out serialNumber) || !int.TryParse(parameters[1], out maxSpeed))
                    {
                        MainGui.Main.AddInfoToLb("Could not parse serialNumber or maxSpeed");
                        return;
                    }
                    Rfid rfid;
                    if (!Rfid.GetRfid(serialNumber, maxSpeed, out rfid))
                    {
                        MainGui.Main.AddInfoToLb("Invalid rfid received");
                        return;
                    }

                    // add to database
                    if (!DatabaseWrapper.AddRfid(rfid, messageReceiver.Zone))
                    {
                        MainGui.Main.AddInfoToLb("Could not add rfid to database");
                        return;
                    }
                    MainGui.Main.AddInfoToLb($"Rfid added to database: ({rfid})");
                    break;
                case Command.ZONE:
                    int zone;
                    if (!int.TryParse(sParameter, out zone))
                    {
                        MainGui.Main.AddInfoToLb($"Unparseable string with zone received: ({sParameter})");
                        return;
                    }
                    messageReceiver.Zone = zone;
                    MainGui.Main.AddInfoToLb($"Zone set to ({messageReceiver.Zone})");
                    break;
                default:
                    MainGui.Main.AddInfoToLb($"Invalid command received:{command}");
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
    }
}
