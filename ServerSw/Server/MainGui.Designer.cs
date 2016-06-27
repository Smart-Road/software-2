namespace Server
{
    partial class MainGui
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
            this.btnAddToDatabase = new System.Windows.Forms.Button();
            this.nudRFIDSpeed = new System.Windows.Forms.NumericUpDown();
            this.txtNumberRFID = new System.Windows.Forms.Label();
            this.txtSpeedRFID = new System.Windows.Forms.Label();
            this.btnGetListOfRFID = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.lbInfo = new System.Windows.Forms.ListBox();
            this.tbRFIDNumber = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblCheckSerialString = new System.Windows.Forms.Label();
            this.tbServerIp = new System.Windows.Forms.TextBox();
            this.txtServerip = new System.Windows.Forms.Label();
            this.nudZoneId = new System.Windows.Forms.NumericUpDown();
            this.txtZoneId = new System.Windows.Forms.Label();
            this.nudPortnumber = new System.Windows.Forms.NumericUpDown();
            this.txtPortnumber = new System.Windows.Forms.Label();
            this.gbMasterServerConnection = new System.Windows.Forms.GroupBox();
            this.gbAddRfid = new System.Windows.Forms.GroupBox();
            this.gbUpdateRfid = new System.Windows.Forms.GroupBox();
            this.btnUpdateRfid = new System.Windows.Forms.Button();
            this.btnEmptyDatabase = new System.Windows.Forms.Button();
            this.syncTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.nudRFIDSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudZoneId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPortnumber)).BeginInit();
            this.gbMasterServerConnection.SuspendLayout();
            this.gbAddRfid.SuspendLayout();
            this.gbUpdateRfid.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddToDatabase
            // 
            this.btnAddToDatabase.Enabled = false;
            this.btnAddToDatabase.Location = new System.Drawing.Point(91, 127);
            this.btnAddToDatabase.Name = "btnAddToDatabase";
            this.btnAddToDatabase.Size = new System.Drawing.Size(106, 23);
            this.btnAddToDatabase.TabIndex = 5;
            this.btnAddToDatabase.Text = "Add rfid";
            this.btnAddToDatabase.UseVisualStyleBackColor = true;
            this.btnAddToDatabase.Click += new System.EventHandler(this.btnAddToDatabase_Click);
            // 
            // nudRFIDSpeed
            // 
            this.nudRFIDSpeed.Location = new System.Drawing.Point(91, 45);
            this.nudRFIDSpeed.Name = "nudRFIDSpeed";
            this.nudRFIDSpeed.Size = new System.Drawing.Size(106, 20);
            this.nudRFIDSpeed.TabIndex = 4;
            // 
            // txtNumberRFID
            // 
            this.txtNumberRFID.AutoSize = true;
            this.txtNumberRFID.Location = new System.Drawing.Point(6, 22);
            this.txtNumberRFID.Name = "txtNumberRFID";
            this.txtNumberRFID.Size = new System.Drawing.Size(71, 13);
            this.txtNumberRFID.TabIndex = 6;
            this.txtNumberRFID.Text = "Serial number";
            // 
            // txtSpeedRFID
            // 
            this.txtSpeedRFID.AutoSize = true;
            this.txtSpeedRFID.Location = new System.Drawing.Point(6, 47);
            this.txtSpeedRFID.Name = "txtSpeedRFID";
            this.txtSpeedRFID.Size = new System.Drawing.Size(38, 13);
            this.txtSpeedRFID.TabIndex = 7;
            this.txtSpeedRFID.Text = "Speed";
            // 
            // btnGetListOfRFID
            // 
            this.btnGetListOfRFID.Enabled = false;
            this.btnGetListOfRFID.Location = new System.Drawing.Point(86, 127);
            this.btnGetListOfRFID.Name = "btnGetListOfRFID";
            this.btnGetListOfRFID.Size = new System.Drawing.Size(100, 23);
            this.btnGetListOfRFID.TabIndex = 7;
            this.btnGetListOfRFID.Text = "Sync with server";
            this.btnGetListOfRFID.UseVisualStyleBackColor = true;
            this.btnGetListOfRFID.Click += new System.EventHandler(this.btnGetListOfRFID_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(12, 413);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 8;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // lbInfo
            // 
            this.lbInfo.FormattingEnabled = true;
            this.lbInfo.Location = new System.Drawing.Point(12, 188);
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(624, 212);
            this.lbInfo.TabIndex = 10;
            this.lbInfo.TabStop = false;
            // 
            // tbRFIDNumber
            // 
            this.tbRFIDNumber.Location = new System.Drawing.Point(91, 19);
            this.tbRFIDNumber.MaxLength = 50;
            this.tbRFIDNumber.Name = "tbRFIDNumber";
            this.tbRFIDNumber.Size = new System.Drawing.Size(106, 20);
            this.tbRFIDNumber.TabIndex = 3;
            this.tbRFIDNumber.TextChanged += new System.EventHandler(this.tbRFIDNumber_TextChanged);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(86, 98);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(100, 23);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblCheckSerialString
            // 
            this.lblCheckSerialString.AutoSize = true;
            this.lblCheckSerialString.BackColor = System.Drawing.Color.White;
            this.lblCheckSerialString.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCheckSerialString.ForeColor = System.Drawing.Color.Red;
            this.lblCheckSerialString.Location = new System.Drawing.Point(179, 22);
            this.lblCheckSerialString.Name = "lblCheckSerialString";
            this.lblCheckSerialString.Size = new System.Drawing.Size(15, 13);
            this.lblCheckSerialString.TabIndex = 13;
            this.lblCheckSerialString.Text = "X";
            // 
            // tbServerIp
            // 
            this.tbServerIp.Location = new System.Drawing.Point(86, 72);
            this.tbServerIp.Name = "tbServerIp";
            this.tbServerIp.Size = new System.Drawing.Size(100, 20);
            this.tbServerIp.TabIndex = 1;
            this.tbServerIp.Text = "127.0.0.1";
            // 
            // txtServerip
            // 
            this.txtServerip.AutoSize = true;
            this.txtServerip.Location = new System.Drawing.Point(7, 75);
            this.txtServerip.Name = "txtServerip";
            this.txtServerip.Size = new System.Drawing.Size(49, 13);
            this.txtServerip.TabIndex = 15;
            this.txtServerip.Text = "Server ip";
            // 
            // nudZoneId
            // 
            this.nudZoneId.Location = new System.Drawing.Point(86, 46);
            this.nudZoneId.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudZoneId.Name = "nudZoneId";
            this.nudZoneId.Size = new System.Drawing.Size(100, 20);
            this.nudZoneId.TabIndex = 16;
            this.nudZoneId.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // txtZoneId
            // 
            this.txtZoneId.AutoSize = true;
            this.txtZoneId.Location = new System.Drawing.Point(6, 48);
            this.txtZoneId.Name = "txtZoneId";
            this.txtZoneId.Size = new System.Drawing.Size(43, 13);
            this.txtZoneId.TabIndex = 17;
            this.txtZoneId.Text = "Zone id";
            // 
            // nudPortnumber
            // 
            this.nudPortnumber.Location = new System.Drawing.Point(86, 20);
            this.nudPortnumber.Name = "nudPortnumber";
            this.nudPortnumber.Size = new System.Drawing.Size(100, 20);
            this.nudPortnumber.TabIndex = 18;
            this.nudPortnumber.Value = new decimal(new int[] {
            14,
            0,
            0,
            0});
            // 
            // txtPortnumber
            // 
            this.txtPortnumber.AutoSize = true;
            this.txtPortnumber.Location = new System.Drawing.Point(6, 22);
            this.txtPortnumber.Name = "txtPortnumber";
            this.txtPortnumber.Size = new System.Drawing.Size(64, 13);
            this.txtPortnumber.TabIndex = 19;
            this.txtPortnumber.Text = "Port number";
            // 
            // gbMasterServerConnection
            // 
            this.gbMasterServerConnection.Controls.Add(this.txtPortnumber);
            this.gbMasterServerConnection.Controls.Add(this.txtServerip);
            this.gbMasterServerConnection.Controls.Add(this.tbServerIp);
            this.gbMasterServerConnection.Controls.Add(this.txtZoneId);
            this.gbMasterServerConnection.Controls.Add(this.btnGetListOfRFID);
            this.gbMasterServerConnection.Controls.Add(this.nudPortnumber);
            this.gbMasterServerConnection.Controls.Add(this.nudZoneId);
            this.gbMasterServerConnection.Controls.Add(this.btnConnect);
            this.gbMasterServerConnection.Location = new System.Drawing.Point(12, 12);
            this.gbMasterServerConnection.Name = "gbMasterServerConnection";
            this.gbMasterServerConnection.Size = new System.Drawing.Size(200, 158);
            this.gbMasterServerConnection.TabIndex = 20;
            this.gbMasterServerConnection.TabStop = false;
            this.gbMasterServerConnection.Text = "Master-server connection";
            // 
            // gbAddRfid
            // 
            this.gbAddRfid.Controls.Add(this.btnAddToDatabase);
            this.gbAddRfid.Controls.Add(this.txtNumberRFID);
            this.gbAddRfid.Controls.Add(this.txtSpeedRFID);
            this.gbAddRfid.Controls.Add(this.lblCheckSerialString);
            this.gbAddRfid.Controls.Add(this.tbRFIDNumber);
            this.gbAddRfid.Controls.Add(this.nudRFIDSpeed);
            this.gbAddRfid.Location = new System.Drawing.Point(428, 12);
            this.gbAddRfid.Name = "gbAddRfid";
            this.gbAddRfid.Size = new System.Drawing.Size(200, 158);
            this.gbAddRfid.TabIndex = 21;
            this.gbAddRfid.TabStop = false;
            this.gbAddRfid.Text = "Add rfid";
            // 
            // gbUpdateRfid
            // 
            this.gbUpdateRfid.Controls.Add(this.btnUpdateRfid);
            this.gbUpdateRfid.Location = new System.Drawing.Point(220, 12);
            this.gbUpdateRfid.Name = "gbUpdateRfid";
            this.gbUpdateRfid.Size = new System.Drawing.Size(200, 158);
            this.gbUpdateRfid.TabIndex = 22;
            this.gbUpdateRfid.TabStop = false;
            this.gbUpdateRfid.Text = "Update rfid";
            // 
            // btnUpdateRfid
            // 
            this.btnUpdateRfid.Enabled = false;
            this.btnUpdateRfid.Location = new System.Drawing.Point(6, 19);
            this.btnUpdateRfid.Name = "btnUpdateRfid";
            this.btnUpdateRfid.Size = new System.Drawing.Size(188, 131);
            this.btnUpdateRfid.TabIndex = 0;
            this.btnUpdateRfid.Text = "Click here";
            this.btnUpdateRfid.UseVisualStyleBackColor = true;
            this.btnUpdateRfid.Click += new System.EventHandler(this.btnUpdateRfid_Click);
            // 
            // btnEmptyDatabase
            // 
            this.btnEmptyDatabase.Location = new System.Drawing.Point(98, 413);
            this.btnEmptyDatabase.Name = "btnEmptyDatabase";
            this.btnEmptyDatabase.Size = new System.Drawing.Size(188, 23);
            this.btnEmptyDatabase.TabIndex = 23;
            this.btnEmptyDatabase.Text = "Remove all entries from database";
            this.btnEmptyDatabase.UseVisualStyleBackColor = true;
            this.btnEmptyDatabase.Click += new System.EventHandler(this.btnEmptyDatabase_Click);
            // 
            // syncTimer
            // 
            this.syncTimer.Interval = 10000;
            this.syncTimer.Tick += new System.EventHandler(this.syncTimer_Tick);
            // 
            // MainGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 448);
            this.Controls.Add(this.btnEmptyDatabase);
            this.Controls.Add(this.gbUpdateRfid);
            this.Controls.Add(this.gbAddRfid);
            this.Controls.Add(this.gbMasterServerConnection);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.lbInfo);
            this.Name = "MainGui";
            this.Text = "Server";
            ((System.ComponentModel.ISupportInitialize)(this.nudRFIDSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudZoneId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPortnumber)).EndInit();
            this.gbMasterServerConnection.ResumeLayout(false);
            this.gbMasterServerConnection.PerformLayout();
            this.gbAddRfid.ResumeLayout(false);
            this.gbAddRfid.PerformLayout();
            this.gbUpdateRfid.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnAddToDatabase;
        private System.Windows.Forms.NumericUpDown nudRFIDSpeed;
        private System.Windows.Forms.Label txtNumberRFID;
        private System.Windows.Forms.Label txtSpeedRFID;
        private System.Windows.Forms.Button btnGetListOfRFID;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ListBox lbInfo;
        private System.Windows.Forms.TextBox tbRFIDNumber;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblCheckSerialString;
        private System.Windows.Forms.TextBox tbServerIp;
        private System.Windows.Forms.Label txtServerip;
        private System.Windows.Forms.NumericUpDown nudZoneId;
        private System.Windows.Forms.Label txtZoneId;
        private System.Windows.Forms.NumericUpDown nudPortnumber;
        private System.Windows.Forms.Label txtPortnumber;
        private System.Windows.Forms.GroupBox gbMasterServerConnection;
        private System.Windows.Forms.GroupBox gbAddRfid;
        private System.Windows.Forms.GroupBox gbUpdateRfid;
        private System.Windows.Forms.Button btnUpdateRfid;
        private System.Windows.Forms.Button btnEmptyDatabase;
        private System.Windows.Forms.Timer syncTimer;
    }
}

