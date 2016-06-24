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
                case Command.SYNC:
                    
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
