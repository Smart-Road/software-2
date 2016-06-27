using System;

namespace Server
{
    public class CommandHandledEventArgs : EventArgs
    {
        public readonly string Message;
        public readonly bool Valid;
        public CommandHandledEventArgs(bool valid, string message)
        {
            Valid = valid;
            Message = message;
        }
    }

}
