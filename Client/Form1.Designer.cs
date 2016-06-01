namespace Client
{
    partial class Form1
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
            this.btnAddToDatabase = new System.Windows.Forms.Button();
            this.nudRFIDSpeed = new System.Windows.Forms.NumericUpDown();
            this.txtNumberRFID = new System.Windows.Forms.Label();
            this.txtSpeedRFID = new System.Windows.Forms.Label();
            this.btnGetListOfRFID = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.lbInfo = new System.Windows.Forms.ListBox();
            this.tbRFIDNumber = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblCheckSerialString = new System.Windows.Forms.Label();
            this.tbServerIp = new System.Windows.Forms.TextBox();
            this.txtServerip = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.nudRFIDSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAddToDatabase
            // 
            this.btnAddToDatabase.Enabled = false;
            this.btnAddToDatabase.Location = new System.Drawing.Point(214, 88);
            this.btnAddToDatabase.Name = "btnAddToDatabase";
            this.btnAddToDatabase.Size = new System.Drawing.Size(148, 23);
            this.btnAddToDatabase.TabIndex = 5;
            this.btnAddToDatabase.Text = "Add RFID to database";
            this.btnAddToDatabase.UseVisualStyleBackColor = true;
            this.btnAddToDatabase.Click += new System.EventHandler(this.btnAddToDatabase_Click);
            // 
            // nudRFIDSpeed
            // 
            this.nudRFIDSpeed.Location = new System.Drawing.Point(88, 96);
            this.nudRFIDSpeed.Name = "nudRFIDSpeed";
            this.nudRFIDSpeed.Size = new System.Drawing.Size(120, 20);
            this.nudRFIDSpeed.TabIndex = 4;
            // 
            // txtNumberRFID
            // 
            this.txtNumberRFID.AutoSize = true;
            this.txtNumberRFID.Location = new System.Drawing.Point(12, 72);
            this.txtNumberRFID.Name = "txtNumberRFID";
            this.txtNumberRFID.Size = new System.Drawing.Size(96, 13);
            this.txtNumberRFID.TabIndex = 6;
            this.txtNumberRFID.Text = "serialNumber RFID";
            // 
            // txtSpeedRFID
            // 
            this.txtSpeedRFID.AutoSize = true;
            this.txtSpeedRFID.Location = new System.Drawing.Point(12, 98);
            this.txtSpeedRFID.Name = "txtSpeedRFID";
            this.txtSpeedRFID.Size = new System.Drawing.Size(66, 13);
            this.txtSpeedRFID.TabIndex = 7;
            this.txtSpeedRFID.Text = "Speed RFID";
            // 
            // btnGetListOfRFID
            // 
            this.btnGetListOfRFID.Location = new System.Drawing.Point(166, 136);
            this.btnGetListOfRFID.Name = "btnGetListOfRFID";
            this.btnGetListOfRFID.Size = new System.Drawing.Size(148, 23);
            this.btnGetListOfRFID.TabIndex = 7;
            this.btnGetListOfRFID.Text = "Lijst uit database ophalen";
            this.btnGetListOfRFID.UseVisualStyleBackColor = true;
            this.btnGetListOfRFID.Click += new System.EventHandler(this.btnGetListOfRFID_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(22, 138);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(113, 21);
            this.comboBox1.TabIndex = 6;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(110, 389);
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
            this.lbInfo.Location = new System.Drawing.Point(22, 174);
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(250, 212);
            this.lbInfo.TabIndex = 10;
            this.lbInfo.TabStop = false;
            // 
            // tbRFIDNumber
            // 
            this.tbRFIDNumber.Location = new System.Drawing.Point(108, 69);
            this.tbRFIDNumber.MaxLength = 8;
            this.tbRFIDNumber.Name = "tbRFIDNumber";
            this.tbRFIDNumber.Size = new System.Drawing.Size(100, 20);
            this.tbRFIDNumber.TabIndex = 3;
            this.tbRFIDNumber.TextChanged += new System.EventHandler(this.tbRFIDNumber_TextChanged);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(12, 32);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblCheckSerialString
            // 
            this.lblCheckSerialString.AutoSize = true;
            this.lblCheckSerialString.Location = new System.Drawing.Point(215, 69);
            this.lblCheckSerialString.Name = "lblCheckSerialString";
            this.lblCheckSerialString.Size = new System.Drawing.Size(73, 13);
            this.lblCheckSerialString.TabIndex = 13;
            this.lblCheckSerialString.Text = "Input checker";
            // 
            // tbServerIp
            // 
            this.tbServerIp.Location = new System.Drawing.Point(159, 34);
            this.tbServerIp.Name = "tbServerIp";
            this.tbServerIp.Size = new System.Drawing.Size(100, 20);
            this.tbServerIp.TabIndex = 1;
            this.tbServerIp.Text = "localhost";
            // 
            // txtServerip
            // 
            this.txtServerip.AutoSize = true;
            this.txtServerip.Location = new System.Drawing.Point(104, 37);
            this.txtServerip.Name = "txtServerip";
            this.txtServerip.Size = new System.Drawing.Size(52, 13);
            this.txtServerip.TabIndex = 15;
            this.txtServerip.Text = "Server ip:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(842, 539);
            this.Controls.Add(this.txtServerip);
            this.Controls.Add(this.tbServerIp);
            this.Controls.Add(this.lblCheckSerialString);
            this.Controls.Add(this.tbRFIDNumber);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.lbInfo);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.btnGetListOfRFID);
            this.Controls.Add(this.txtSpeedRFID);
            this.Controls.Add(this.txtNumberRFID);
            this.Controls.Add(this.nudRFIDSpeed);
            this.Controls.Add(this.btnAddToDatabase);
            this.Controls.Add(this.btnConnect);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.nudRFIDSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnAddToDatabase;
        private System.Windows.Forms.NumericUpDown nudRFIDSpeed;
        private System.Windows.Forms.Label txtNumberRFID;
        private System.Windows.Forms.Label txtSpeedRFID;
        private System.Windows.Forms.Button btnGetListOfRFID;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ListBox lbInfo;
        private System.Windows.Forms.TextBox tbRFIDNumber;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblCheckSerialString;
        private System.Windows.Forms.TextBox tbServerIp;
        private System.Windows.Forms.Label txtServerip;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}

