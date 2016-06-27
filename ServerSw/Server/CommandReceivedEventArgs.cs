using System;

namespace Server
{
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
