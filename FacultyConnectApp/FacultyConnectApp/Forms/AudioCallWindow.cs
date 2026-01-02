using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using NAudio.Wave;
using Firebase.Database;
using System.Diagnostics;
using Firebase.Database.Query;
using System.Threading;
using System.Net.Http;
using Newtonsoft.Json;

namespace FacultyConnectApp.Forms
{
    public partial class AudioCallWindow : Form
    {
        // Add these constants near the top of your class with the other private fields
         // Optimal buffer duration in milliseconds
        private WaveOut waveOut;
        private BufferedWaveProvider waveProvider;
        private Thread receiveThread;
        private bool isReceiving = false;
        public bool IsAcceptedCall { get; set; } = false;
        private string lecturerName = "Tom Walingo";
        private UdpClient udpReceiver;
        private const int listenPort = 5006;

        // Audio streaming related variables
        private UdpClient udpSender;
        private WaveInEvent waveIn;

        // IMPORTANT: Update with your actual Raspberry Pi IP address
        private string piIPAddress = "192.168.1.117";  // Replace with your Pi's actual IP!
        private int destinationPort = 5005;              // Same as what Pi is listening on

        public AudioCallWindow()
        {
            InitializeComponent();
            pictureBox1.SendToBack();
            pictureBox1.Dock = DockStyle.Fill;
        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                // End call button
                StopSendingAudio();

                // Add termination notification for Python app
                using (var client = new HttpClient())
                {
                    var terminationData = new
                    {
                        status = "terminated",
                        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                    };

                    var json = JsonConvert.SerializeObject(terminationData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    string firebaseUrl = "https://facultyconnectav-default-rtdb.asia-southeast1.firebasedatabase.app/" +
                        $"lecturers/{lecturerName}/call_termination.json";

                    await client.PutAsync(firebaseUrl, content);
                    Debug.WriteLine("Termination flag set in Firebase");
                }

                // Update the call status to terminated
                using (var client = new HttpClient())
                {
                    var statusData = new
                    {
                        value = "terminated"
                    };

                    var json = JsonConvert.SerializeObject(statusData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    string firebaseUrl = "https://facultyconnectav-default-rtdb.asia-southeast1.firebasedatabase.app/" +
                        $"lecturers/{lecturerName}/last_call_status.json";

                    await client.PutAsync(firebaseUrl, content);
                    Debug.WriteLine("Call status set to terminated in Firebase");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending termination signal: {ex.Message}");
            }
            finally
            {
                this.Close();
            }
        }

        private async void AudioCallWindow_Load(object sender, EventArgs e)
        {
            Debug.WriteLine($"⚠️ AUDIO CALL WINDOW LOADING - IsAcceptedCall: {IsAcceptedCall}");
            Debug.WriteLine("AudioCallWindow_Load starting - IsAcceptedCall: " + IsAcceptedCall);

            try
            {
                if (!IsAcceptedCall)
                {
                    Debug.WriteLine("Call not accepted, showing error message");
                    MessageBox.Show("Call was rejected or is no longer active",
                        "Call Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Debug.WriteLine("Closing window due to not accepted call");
                    this.Close();
                    return; // Important to return here
                }
                else
                {
                    Debug.WriteLine("Call accepted, initializing audio connection");
                    InitializeAudioConnection();

                    // Update the last_call_status in Firebase
                    await UpdateCallStatusInFirebase("accepted");
                }

                // Set up UI elements
                pictureBox1.Controls.Add(label1);
                pictureBox1.Controls.Add(button1);
                label1.BackColor = Color.Transparent;

                // Update UI to show active call
                label1.Text = "Audio Call Active";
                Debug.WriteLine("AudioCallWindow_Load completed successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in AudioCallWindow_Load: {ex.Message}");
                MessageBox.Show($"Error initializing call window: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private async Task UpdateCallStatusInFirebase(string status)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var statusData = new
                    {
                        value = status
                    };

                    var json = JsonConvert.SerializeObject(statusData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    string firebaseUrl = "https://facultyconnectav-default-rtdb.asia-southeast1.firebasedatabase.app/" +
                        $"lecturers/{lecturerName}/last_call_status.json";

                    await client.PutAsync(firebaseUrl, content);

                    // Also update timestamp
                    var timestampData = new
                    {
                        value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                    };

                    json = JsonConvert.SerializeObject(timestampData);
                    content = new StringContent(json, Encoding.UTF8, "application/json");

                    firebaseUrl = "https://facultyconnectav-default-rtdb.asia-southeast1.firebasedatabase.app/" +
                        $"lecturers/{lecturerName}/last_call_timestamp.json";

                    await client.PutAsync(firebaseUrl, content);

                    Debug.WriteLine($"Call status set to {status} in Firebase");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating call status: {ex.Message}");
            }
        }

        // Initialize the audio connection
        private void InitializeAudioConnection()
        {
            Debug.WriteLine("⚠️ SETTING UP AUDIO CONNECTION");
            Debug.WriteLine($"IsAcceptedCall = {IsAcceptedCall}");

            // Log audio device information
            Debug.WriteLine("Audio device information:");

            try
            {
                // Get output (playback) devices
                int outputDeviceCount = NAudio.Wave.WaveOut.DeviceCount;
                Debug.WriteLine($"Found {outputDeviceCount} output devices:");

                for (int i = 0; i < outputDeviceCount; i++)
                {
                    var capabilities = NAudio.Wave.WaveOut.GetCapabilities(i);
                    Debug.WriteLine($"Output Device {i}: {capabilities.ProductName}");
                }

                // Get input (recording) devices
                int inputDeviceCount = NAudio.Wave.WaveIn.DeviceCount;
                Debug.WriteLine($"Found {inputDeviceCount} input devices:");

                for (int i = 0; i < inputDeviceCount; i++)
                {
                    var capabilities = NAudio.Wave.WaveIn.GetCapabilities(i);
                    Debug.WriteLine($"Input Device {i}: {capabilities.ProductName}");
                }

                Debug.WriteLine("Initializing bidirectional audio connection...");

                // Start sending audio to the Pi/visitor laptop
                StartSendingAudio();

                // Start receiving audio from the Pi/visitor laptop
                StartReceivingAudio();

                Debug.WriteLine("Audio connection successfully initialized");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing audio connection: {ex.Message}");
                MessageBox.Show($"Failed to initialize audio connection: {ex.Message}",
                    "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw; // Re-throw to be handled by caller
            }
        }

        private void StartSendingAudio()
        {
            
            try
            {
                Debug.WriteLine("Starting to send audio...");

                udpSender = new UdpClient();
                waveIn = new WaveInEvent();
                waveIn.DeviceNumber = 0;  // First recording device (default microphone)
                waveIn.WaveFormat = new WaveFormat(8000, 16, 1); // 8000Hz, 16-bit, Mono
                waveIn.BufferMilliseconds = 50; // Lower latency buffer

                

                waveIn.DataAvailable += WaveIn_DataAvailable;
                waveIn.StartRecording();

                Debug.WriteLine("Audio sending started successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error starting microphone: {ex.Message}");
                MessageBox.Show("Error starting microphone: " + ex.Message);
            }
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            try
            {
                // Calculate audio level (to check if microphone is capturing anything)
                int sum = 0;
                for (int i = 0; i < e.BytesRecorded; i += 2)
                {
                    short sample = BitConverter.ToInt16(e.Buffer, i);
                    sum += Math.Abs(sample);
                }
                int level = sum / (e.BytesRecorded / 2); // Average level

                // Log audio level occasionally (not every packet to avoid console spam)
                if (DateTime.Now.Second % 5 == 0 && DateTime.Now.Millisecond < 100)
                {
                    Debug.WriteLine($"Sending audio packet - Level: {level}, Bytes: {e.BytesRecorded}");
                }

                // Send audio data
                udpSender.Send(e.Buffer, e.BytesRecorded, piIPAddress, destinationPort);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending audio: {ex.Message}");
            }
        }

        private void StopSendingAudio()
        {
            try
            {
                Debug.WriteLine("Stopping audio sending and receiving...");

                // Stop receiving
                isReceiving = false;
                if (udpReceiver != null)
                {
                    try { udpReceiver.Close(); } catch { }
                    udpReceiver = null;
                }

                // Stop audio output
                if (waveOut != null)
                {
                    try
                    {
                        waveOut.Stop();
                        waveOut.Dispose();
                    }
                    catch { }
                    waveOut = null;
                }

                // Stop sending
                if (waveIn != null)
                {
                    try
                    {
                        waveIn.StopRecording();
                        waveIn.Dispose();
                    }
                    catch { }
                    waveIn = null;
                }

                if (udpSender != null)
                {
                    try { udpSender.Close(); } catch { }
                    udpSender = null;
                }

                Debug.WriteLine("Audio stopped");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error stopping audio: {ex.Message}");
            }
        }

        private void StartReceivingAudio()
        {
            Debug.WriteLine("⚠️ STARTING TO RECEIVE AUDIO");
            Debug.WriteLine($"Current time: {DateTime.Now.ToString("HH:mm:ss.fff")}");
            try
            {
                Debug.WriteLine("Starting to receive audio...");

                // Close any existing receiver
                if (udpReceiver != null)
                {
                    try { udpReceiver.Close(); } catch { }
                    udpReceiver = null;
                }

                // Initialize audio output
                waveOut = new WaveOut();
                waveProvider = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));
                waveProvider.DiscardOnBufferOverflow = true;
                waveOut.Init(waveProvider);
                waveOut.Volume = 1.0f; // Maximum volume
                waveOut.Play();

                // Create and bind UDP client
                udpReceiver = new UdpClient(listenPort); // The port we listen on (should match Python's SEND_PORT)

                // Start receiving thread
                isReceiving = true;
                receiveThread = new Thread(ReceiveAudio);
                receiveThread.IsBackground = true;
                receiveThread.Start();

                Debug.WriteLine($"Audio receiver started on port {listenPort}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error starting audio receiver: {ex.Message}");
                MessageBox.Show("Error starting audio receiver: " + ex.Message);
            }
        }

        private void ReceiveAudio()
        {
            Debug.WriteLine("Audio receive thread started");
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            int packetCount = 0;

            try
            {
                while (isReceiving)
                {
                    try
                    {
                        byte[] buffer = udpReceiver.Receive(ref remoteEP);
                        packetCount++;

                        // Log occasionally
                        if (packetCount % 20 == 0)
                        {
                            // Calculate audio level for debugging
                            int sum = 0;
                            for (int i = 0; i < buffer.Length; i += 2)
                            {
                                if (i + 1 < buffer.Length)
                                {
                                    short sample = BitConverter.ToInt16(buffer, i);
                                    sum += Math.Abs(sample);
                                }
                            }
                            int level = buffer.Length > 0 ? sum / (buffer.Length / 2) : 0;

                            Debug.WriteLine($"Received packet #{packetCount} - Length: {buffer.Length}, Level: {level}");
                        }

                        // Add to audio buffer for playback
                        waveProvider.AddSamples(buffer, 0, buffer.Length);
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode == SocketError.TimedOut)
                        {
                            // Timeout is normal, just continue
                            continue;
                        }
                        Debug.WriteLine($"Socket error in receive: {ex.Message}");
                        Thread.Sleep(100);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error in audio receive: {ex.Message}");
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Fatal error in receive thread: {ex.Message}");
            }

            Debug.WriteLine("Audio receive thread ending");
        }

        // Clean up resources when the form is closing
        private void AudioCallWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopSendingAudio();
        }

        // These methods are empty but might be needed for designer
        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
    }
}
