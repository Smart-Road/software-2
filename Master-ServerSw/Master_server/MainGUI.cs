using System;
using Server;
using System.Windows.Forms;

namespace Server
{
    public partial class MainGui : Form
    {
        private readonly ConnectionAccepter _accepter;
        public static MainGui Main { get; private set; }

        private const int PortNumber = 14;
        public MainGui()
        {
            InitializeComponent();
            Database.PrepareDatabase();
            _accepter = new ConnectionAccepter(PortNumber);
            _accepter.StartAccepting();
            Main = this;
        }

        private void btnLoadAllFromDb_Click(object sender, EventArgs e)
        {
            var databaseEntries = DatabaseWrapper.LoadAllFromDatabase();
            var databaseEntries2 = DatabaseWrapper.LoadZoneFromDb(120);
        }

        public void AddInfoToLb(string info)
        {
            lbInfo.Invoke(new EventHandler(delegate
            {
                lbInfo.Items.Add(info);
            }));
        }
    }
}
