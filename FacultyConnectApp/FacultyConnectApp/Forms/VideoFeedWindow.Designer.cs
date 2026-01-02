namespace FacultyConnectApp.Forms

{
    partial class VideoFeedWindow
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
            this.panelTopBar = new System.Windows.Forms.Panel();
            this.btnEndCall = new System.Windows.Forms.Button();
            this.btnSwitchToAudio = new System.Windows.Forms.Button();
            this.videoPictureBox = new System.Windows.Forms.PictureBox();
            this.panelTopBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.videoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTopBar
            // 
            this.panelTopBar.BackColor = System.Drawing.Color.CornflowerBlue;
            this.panelTopBar.Controls.Add(this.btnEndCall);
            this.panelTopBar.Controls.Add(this.btnSwitchToAudio);
            this.panelTopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopBar.Location = new System.Drawing.Point(0, 0);
            this.panelTopBar.Name = "panelTopBar";
            this.panelTopBar.Size = new System.Drawing.Size(784, 40);
            this.panelTopBar.TabIndex = 0;
            this.panelTopBar.Paint += new System.Windows.Forms.PaintEventHandler(this.panelTopBar_Paint);
            // 
            // btnEndCall
            // 
            this.btnEndCall.ForeColor = System.Drawing.Color.Black;
            this.btnEndCall.Location = new System.Drawing.Point(670, 8);
            this.btnEndCall.Name = "btnEndCall";
            this.btnEndCall.Size = new System.Drawing.Size(85, 25);
            this.btnEndCall.TabIndex = 1;
            this.btnEndCall.Text = "End Call";
            this.btnEndCall.UseVisualStyleBackColor = true;
            this.btnEndCall.Click += new System.EventHandler(this.btnEndCall_Click);
            // 
            // btnSwitchToAudio
            // 
            this.btnSwitchToAudio.ForeColor = System.Drawing.Color.Black;
            this.btnSwitchToAudio.Location = new System.Drawing.Point(560, 8);
            this.btnSwitchToAudio.Name = "btnSwitchToAudio";
            this.btnSwitchToAudio.Size = new System.Drawing.Size(100, 25);
            this.btnSwitchToAudio.TabIndex = 2;
            this.btnSwitchToAudio.Text = "Switch to Audio";
            this.btnSwitchToAudio.UseVisualStyleBackColor = true;
            this.btnSwitchToAudio.Click += new System.EventHandler(this.btnSwitchToAudio_Click);
            // 
            // videoPictureBox
            // 
            this.videoPictureBox.BackColor = System.Drawing.Color.Black;
            this.videoPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.videoPictureBox.Location = new System.Drawing.Point(0, 40);
            this.videoPictureBox.Name = "videoPictureBox";
            this.videoPictureBox.Size = new System.Drawing.Size(784, 421);
            this.videoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.videoPictureBox.TabIndex = 3;
            this.videoPictureBox.TabStop = false;
            this.videoPictureBox.Click += new System.EventHandler(this.videoPictureBox_Click);
            // 
            // VideoFeedWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.videoPictureBox);
            this.Controls.Add(this.panelTopBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "VideoFeedWindow";
            this.Text = "Faculty Connect – Live Feed";
            this.Load += new System.EventHandler(this.VideoFeedWindow_Load);
            this.panelTopBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.videoPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnSwitchToAudio;
        private System.Windows.Forms.Button btnEndCall;
        private System.Windows.Forms.Panel panelTopBar;
        private System.Windows.Forms.PictureBox videoPictureBox;
       
    }
}
// 