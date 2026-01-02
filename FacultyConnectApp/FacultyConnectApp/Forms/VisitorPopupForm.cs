using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FacultyConnectApp.Models;
using FacultyConnectApp.Services;
using Newtonsoft.Json;
using static Google.Rpc.Context.AttributeContext.Types;

namespace FacultyConnectApp.Forms
{
    public partial class VisitorPopupForm : Form
    {
        private VisitorRequest _request;

        public VisitorPopupForm(VisitorRequest request)
        {
            InitializeComponent();
            _request = request;
            Debug.WriteLine($"Popup form created with visitor: {request.visitor_name}");
        }

        private void VisitorPopupForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Set form title
                this.Text = "Visitor Request";

                // Populate visitor information
                lblVisitorName.Text = "Visitor: " + _request.visitor_name;
                lblStudentNumber.Text = "Student #: " + _request.student_number;
                lblPurpose.Text = "Purpose: " + _request.purpose;

                Debug.WriteLine("Popup form loaded with visitor data");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading popup form: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Form Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void btnYes_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Accept button clicked");
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Decline button clicked");
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void VisitorPopupForm_Load_1(object sender, EventArgs e)
        {

        }

        private void lblStudentNumber_Click(object sender, EventArgs e)
        {

        }

        private void lblVisitorName_Click(object sender, EventArgs e)
        {

        }
    }
}
