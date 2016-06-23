namespace Master_server
{
    partial class MainGUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.connect = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.Stop = new System.Windows.Forms.Button();
            this.tbProgress = new System.Windows.Forms.TextBox();
            this.MessageRecieve = new System.Windows.Forms.Timer(this.components);
            this.btnLoadAllFromDb = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // connect
            // 
            this.connect.Location = new System.Drawing.Point(21, 47);
            this.connect.Name = "connect";
            this.connect.Size = new System.Drawing.Size(75, 23);
            this.connect.TabIndex = 0;
            this.connect.Text = "Start";
            this.connect.UseVisualStyleBackColor = true;
            this.connect.Click += new System.EventHandler(this.Start_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(10, 120);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(262, 134);
            this.listBox1.TabIndex = 5;
            // 
            // Stop
            // 
            this.Stop.Location = new System.Drawing.Point(140, 47);
            this.Stop.Name = "Stop";
            this.Stop.Size = new System.Drawing.Size(75, 23);
            this.Stop.TabIndex = 6;
            this.Stop.Text = "Stop";
            this.Stop.UseVisualStyleBackColor = true;
            this.Stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // tbProgress
            // 
            this.tbProgress.Location = new System.Drawing.Point(61, 85);
            this.tbProgress.Name = "tbProgress";
            this.tbProgress.Size = new System.Drawing.Size(100, 20);
            this.tbProgress.TabIndex = 7;
            // 
            // MessageRecieve
            // 
            this.MessageRecieve.Interval = 200;
            this.MessageRecieve.Tick += new System.EventHandler(this.MessageRecieve_Tick);
            // 
            // btnLoadAllFromDb
            // 
            this.btnLoadAllFromDb.AutoSize = true;
            this.btnLoadAllFromDb.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLoadAllFromDb.Location = new System.Drawing.Point(179, 83);
            this.btnLoadAllFromDb.Name = "btnLoadAllFromDb";
            this.btnLoadAllFromDb.Size = new System.Drawing.Size(79, 23);
            this.btnLoadAllFromDb.TabIndex = 8;
            this.btnLoadAllFromDb.Text = "Load from db";
            this.btnLoadAllFromDb.UseVisualStyleBackColor = true;
            this.btnLoadAllFromDb.Click += new System.EventHandler(this.btnLoadAllFromDb_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.btnLoadAllFromDb);
            this.Controls.Add(this.tbProgress);
            this.Controls.Add(this.Stop);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.connect);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connect;
        private System.Windows.Forms.ListBox listBox1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button Stop;
        private System.Windows.Forms.TextBox tbProgress;
        private System.Windows.Forms.Timer MessageRecieve;
        private System.Windows.Forms.Button btnLoadAllFromDb;
    }
}

