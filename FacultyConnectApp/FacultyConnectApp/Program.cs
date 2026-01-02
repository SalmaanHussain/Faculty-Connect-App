using FacultyConnectApp.Classes;
using FacultyConnectApp.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacultyConnectApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Use a mutex to ensure only one instance runs
            using (Mutex mutex = new Mutex(true, "FacultyConnectAppMutex", out bool isNewInstance))
            {
                if (isNewInstance)
                {
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    // Force terminate when application exits
                    Application.ApplicationExit += (s, e) => {
                        Debug.WriteLine("Application exit detected - forcing termination");
                        Environment.Exit(0);
                    };

                    // Also handle process exit
                    AppDomain.CurrentDomain.ProcessExit += (s, e) => {
                        Debug.WriteLine("Process exit detected - forcing termination");
                        Environment.Exit(0);
                    };

                    FirestoreHelper.SetEnvireomentVariable();
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    // Start with Form1 as originally intended
                    Application.Run(new Form1());
                }
                else
                {
                    // Application is already running
                    MessageBox.Show("FacultyConnectApp is already running.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
