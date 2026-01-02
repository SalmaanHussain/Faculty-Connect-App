namespace FacultyConnectApp.Forms
{
    partial class RegisterForms
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegisterForms));
            this.UserBox = new System.Windows.Forms.TextBox();
            this.PassBox = new System.Windows.Forms.TextBox();
            this.ZipBox = new System.Windows.Forms.TextBox();
            this.GenBox = new System.Windows.Forms.ComboBox();
            this.BackToLoginBtn = new System.Windows.Forms.Button();
            this.RegBtn = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // UserBox
            // 
            this.UserBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.UserBox.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.UserBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UserBox.ForeColor = System.Drawing.Color.DimGray;
            this.UserBox.Location = new System.Drawing.Point(236, 132);
            this.UserBox.Name = "UserBox";
            this.UserBox.Size = new System.Drawing.Size(277, 25);
            this.UserBox.TabIndex = 0;
            this.UserBox.Text = "Username";
            // 
            // PassBox
            // 
            this.PassBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PassBox.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.PassBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PassBox.ForeColor = System.Drawing.Color.DimGray;
            this.PassBox.Location = new System.Drawing.Point(236, 178);
            this.PassBox.Name = "PassBox";
            this.PassBox.Size = new System.Drawing.Size(277, 25);
            this.PassBox.TabIndex = 2;
            this.PassBox.Text = "Password";
            // 
            // ZipBox
            // 
            this.ZipBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ZipBox.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ZipBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ZipBox.ForeColor = System.Drawing.Color.DimGray;
            this.ZipBox.Location = new System.Drawing.Point(236, 275);
            this.ZipBox.Name = "ZipBox";
            this.ZipBox.Size = new System.Drawing.Size(277, 25);
            this.ZipBox.TabIndex = 4;
            this.ZipBox.Text = "Zip Code";
            // 
            // GenBox
            // 
            this.GenBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.GenBox.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.GenBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GenBox.ForeColor = System.Drawing.Color.DimGray;
            this.GenBox.FormattingEnabled = true;
            this.GenBox.Items.AddRange(new object[] {
            "Male",
            "Female"});
            this.GenBox.Location = new System.Drawing.Point(236, 227);
            this.GenBox.Name = "GenBox";
            this.GenBox.Size = new System.Drawing.Size(277, 25);
            this.GenBox.TabIndex = 5;
            this.GenBox.Text = "Gender";
            // 
            // BackToLoginBtn
            // 
            this.BackToLoginBtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BackToLoginBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.BackToLoginBtn.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BackToLoginBtn.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BackToLoginBtn.Location = new System.Drawing.Point(236, 336);
            this.BackToLoginBtn.Name = "BackToLoginBtn";
            this.BackToLoginBtn.Size = new System.Drawing.Size(277, 25);
            this.BackToLoginBtn.TabIndex = 8;
            this.BackToLoginBtn.Text = "Back to Login";
            this.BackToLoginBtn.UseVisualStyleBackColor = false;
            this.BackToLoginBtn.Click += new System.EventHandler(this.button1_Click);
            // 
            // RegBtn
            // 
            this.RegBtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.RegBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.RegBtn.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RegBtn.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.RegBtn.Location = new System.Drawing.Point(236, 373);
            this.RegBtn.Name = "RegBtn";
            this.RegBtn.Size = new System.Drawing.Size(277, 25);
            this.RegBtn.TabIndex = 9;
            this.RegBtn.Text = "Register User";
            this.RegBtn.UseVisualStyleBackColor = false;
            this.RegBtn.Click += new System.EventHandler(this.RegBtn_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(316, 13);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(113, 112);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // RegisterForms
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(723, 433);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.RegBtn);
            this.Controls.Add(this.BackToLoginBtn);
            this.Controls.Add(this.GenBox);
            this.Controls.Add(this.ZipBox);
            this.Controls.Add(this.PassBox);
            this.Controls.Add(this.UserBox);
            this.Name = "RegisterForms";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RegisterForms";
            this.Load += new System.EventHandler(this.RegisterForms_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox UserBox;
        private System.Windows.Forms.TextBox PassBox;
        private System.Windows.Forms.TextBox ZipBox;
        private System.Windows.Forms.ComboBox GenBox;
        private System.Windows.Forms.Button BackToLoginBtn;
        private System.Windows.Forms.Button RegBtn;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}