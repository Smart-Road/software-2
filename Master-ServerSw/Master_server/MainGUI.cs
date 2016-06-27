using System;
using Server;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace Server
{
    public partial class MainGui : Form
    {
        private readonly ConnectionAccepter _accepter;
        private readonly CommandHandler _commandHandler = new CommandHandler();

        private const int PortNumber = 14;
        public MainGui()
        {
            InitializeComponent();
            Database.PrepareDatabase();
            _accepter = new ConnectionAccepter(PortNumber, _commandHandler);
            _accepter.ClientAccepted += _accepter_ClientAccepted;
            _commandHandler.CommandHandlerCallback += _commandHandler_CommandHandlerCallback;
            _commandHandler.CommandReceived += _commandHandler_CommandReceived;
            _accepter.ClientDisconnected += _accepter_ClientDisconnected;
            _accepter.StartAccepting();
        }

        private void _commandHandler_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            AddInfoToLb($"Command received:{e.Command}:{e.Parameter}");
        }

        private void _commandHandler_CommandHandlerCallback(object sender, CommandHandledEventArgs e)
        {
            AddInfoToLb($"Command handled:{e.Message}");
        }

        private void _accepter_ClientDisconnected(object sender, ConnectionLostEventArgs e)
        {
            AddInfoToLb($"Connection lost with {e.RemoteEndPoint}");
        }

        private void _accepter_ClientAccepted(object sender, ConnectionUpdateEventArgs e)
        {
            AddInfoToLb($"({e.Client.Client.RemoteEndPoint}) has connected");
        }

        public void AddInfoToLb(string info)
        {
            lbInfo.Invoke(new EventHandler(delegate
            {
                lbInfo.Items.Insert(0, info);
            }));
        }
    }
}
