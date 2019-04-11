namespace Gui {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.mainTabs = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnStop = new System.Windows.Forms.Button();
            this.logTabs = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.infoLog = new System.Windows.Forms.RichTextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.errorLog = new System.Windows.Forms.RichTextBox();
            this.startButton = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lblSent = new System.Windows.Forms.Label();
            this.lblRecv = new System.Windows.Forms.Label();
            this.mainTabs.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.logTabs.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTabs
            // 
            this.mainTabs.Controls.Add(this.tabPage1);
            this.mainTabs.Controls.Add(this.tabPage2);
            this.mainTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabs.Location = new System.Drawing.Point(0, 0);
            this.mainTabs.Name = "mainTabs";
            this.mainTabs.SelectedIndex = 0;
            this.mainTabs.Size = new System.Drawing.Size(680, 272);
            this.mainTabs.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lblRecv);
            this.tabPage1.Controls.Add(this.lblSent);
            this.tabPage1.Controls.Add(this.btnStop);
            this.tabPage1.Controls.Add(this.logTabs);
            this.tabPage1.Controls.Add(this.startButton);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(672, 246);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Main";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(553, 51);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(113, 23);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // logTabs
            // 
            this.logTabs.Controls.Add(this.tabPage3);
            this.logTabs.Controls.Add(this.tabPage4);
            this.logTabs.Location = new System.Drawing.Point(0, 0);
            this.logTabs.Name = "logTabs";
            this.logTabs.SelectedIndex = 0;
            this.logTabs.Size = new System.Drawing.Size(547, 240);
            this.logTabs.TabIndex = 1;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.infoLog);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(539, 214);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Log";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // infoLog
            // 
            this.infoLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoLog.Location = new System.Drawing.Point(3, 3);
            this.infoLog.Name = "infoLog";
            this.infoLog.Size = new System.Drawing.Size(533, 208);
            this.infoLog.TabIndex = 2;
            this.infoLog.Text = "";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.errorLog);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(539, 214);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Errors";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // errorLog
            // 
            this.errorLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorLog.Location = new System.Drawing.Point(3, 3);
            this.errorLog.Name = "errorLog";
            this.errorLog.Size = new System.Drawing.Size(533, 208);
            this.errorLog.TabIndex = 3;
            this.errorLog.Text = "";
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(553, 22);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(113, 23);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(672, 246);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Settings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lblSent
            // 
            this.lblSent.AutoSize = true;
            this.lblSent.Location = new System.Drawing.Point(553, 100);
            this.lblSent.Name = "lblSent";
            this.lblSent.Size = new System.Drawing.Size(32, 13);
            this.lblSent.TabIndex = 1;
            this.lblSent.Text = "Sent:";
            // 
            // lblRecv
            // 
            this.lblRecv.AutoSize = true;
            this.lblRecv.Location = new System.Drawing.Point(553, 126);
            this.lblRecv.Name = "lblRecv";
            this.lblRecv.Size = new System.Drawing.Size(36, 13);
            this.lblRecv.TabIndex = 3;
            this.lblRecv.Text = "Recv:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 272);
            this.Controls.Add(this.mainTabs);
            this.Name = "Form1";
            this.Text = "ZBase Server";
            this.mainTabs.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.logTabs.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl mainTabs;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabControl logTabs;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.RichTextBox infoLog;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.RichTextBox errorLog;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblRecv;
        private System.Windows.Forms.Label lblSent;
    }
}

