using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace TCPlistener
{
    public partial class Form1 : Form
    {
        private SocketHelper helper = new SocketHelper();
        private BackgroundWorker bw = new BackgroundWorker();
        public Form1()
        {
            InitializeComponent();

            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            //bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }


        static string output = "";
        private void Start_Click(object sender, EventArgs e)
        {
            MessageRecieve.Start();    
        }

        private void startwork()
        {
            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
                connect.Enabled = false;
            }
            

        }
        private void connecting()
        {
            // Create an instance of the TcpListener class.
            TcpListener tcpListener = null;
            IPAddress ipAddress = IPAddress.Loopback;
            try
            {
                // Set the listener on the local IP address 
                // and specify the port.
                tcpListener = new TcpListener(ipAddress, 13);
                tcpListener.Start();
                //output = "Waiting for a connection...";
                // Create a TCP socket. 
                // If you ran this server on the desktop, you could use 
                // Socket socket = tcpListener.AcceptSocket() 
                // for greater flexibility.
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                // Read the data stream from the client. 
                byte[] bytes = new byte[256];
                NetworkStream stream = tcpClient.GetStream();
                stream.Read(bytes, 0, bytes.Length);
                helper.processMsg(tcpClient, stream, bytes);
                tcpClient.Close();
            }
            catch (Exception ex)
            {
                output = "Error: " + ex.ToString();
                MessageBox.Show(output);
            }
            finally
            {
                if (tcpListener != null)
                    tcpListener.Stop();
            }

        }

        private void Stop_Click(object sender, EventArgs e)
        {
            connect.Enabled = true;
            if (bw.WorkerSupportsCancellation == true)
            {
                bw.CancelAsync();
            }
            MessageRecieve.Stop();
        }
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            connecting();
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            connect.Enabled = true;
            if ((e.Cancelled == true))
            {
                tbProgress.Text = "Canceled!";
            }

            else if (!(e.Error == null))
            {
                tbProgress.Text = ("Error: " + e.Error.Message);
            }

            else
            {
                tbProgress.Text = "Done!";
            }
            listBox1.Items.Add(helper.LastMessage);
        }
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            tbProgress.Text = (e.ProgressPercentage.ToString() + "%");
        }

        private void MessageRecieve_Tick(object sender, EventArgs e)
        {
            startwork();
        }
    }
}
