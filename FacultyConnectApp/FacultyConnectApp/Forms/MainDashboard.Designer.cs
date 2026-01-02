using Google.Type;
using System.Windows.Forms;
using static Google.Rpc.Context.AttributeContext.Types;

namespace FacultyConnectApp.Forms
{
    partial class MainDashboard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainDashboard));
            this.btnGrantAccess = new System.Windows.Forms.Button();
            this.btnAudioCall = new System.Windows.Forms.Button();
            this.btnVideoCall = new System.Windows.Forms.Button();
            this.panelSidebar = new System.Windows.Forms.Panel();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.lstLecturers = new System.Windows.Forms.ListBox();
            this.lblFacultyConnect = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panelVisitorMessage = new System.Windows.Forms.Panel();
            this.lblVisitorMessage = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.lblPurpose = new System.Windows.Forms.Label();
            this.lblStudentNumber = new System.Windows.Forms.Label();
            this.lblVisitorName = new System.Windows.Forms.Label();
            this.lblTimestamp = new System.Windows.Forms.Label();
            this.panelMessaging = new System.Windows.Forms.Panel();
            this.lblMessagingTitle = new System.Windows.Forms.Label();
            this.txtRecentMessage = new System.Windows.Forms.TextBox();
            this.bottomPanel = new System.Windows.Forms.TableLayoutPanel();
            this.txtMessageInput = new System.Windows.Forms.TextBox();
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.btnMessageHistory = new System.Windows.Forms.Button();
            this.btnDirectTest = new System.Windows.Forms.Button();
            this.btnTestRequest = new System.Windows.Forms.Button();
            this.btnRestartListener = new System.Windows.Forms.Button();
            this.btnTestFirebase = new System.Windows.Forms.Button();
            this.panelSidebar.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel3.SuspendLayout();
            this.panelVisitorMessage.SuspendLayout();
            this.panelMessaging.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGrantAccess
            // 
            this.btnGrantAccess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGrantAccess.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGrantAccess.Location = new System.Drawing.Point(240, 678);
            this.btnGrantAccess.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnGrantAccess.Name = "btnGrantAccess";
            this.btnGrantAccess.Size = new System.Drawing.Size(888, 51);
            this.btnGrantAccess.TabIndex = 0;
            this.btnGrantAccess.Text = "Grant Access\r\n";
            this.btnGrantAccess.UseVisualStyleBackColor = true;
            this.btnGrantAccess.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnAudioCall
            // 
            this.btnAudioCall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAudioCall.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAudioCall.Location = new System.Drawing.Point(18, 507);
            this.btnAudioCall.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnAudioCall.Name = "btnAudioCall";
            this.btnAudioCall.Size = new System.Drawing.Size(201, 44);
            this.btnAudioCall.TabIndex = 2;
            this.btnAudioCall.Text = "Audio Call";
            this.btnAudioCall.UseVisualStyleBackColor = true;
            this.btnAudioCall.Click += new System.EventHandler(this.btnAudioCall_Click);
            // 
            // btnVideoCall
            // 
            this.btnVideoCall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVideoCall.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVideoCall.Location = new System.Drawing.Point(667, 507);
            this.btnVideoCall.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnVideoCall.Name = "btnVideoCall";
            this.btnVideoCall.Size = new System.Drawing.Size(201, 44);
            this.btnVideoCall.TabIndex = 4;
            this.btnVideoCall.Text = "View Visitor";
            this.btnVideoCall.UseVisualStyleBackColor = true;
            this.btnVideoCall.Click += new System.EventHandler(this.btnVideoCall_Click);
            // 
            // panelSidebar
            // 
            this.panelSidebar.BackColor = System.Drawing.Color.CornflowerBlue;
            this.panelSidebar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSidebar.Controls.Add(this.lblWelcome);
            this.panelSidebar.Controls.Add(this.lstLecturers);
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSidebar.Location = new System.Drawing.Point(0, 0);
            this.panelSidebar.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(223, 741);
            this.panelSidebar.TabIndex = 0;
            this.panelSidebar.Paint += new System.Windows.Forms.PaintEventHandler(this.panelSidebar_Paint_1);
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWelcome.Location = new System.Drawing.Point(16, 96);
            this.lblWelcome.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(146, 50);
            this.lblWelcome.TabIndex = 1;
            this.lblWelcome.Text = "Welcome back,\n Prof. Walingo";
            this.lblWelcome.Click += new System.EventHandler(this.lblWelcome_Click);
            // 
            // lstLecturers
            // 
            this.lstLecturers.BackColor = System.Drawing.Color.CornflowerBlue;
            this.lstLecturers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstLecturers.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstLecturers.ForeColor = System.Drawing.Color.White;
            this.lstLecturers.FormattingEnabled = true;
            this.lstLecturers.ItemHeight = 17;
            this.lstLecturers.Items.AddRange(new object[] {
            "Ernest Bhero",
            "",
            "",
            "Ray Khuboni",
            "",
            "",
            "Bashan Naidoo",
            "",
            "",
            "Tahmid Quazi",
            "",
            "",
            "Jules-Raymond Tapamo",
            "",
            "",
            "Tom Walingo",
            "",
            "",
            "Parker Jonathan",
            "",
            "",
            "Sulaiman Patel",
            "",
            "",
            "Narushan Pillay",
            "",
            "",
            "Seare Rezenom"});
            this.lstLecturers.Location = new System.Drawing.Point(20, 175);
            this.lstLecturers.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.lstLecturers.Name = "lstLecturers";
            this.lstLecturers.Size = new System.Drawing.Size(157, 544);
            this.lstLecturers.TabIndex = 1;
            this.lstLecturers.SelectedIndexChanged += new System.EventHandler(this.lstLecturers_SelectedIndexChanged);
            // 
            // lblFacultyConnect
            // 
            this.lblFacultyConnect.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblFacultyConnect.AutoSize = true;
            this.lblFacultyConnect.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFacultyConnect.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.lblFacultyConnect.Location = new System.Drawing.Point(508, 23);
            this.lblFacultyConnect.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFacultyConnect.Name = "lblFacultyConnect";
            this.lblFacultyConnect.Size = new System.Drawing.Size(233, 32);
            this.lblFacultyConnect.TabIndex = 0;
            this.lblFacultyConnect.Text = "FACULTY CONNECT";
            this.lblFacultyConnect.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblFacultyConnect.Click += new System.EventHandler(this.lblFacultyConnect_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.lblFacultyConnect);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1158, 78);
            this.panel1.TabIndex = 5;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.Image = global::FacultyConnectApp.Properties.Resources.uzkn;
            this.pictureBox2.Location = new System.Drawing.Point(976, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(180, 78);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 6;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(442, 9);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(71, 64);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.panelVisitorMessage);
            this.panel3.Controls.Add(this.panelMessaging);
            this.panel3.Controls.Add(this.btnAudioCall);
            this.panel3.Controls.Add(this.btnVideoCall);
            this.panel3.Location = new System.Drawing.Point(240, 96);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(888, 567);
            this.panel3.TabIndex = 6;
            this.panel3.Paint += new System.Windows.Forms.PaintEventHandler(this.panelVisitorMessage_Paint);
            // 
            // panelVisitorMessage
            // 
            this.panelVisitorMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelVisitorMessage.BackColor = System.Drawing.SystemColors.Control;
            this.panelVisitorMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelVisitorMessage.Controls.Add(this.lblVisitorMessage);
            this.panelVisitorMessage.Controls.Add(this.statusLabel);
            this.panelVisitorMessage.Controls.Add(this.lblPurpose);
            this.panelVisitorMessage.Controls.Add(this.lblStudentNumber);
            this.panelVisitorMessage.Controls.Add(this.lblVisitorName);
            this.panelVisitorMessage.Controls.Add(this.lblTimestamp);
            this.panelVisitorMessage.Location = new System.Drawing.Point(18, 15);
            this.panelVisitorMessage.Name = "panelVisitorMessage";
            this.panelVisitorMessage.Size = new System.Drawing.Size(850, 167);
            this.panelVisitorMessage.TabIndex = 15;
            // 
            // lblVisitorMessage
            // 
            this.lblVisitorMessage.AutoSize = true;
            this.lblVisitorMessage.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVisitorMessage.Location = new System.Drawing.Point(10, 10);
            this.lblVisitorMessage.Name = "lblVisitorMessage";
            this.lblVisitorMessage.Size = new System.Drawing.Size(151, 25);
            this.lblVisitorMessage.TabIndex = 0;
            this.lblVisitorMessage.Text = "Visitor Message";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.Location = new System.Drawing.Point(12, 38);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(39, 13);
            this.statusLabel.TabIndex = 8;
            this.statusLabel.Text = "Status";
            this.statusLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // lblPurpose
            // 
            this.lblPurpose.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblPurpose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPurpose.Location = new System.Drawing.Point(1, 93);
            this.lblPurpose.Name = "lblPurpose";
            this.lblPurpose.Size = new System.Drawing.Size(0, 16);
            this.lblPurpose.TabIndex = 7;
            this.lblPurpose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStudentNumber
            // 
            this.lblStudentNumber.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblStudentNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStudentNumber.Location = new System.Drawing.Point(1, 63);
            this.lblStudentNumber.Name = "lblStudentNumber";
            this.lblStudentNumber.Size = new System.Drawing.Size(0, 16);
            this.lblStudentNumber.TabIndex = 6;
            this.lblStudentNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVisitorName
            // 
            this.lblVisitorName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblVisitorName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVisitorName.Location = new System.Drawing.Point(1, 33);
            this.lblVisitorName.Name = "lblVisitorName";
            this.lblVisitorName.Size = new System.Drawing.Size(0, 16);
            this.lblVisitorName.TabIndex = 5;
            this.lblVisitorName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTimestamp
            // 
            this.lblTimestamp.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblTimestamp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimestamp.Location = new System.Drawing.Point(1, 123);
            this.lblTimestamp.Name = "lblTimestamp";
            this.lblTimestamp.Size = new System.Drawing.Size(0, 16);
            this.lblTimestamp.TabIndex = 8;
            this.lblTimestamp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelMessaging
            // 
            this.panelMessaging.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMessaging.BackColor = System.Drawing.Color.CornflowerBlue;
            this.panelMessaging.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMessaging.Controls.Add(this.lblMessagingTitle);
            this.panelMessaging.Controls.Add(this.txtRecentMessage);
            this.panelMessaging.Controls.Add(this.bottomPanel);
            this.panelMessaging.Location = new System.Drawing.Point(18, 200);
            this.panelMessaging.Name = "panelMessaging";
            this.panelMessaging.Size = new System.Drawing.Size(850, 290);
            this.panelMessaging.TabIndex = 14;
            // 
            // lblMessagingTitle
            // 
            this.lblMessagingTitle.AutoSize = true;
            this.lblMessagingTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessagingTitle.Location = new System.Drawing.Point(10, 10);
            this.lblMessagingTitle.Name = "lblMessagingTitle";
            this.lblMessagingTitle.Size = new System.Drawing.Size(247, 21);
            this.lblMessagingTitle.TabIndex = 0;
            this.lblMessagingTitle.Text = "Virtual Receptionist Messaging";
            // 
            // txtRecentMessage
            // 
            this.txtRecentMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRecentMessage.BackColor = System.Drawing.Color.White;
            this.txtRecentMessage.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtRecentMessage.Location = new System.Drawing.Point(13, 40);
            this.txtRecentMessage.Multiline = true;
            this.txtRecentMessage.Name = "txtRecentMessage";
            this.txtRecentMessage.ReadOnly = true;
            this.txtRecentMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRecentMessage.Size = new System.Drawing.Size(824, 100);
            this.txtRecentMessage.TabIndex = 1;
            this.txtRecentMessage.TextChanged += new System.EventHandler(this.txtRecentMessage_TextChanged);
            // 
            // bottomPanel
            // 
            this.bottomPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bottomPanel.ColumnCount = 2;
            this.bottomPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 81.63266F));
            this.bottomPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.36734F));
            this.bottomPanel.Controls.Add(this.txtMessageInput, 0, 0);
            this.bottomPanel.Controls.Add(this.btnSendMessage, 1, 0);
            this.bottomPanel.Controls.Add(this.btnMessageHistory, 0, 1);
            this.bottomPanel.Location = new System.Drawing.Point(10, 160);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.RowCount = 2;
            this.bottomPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.bottomPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.bottomPanel.Size = new System.Drawing.Size(830, 120);
            this.bottomPanel.TabIndex = 5;
            // 
            // txtMessageInput
            // 
            this.txtMessageInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessageInput.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMessageInput.Location = new System.Drawing.Point(3, 3);
            this.txtMessageInput.Multiline = true;
            this.txtMessageInput.Name = "txtMessageInput";
            this.txtMessageInput.Size = new System.Drawing.Size(671, 66);
            this.txtMessageInput.TabIndex = 2;
            this.txtMessageInput.Text = "Type your message here...";
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.BackColor = System.Drawing.Color.White;
            this.btnSendMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSendMessage.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSendMessage.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.btnSendMessage.Location = new System.Drawing.Point(680, 3);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(147, 66);
            this.btnSendMessage.TabIndex = 3;
            this.btnSendMessage.Text = "Send";
            this.btnSendMessage.UseVisualStyleBackColor = false;
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // btnMessageHistory
            // 
            this.btnMessageHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMessageHistory.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMessageHistory.Location = new System.Drawing.Point(3, 75);
            this.btnMessageHistory.Name = "btnMessageHistory";
            this.btnMessageHistory.Size = new System.Drawing.Size(671, 42);
            this.btnMessageHistory.TabIndex = 4;
            this.btnMessageHistory.Text = "Message History";
            this.btnMessageHistory.UseVisualStyleBackColor = true;
            this.btnMessageHistory.Click += new System.EventHandler(this.btnMessageHistory_Click);
            // 
            // btnDirectTest
            // 
            this.btnDirectTest.Location = new System.Drawing.Point(965, 89);
            this.btnDirectTest.Name = "btnDirectTest";
            this.btnDirectTest.Size = new System.Drawing.Size(124, 23);
            this.btnDirectTest.TabIndex = 12;
            this.btnDirectTest.Text = "Direct UI Test";
            this.btnDirectTest.UseVisualStyleBackColor = true;
            this.btnDirectTest.Visible = false;
            this.btnDirectTest.Click += new System.EventHandler(this.btnDirectTest_Click);
            // 
            // btnTestRequest
            // 
            this.btnTestRequest.Location = new System.Drawing.Point(969, 150);
            this.btnTestRequest.Name = "btnTestRequest";
            this.btnTestRequest.Size = new System.Drawing.Size(124, 23);
            this.btnTestRequest.TabIndex = 11;
            this.btnTestRequest.Text = "Test Request";
            this.btnTestRequest.UseVisualStyleBackColor = true;
            this.btnTestRequest.Click += new System.EventHandler(this.btnTestRequest_Click);
            // 
            // btnRestartListener
            // 
            this.btnRestartListener.Location = new System.Drawing.Point(972, 92);
            this.btnRestartListener.Name = "btnRestartListener";
            this.btnRestartListener.Size = new System.Drawing.Size(104, 23);
            this.btnRestartListener.TabIndex = 10;
            this.btnRestartListener.Text = "Restart Listener";
            this.btnRestartListener.UseVisualStyleBackColor = true;
            this.btnRestartListener.Visible = false;
            this.btnRestartListener.Click += new System.EventHandler(this.btnRestartListener_Click);
            // 
            // btnTestFirebase
            // 
            this.btnTestFirebase.Location = new System.Drawing.Point(972, 121);
            this.btnTestFirebase.Name = "btnTestFirebase";
            this.btnTestFirebase.Size = new System.Drawing.Size(104, 23);
            this.btnTestFirebase.TabIndex = 9;
            this.btnTestFirebase.Text = "Test Firebase";
            this.btnTestFirebase.UseVisualStyleBackColor = true;
            this.btnTestFirebase.Visible = false;
            this.btnTestFirebase.Click += new System.EventHandler(this.btnTestFirebase_Click);
            // 
            // MainDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1156, 741);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.btnDirectTest);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnTestRequest);
            this.Controls.Add(this.panelSidebar);
            this.Controls.Add(this.btnRestartListener);
            this.Controls.Add(this.btnGrantAccess);
            this.Controls.Add(this.btnTestFirebase);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "MainDashboard";
            this.Text = "Faculty Connect – Dashboard";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainDashboard_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainDashboard_FormClosed);
            this.Load += new System.EventHandler(this.MainDashboard_Load);
            this.panelSidebar.ResumeLayout(false);
            this.panelSidebar.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panelVisitorMessage.ResumeLayout(false);
            this.panelVisitorMessage.PerformLayout();
            this.panelMessaging.ResumeLayout(false);
            this.panelMessaging.PerformLayout();
            this.bottomPanel.ResumeLayout(false);
            this.bottomPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Button btnGrantAccess;
        internal System.Windows.Forms.Button btnAudioCall;
        internal System.Windows.Forms.Button btnVideoCall;
        internal System.Windows.Forms.Panel panelSidebar;
        internal System.Windows.Forms.Label lblFacultyConnect;
        internal System.Windows.Forms.ListBox lstLecturers;
        internal System.Windows.Forms.Label lblWelcome;
        internal System.Windows.Forms.Panel panel1;
        internal System.Windows.Forms.PictureBox pictureBox1;
        internal System.Windows.Forms.PictureBox pictureBox2;
        internal System.Windows.Forms.Panel panel3;
        internal System.Windows.Forms.Label lblVisitorMessage;
        internal System.Windows.Forms.Label lblVisitorName;
        internal System.Windows.Forms.Label lblPurpose;
        internal System.Windows.Forms.Label lblStudentNumber;
        internal System.Windows.Forms.Label statusLabel;
        internal System.Windows.Forms.Button btnRestartListener;
        internal System.Windows.Forms.Button btnTestFirebase;
        internal System.Windows.Forms.Button btnTestRequest;
        internal System.Windows.Forms.Label lblTimestamp;
        internal Button btnDirectTest;
        private Button btnSendMessage;
        private TextBox txtMessageInput;
        private TextBox txtRecentMessage;
        private Label lblMessagingTitle;
        private Panel panelMessaging;
        private Button btnMessageHistory;
        private Panel panelVisitorMessage;
        private TableLayoutPanel bottomPanel;
    }
}