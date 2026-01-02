using FacultyConnectApp.Forms;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FacultyConnectApp.Classes.UserData;
using static FacultyConnectApp.Classes.FirestoreHelper;
using FacultyConnectApp.Classes;
using System.Diagnostics;

namespace FacultyConnectApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
                string username = Username.Text.Trim();
                string password = Password.Text.Trim();

                try
                {
                    DocumentReference docRef = FirestoreHelper.Database.Collection("Userdata").Document(username);
                    DocumentSnapshot snapshot = docRef.GetSnapshotAsync().Result;

                    if (snapshot.Exists)
                    {
                        UserData data = snapshot.ConvertTo<UserData>();
                        if (Security.Decrypt(data.Password) == password)
                        {

                            // Hide the login form
                            this.Hide();

                            // Create and show the dashboard form
                            MainDashboard dashboard = new MainDashboard(data);
                            dashboard.FormClosed += (s, args) => this.Close(); // Close the app when dashboard is closed
                            dashboard.Show();
                        }
                        else
                        {
                            MessageBox.Show("Login Failed: Incorrect password");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Login Failed: User not found");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
          

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void BackToRegisterBtn_Click(object sender, EventArgs e)
        {
            Hide();
            RegisterForms form = new RegisterForms();
            form.ShowDialog();
            Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            // Only run this code in Debug mode
            #if DEBUG
            // Force terminate the application immediately
            Process.GetCurrentProcess().Kill();
            #endif
        }
    }
}
