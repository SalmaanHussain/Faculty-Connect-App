using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;





namespace FacultyConnectApp.Forms
{
    public partial class VideoFeedWindow : Form
    {
        public VideoFeedWindow()
        {
            InitializeComponent();
        }

        private async void VideoFeedWindow_Load(object sender, EventArgs e)
        {
            
            StartVideoConnection();
        }

        private void btnEndCall_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSwitchToAudio_Click(object sender, EventArgs e)
        {
            try
            {
                // Create and show the audio call window
                AudioCallWindow audioWindow = new AudioCallWindow();
                audioWindow.Show();

                // Close the current video call window
                this.Close();

                Debug.WriteLine("Switched from video call to audio call");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error switching to audio call: {ex.Message}");
                MessageBox.Show($"Error switching to audio call: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void videoBrowser_Click(object sender, EventArgs e)
        {

        }

        private async void StartVideoConnection()
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync(serverIp, serverPort);
                stream = client.GetStream();
                isConnected = true;

                if (!isRunning)
                {
                    isRunning = true;
                    await Task.Run(() => ReceiveVideoStream());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection error: {ex.Message}");
            }
        }

        private async Task ReceiveVideoStream()
        {
            try
            {
                byte[] lengthBuffer = new byte[4];

                while (isConnected && isRunning)
                {
                    int bytesRead = await stream.ReadAsync(lengthBuffer, 0, 4);
                    if (bytesRead != 4)
                        break;

                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(lengthBuffer);

                    uint messageSize = BitConverter.ToUInt32(lengthBuffer, 0);

                    byte[] jpegBuffer = new byte[messageSize];
                    int totalBytesRead = 0;

                    while (totalBytesRead < messageSize)
                    {
                        int chunkSize = Math.Min((int)messageSize - totalBytesRead, 8192);
                        bytesRead = await stream.ReadAsync(jpegBuffer, totalBytesRead, chunkSize);
                        if (bytesRead == 0)
                            break;
                        totalBytesRead += bytesRead;
                    }

                    if (totalBytesRead == messageSize)
                    {
                        using (MemoryStream ms = new MemoryStream(jpegBuffer))
                        {
                            var bitmap = new Bitmap(ms);
                            this.Invoke(new Action(() =>
                            {
                                videoPictureBox.Image?.Dispose();
                                videoPictureBox.Image = new Bitmap(bitmap);
                            }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Stream error: {ex.Message}");
            }
            finally
            {
                isConnected = false;
                isRunning = false;
                stream?.Close();
                client?.Close();
            }
        }





        private TcpClient client;
        private NetworkStream stream;
        private bool isConnected = false;
        private bool isRunning = false;
        private string serverIp = "192.168.137.230"; // 📍 Pi IP here
        private int serverPort = 8485;             // 📍 Port Pi uses

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void videoPictureBox_Click(object sender, EventArgs e)
        {

        }

        private void panelTopBar_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
