using FacultyConnectApp.Classes;
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


namespace FacultyConnectApp.Forms
{
    public partial class RegisterForms : Form
    {
        public RegisterForms()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
            Form1 form = new Form1();
            form.ShowDialog();
            Close();
        }

        private void RegBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckIfuserAlreadyExists())
                {
                    MessageBox.Show("User Already Exist");
                    return;
                }
                var data = GetWriteData();
                DocumentReference docRef = FirestoreHelper.Database.Collection("Userdata").Document(data.Username);
                docRef.SetAsync(data);
                MessageBox.Show("Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private UserData GetWriteData()
        {
            string username = UserBox.Text.Trim();
            string password = Security.Encrypt(PassBox.Text);
            string gender = GenBox.Text.Trim();
            int zip = Convert.ToInt32(ZipBox.Text);

            return new UserData()
            {
                Username = username,
                Password = password,
                Gender = gender,
                ZipCode = zip
            };
        }

        private bool CheckIfuserAlreadyExists()
        {
            string username = UserBox.Text.Trim();
            DocumentReference docRef = FirestoreHelper.Database.Collection("Userdata").Document(username);
            UserData data = docRef.GetSnapshotAsync().Result.ConvertTo<UserData>();

            if (data != null)
            {
                return true;

            }
            return false;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void RegisterForms_Load(object sender, EventArgs e)
        {

        }
    }

            
    
}
