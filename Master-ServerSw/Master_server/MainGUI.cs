using System;
using Server;
using System.Windows.Forms;

namespace Server
{
    public partial class MainGui : Form
    {
        private readonly ConnectionAccepter _accepter;

        private const int PortNumber = 14;
        public MainGui()
        {
            InitializeComponent();
            Database.PrepareDatabase();
            _accepter = new ConnectionAccepter(PortNumber);
            _accepter.ClientAccepted += _accepter_ClientAccepted;
            _accepter.CommandHandlerCallback += _accepter_CommandHandlerCallback;
            _accepter.StartAccepting();
        }

        private void _accepter_CommandHandlerCallback(object sender, CommandHandledEventArgs e)
        {
            AddInfoToLb(e.Message);
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
