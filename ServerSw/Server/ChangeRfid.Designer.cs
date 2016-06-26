namespace Server
{
    partial class ChangeRfid
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
            this.lbRfids = new System.Windows.Forms.ListBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.txtSerialnumber = new System.Windows.Forms.Label();
            this.gbChangeRfid = new System.Windows.Forms.GroupBox();
            this.lblSerialnumber = new System.Windows.Forms.Label();
            this.txtNewSpeed = new System.Windows.Forms.Label();
            this.nudNewSpeed = new System.Windows.Forms.NumericUpDown();
            this.txtCurSpeed = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lblCurSpeed = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.gbChangeRfid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNewSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbRfids
            // 
            this.lbRfids.FormattingEnabled = true;
            this.lbRfids.Location = new System.Drawing.Point(12, 12);
            this.lbRfids.Name = "lbRfids";
            this.lbRfids.Size = new System.Drawing.Size(345, 446);
            this.lbRfids.TabIndex = 0;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(378, 12);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(83, 23);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Load all data";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // txtSerialnumber
            // 
            this.txtSerialnumber.AutoSize = true;
            this.txtSerialnumber.Location = new System.Drawing.Point(5, 21);
            this.txtSerialnumber.Name = "txtSerialnumber";
            this.txtSerialnumber.Size = new System.Drawing.Size(71, 13);
            this.txtSerialnumber.TabIndex = 2;
            this.txtSerialnumber.Text = "Serial number";
            // 
            // gbChangeRfid
            // 
            this.gbChangeRfid.Controls.Add(this.btnUpdate);
            this.gbChangeRfid.Controls.Add(this.splitContainer1);
            this.gbChangeRfid.Location = new System.Drawing.Point(363, 56);
            this.gbChangeRfid.Name = "gbChangeRfid";
            this.gbChangeRfid.Size = new System.Drawing.Size(223, 183);
            this.gbChangeRfid.TabIndex = 3;
            this.gbChangeRfid.TabStop = false;
            this.gbChangeRfid.Text = "Change selected rfid";
            // 
            // lblSerialnumber
            // 
            this.lblSerialnumber.AutoSize = true;
            this.lblSerialnumber.Location = new System.Drawing.Point(12, 21);
            this.lblSerialnumber.Name = "lblSerialnumber";
            this.lblSerialnumber.Size = new System.Drawing.Size(67, 13);
            this.lblSerialnumber.TabIndex = 3;
            this.lblSerialnumber.Text = "select an rfid";
            // 
            // txtNewSpeed
            // 
            this.txtNewSpeed.AutoSize = true;
            this.txtNewSpeed.Location = new System.Drawing.Point(5, 80);
            this.txtNewSpeed.Name = "txtNewSpeed";
            this.txtNewSpeed.Size = new System.Drawing.Size(61, 13);
            this.txtNewSpeed.TabIndex = 4;
            this.txtNewSpeed.Text = "New speed";
            // 
            // nudNewSpeed
            // 
            this.nudNewSpeed.Location = new System.Drawing.Point(15, 78);
            this.nudNewSpeed.Name = "nudNewSpeed";
            this.nudNewSpeed.Size = new System.Drawing.Size(87, 20);
            this.nudNewSpeed.TabIndex = 5;
            // 
            // txtCurSpeed
            // 
            this.txtCurSpeed.AutoSize = true;
            this.txtCurSpeed.Location = new System.Drawing.Point(5, 51);
            this.txtCurSpeed.Name = "txtCurSpeed";
            this.txtCurSpeed.Size = new System.Drawing.Size(73, 13);
            this.txtCurSpeed.TabIndex = 6;
            this.txtCurSpeed.Text = "Current speed";
            // 
            // splitContainer1
            // 
            this.splitContainer1.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(6, 28);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtCurSpeed);
            this.splitContainer1.Panel1.Controls.Add(this.txtSerialnumber);
            this.splitContainer1.Panel1.Controls.Add(this.txtNewSpeed);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lblCurSpeed);
            this.splitContainer1.Panel2.Controls.Add(this.nudNewSpeed);
            this.splitContainer1.Panel2.Controls.Add(this.lblSerialnumber);
            this.splitContainer1.Size = new System.Drawing.Size(208, 120);
            this.splitContainer1.SplitterDistance = 87;
            this.splitContainer1.TabIndex = 4;
            // 
            // lblCurSpeed
            // 
            this.lblCurSpeed.AutoSize = true;
            this.lblCurSpeed.Location = new System.Drawing.Point(12, 51);
            this.lblCurSpeed.Name = "lblCurSpeed";
            this.lblCurSpeed.Size = new System.Drawing.Size(31, 13);
            this.lblCurSpeed.TabIndex = 6;
            this.lblCurSpeed.Text = "none";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(6, 154);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(208, 23);
            this.btnUpdate.TabIndex = 5;
            this.btnUpdate.Text = "Update selected rfid";
            this.btnUpdate.UseVisualStyleBackColor = true;
            // 
            // ChangeRfid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 472);
            this.Controls.Add(this.gbChangeRfid);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.lbRfids);
            this.Name = "ChangeRfid";
            this.Text = "ChangeRfid";
            this.gbChangeRfid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudNewSpeed)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbRfids;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label txtSerialnumber;
        private System.Windows.Forms.GroupBox gbChangeRfid;
        private System.Windows.Forms.Label txtCurSpeed;
        private System.Windows.Forms.NumericUpDown nudNewSpeed;
        private System.Windows.Forms.Label txtNewSpeed;
        private System.Windows.Forms.Label lblSerialnumber;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label lblCurSpeed;
        private System.Windows.Forms.Button btnUpdate;
    }
}