using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace FacultyConnectApp.Forms
{
    public partial class AudioCallRequestForm : Form
    {
        private string callerName;

        public AudioCallRequestForm(string callerName)
        {
            InitializeComponent();
            this.callerName = callerName;
            Debug.WriteLine($"AudioCallRequestForm created for caller: {callerName}");
        }

        private void AudioCallRequestForm_Load(object sender, EventArgs e)
        {
            // Configure the form
            this.Text = "Incoming Audio Call";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;

            // Set caller info
            if (lblCallerName != null)
            {
                lblCallerName.Text = $"From: {callerName}";
            }

            Debug.WriteLine("AudioCallRequestForm_Load completed");
        }

        private void btnAccept_Click_1(object sender, EventArgs e)
        {
            Debug.WriteLine("Accept button clicked");
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btnReject_Click_1(object sender, EventArgs e)
        {
            Debug.WriteLine("Reject button clicked");
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        // Override this to ensure the form closes properly
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Set the DialogResult if not already set
            if (this.DialogResult == DialogResult.None)
            {
                this.DialogResult = DialogResult.Cancel;
            }

            base.OnFormClosing(e);
        }

        private void AudioCallRequestForm_Load_1(object sender, EventArgs e)
        {

        }

        private void lblCallerName_Click(object sender, EventArgs e)
        {

        }
    }
}