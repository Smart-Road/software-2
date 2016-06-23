using System;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Master_server
{
    public partial class MainGui : Form
    {
        private readonly ConnectionAccepter _accepter;
        public static MainGui Main { get; private set; }

        private const int PortNumber = 13;
        public MainGui()
        {
            InitializeComponent();
            _accepter = new ConnectionAccepter(PortNumber, this);
            _accepter.StartAccepting();
            Main = this;
            Database.PrepareConnection();
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
