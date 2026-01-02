using FacultyConnectApp.Classes;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using System.IO;
using FacultyConnectApp.Services;
using FacultyConnectApp.Models;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Win32;
using Firebase.Database;


namespace FacultyConnectApp.Forms
{
    public partial class MainDashboard : Form
    {
        private List<ChatMessage> messageHistory = new List<ChatMessage>();
        private UserData currentUser;
        private string lecturerName;
        private FirebaseService firebaseService;
        private bool isConnected = false;
        private NotifyIcon notifyIcon;
        private List<VisitorRequest> visitorHistory = new List<VisitorRequest>();
        private const int MAX_HISTORY = 5;

        public MainDashboard(UserData userData)
        {
            InitializeComponent();
            currentUser = userData;

            btnTestRequest.Visible = false;
            btnDirectTest.Visible = false;
            btnTestFirebase.Visible = false;
            btnRestartListener.Visible = false;

            this.Resize += MainDashboard_Resize;

            btnSendMessage.Click -= btnSendMessage_Click;
            btnSendMessage.Click += btnSendMessage_Click;
        }

        private void MainDashboard_Resize(object sender, EventArgs e)
        {
            SetupMessagingLayout();
            CenterControls();

        }
        private void CenterControls()
        {
            try
            {

                int leftMargin = 20;
                int labelSpacing = 30;
                int startY = 30;

                lblVisitorName.Left = leftMargin;
                lblVisitorName.Top = startY;
                lblVisitorName.Width = panel3.Width - (leftMargin * 2);

                lblStudentNumber.Left = leftMargin;
                lblStudentNumber.Top = startY + labelSpacing;
                lblStudentNumber.Width = panel3.Width - (leftMargin * 2);

                lblPurpose.Left = leftMargin;
                lblPurpose.Top = startY + (labelSpacing * 2);
                lblPurpose.Width = panel3.Width - (leftMargin * 2);

                lblTimestamp.Left = leftMargin;
                lblTimestamp.Top = startY + (labelSpacing * 3);
                lblTimestamp.Width = panel3.Width - (leftMargin * 2);

                panelMessaging.Top = lblTimestamp.Bottom + 60;
                panelMessaging.Left = leftMargin;
                panelMessaging.Width = panel3.Width - (leftMargin * 2);

                int remainingHeight = panel3.Height - panelMessaging.Top - 70; 
                panelMessaging.Height = Math.Max(150, remainingHeight); 

                int buttonY = panel3.Height - 60;
                int buttonWidth = 200;

                int totalWidth = panel3.Width;
                int totalButtonWidth = buttonWidth * 3;
                int totalSpacing = totalWidth - totalButtonWidth;
                int buttonSpacing = totalSpacing / 4; 

                btnAudioCall.Top = buttonY;
                btnAudioCall.Left = buttonSpacing;
                btnAudioCall.Width = buttonWidth;

                btnVideoCall.Top = buttonY;
                btnVideoCall.Left = buttonSpacing * 3 + buttonWidth * 2;
                btnVideoCall.Width = buttonWidth;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CenterControls: {ex.Message}");
            }
        }


        private async void MainDashboard_Load(object sender, EventArgs e)
        {
           
            SetupMessagingLayout();

            this.Resize += MainDashboard_Resize;
            try
            {
                UpdateConnectionStatus("Initializing...");
                Debug.WriteLine("MainDashboard is initializing...");

                LoadLecturerIdentity();

                firebaseService = new FirebaseService(this);
                firebaseService.SetNotifyIcon(notifyIcon);

                if (!string.IsNullOrEmpty(lecturerName))
                {

                    Debug.WriteLine("Testing Firebase connection...");
                    UpdateConnectionStatus($"Testing connection for {lecturerName}...");
                    await firebaseService.SilentTestConnection(lecturerName);

                    Debug.WriteLine("Starting Firebase listener...");
                    UpdateConnectionStatus($"Starting listener for {lecturerName}...");
                    await firebaseService.ListenForRequests(lecturerName);

                    Debug.WriteLine("Starting audio call request listener...");
                    await firebaseService.ListenForAudioCallRequests(lecturerName);

                    isConnected = true;
                    UpdateConnectionStatus($"Connected: Listening for requests for {lecturerName}");
                    Debug.WriteLine($"Firebase listener started for {lecturerName}");
                }
                else
                {
                    MessageBox.Show("Lecturer name not loaded. Firebase listener not started.",
                        "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    UpdateConnectionStatus("Not connected - lecturer name missing");
                    Debug.WriteLine("Failed to start Firebase listener: Lecturer name is empty");
                }

             
                if (!string.IsNullOrEmpty(lecturerName))
                {
                    lblWelcome.Text = $"Welcome back,\nProf. Walingo";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in MainDashboard_Load: {ex.Message}");
                MessageBox.Show($"Error initializing dashboard: {ex.Message}",
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateConnectionStatus("Error: Failed to initialize");
            }
            
            InitializeMessaging();
        }

        private void InitializeSystemTray()
        {
            try
            {
                Debug.WriteLine("Initializing system tray icon");

                notifyIcon = new NotifyIcon();
                notifyIcon.Icon = SystemIcons.Application; 
                notifyIcon.Text = "Faculty Connect";
                notifyIcon.Visible = true;

                ContextMenuStrip menu = new ContextMenuStrip();
                menu.Items.Add("Open Dashboard", null, (s, e) => {
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                    this.BringToFront();
                    this.Focus();
                });
                menu.Items.Add("Exit Application", null, (s, e) => {
                    notifyIcon.Visible = false;
                    notifyIcon.Dispose();
                    Application.Exit();
                });
                notifyIcon.ContextMenuStrip = menu;

                notifyIcon.DoubleClick += (s, e) => {
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                    this.BringToFront();
                    this.Focus();
                };

                this.FormClosing += new FormClosingEventHandler(MainDashboard_FormClosing);
                this.FormClosed += new FormClosedEventHandler(MainDashboard_FormClosed);

                this.Resize += (s, e) => {
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.ShowInTaskbar = true;
                        notifyIcon.ShowBalloonTip(3000, "Faculty Connect",
                            "Application is running in the background", ToolTipIcon.Info);
                    }
                };

                Debug.WriteLine("System tray icon initialized successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing system tray: {ex.Message}");
            }
        }

        private void MainDashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; 
                this.Hide(); 

                notifyIcon.ShowBalloonTip(3000, "Faculty Connect",
                    "Application is still running in the system tray. Right-click to exit.",
                    ToolTipIcon.Info);
            }
        }

        private void MainDashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // Clean up resources
                if (notifyIcon != null)
                {
                    notifyIcon.Visible = false;
                    notifyIcon.Dispose();
                }

                // Stop Firebase listener
                firebaseService?.StopListening();

                // Force terminate if Visual Studio is in debug mode
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in FormClosed: {ex.Message}");
            }
        }

        private void SetStartupWithWindows(bool enable)
        {
            try
            {
                string appName = "FacultyConnectApp";
                string appPath = Application.ExecutablePath;

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    if (enable)
                    {
                        key.SetValue(appName, appPath);
                        Debug.WriteLine("Application set to start with Windows");
                    }
                    else
                    {
                        if (key.GetValue(appName) != null)
                        {
                            key.DeleteValue(appName);
                            Debug.WriteLine("Application removed from Windows startup");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting startup: {ex.Message}");
            }
        }

        private void LoadLecturerIdentity()
        {
            try
            {
                if (File.Exists("config.json"))
                {
                    var configText = File.ReadAllText("config.json");
                    var config = JsonConvert.DeserializeObject<Config>(configText);
                    lecturerName = config.lecturer_name;
                    Debug.WriteLine($"Loaded lecturer: {lecturerName}");
                }
                else
                {
                    Debug.WriteLine("Config file not found");
                    // For testing purposes, set a default name - use the exact name in your Firebase
                    lecturerName = "Tom Walingo";
                    Debug.WriteLine($"Using default lecturer name: {lecturerName}");
                    MessageBox.Show("Config file (config.json) not found. Using default lecturer name for testing.",
                        "Configuration Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading lecturer config: {ex.Message}");
                MessageBox.Show("Error reading lecturer config: " + ex.Message,
                    "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void UpdateVisitorMessageOnDashboard(VisitorRequest request)
        {
            try
            {
                Debug.WriteLine($"UpdateVisitorMessageOnDashboard called with visitor: {request.visitor_name}");

                if (this.InvokeRequired)
                {
                    Debug.WriteLine("Invoke required for UI update - using Invoke");
                    this.Invoke(new Action(() => UpdateVisitorMessageOnDashboard(request)));
                    return;
                }

                // Update visitor information with center-aligned text
                lblVisitorName.Text = "Visitor: " + request.visitor_name;
                lblStudentNumber.Text = "Student #: " + request.student_number;
                lblPurpose.Text = "Purpose: " + request.purpose;

                // Format and display timestamp
                try
                {
                    if (!string.IsNullOrEmpty(request.timestamp))
                    {
                        DateTime requestTime;
                        if (DateTime.TryParse(request.timestamp, out requestTime))
                        {
                            lblTimestamp.Text = "Time: " + requestTime.ToString("h:mm tt, MMM d");
                        }
                        else
                        {
                            lblTimestamp.Text = "Time: " + request.timestamp;
                        }
                    }
                    else
                    {
                        lblTimestamp.Text = "Time: " + DateTime.Now.ToString("h:mm tt, MMM d");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error formatting timestamp: {ex.Message}");
                    lblTimestamp.Text = "Time: " + DateTime.Now.ToString("h:mm tt, MMM d");
                }

                // Refresh the UI
                CenterControls(); // Recalculate positions

                lblVisitorName.Refresh(); lblTimestamp.Refresh();
                lblStudentNumber.Refresh();
                lblPurpose.Refresh();
                

                Debug.WriteLine("Dashboard labels updated successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdateVisitorMessageOnDashboard: {ex.Message}");
                MessageBox.Show($"Error updating visitor message: {ex.Message}",
                    "UI Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ShowVisitorHistory()
        {
            try
            {
                if (visitorHistory.Count == 0)
                {
                    MessageBox.Show("No visitor history available.",
                        "Visitor History", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Create a history display string
                StringBuilder history = new StringBuilder();
                history.AppendLine("Recent Visitors:");
                history.AppendLine();

                for (int i = 0; i < visitorHistory.Count; i++)
                {
                    var visitor = visitorHistory[i];
                    history.AppendLine($"{i + 1}. {visitor.visitor_name}");
                    history.AppendLine($"   Purpose: {visitor.purpose}");
                    history.AppendLine($"   Student #: {visitor.student_number}");

                    // Format timestamp
                    DateTime requestTime;
                    if (DateTime.TryParse(visitor.timestamp, out requestTime))
                        history.AppendLine($"   Time: {requestTime.ToString("h:mm tt, MMM d, yyyy")}");
                    else
                        history.AppendLine($"   Time: {visitor.timestamp}");

                    history.AppendLine();
                }

                MessageBox.Show(history.ToString(), "Visitor History", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error showing visitor history: {ex.Message}");
            }
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateConnectionStatus("Sending door access signal...");

                // Create proper JSON structure
                var data = new Dictionary<string, bool>
                {
                    ["access_granted"] = true
                };

                var json = JsonConvert.SerializeObject(data);

                using (var client = new HttpClient())
                {
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    string firebaseUrl = "https://facultyconnectdb-default-rtdb.asia-southeast1.firebasedatabase.app/door_control.json";

                    var response = await client.PutAsync(firebaseUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Access granted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Debug.WriteLine("Door access signal sent successfully");
                    }
                    else
                    {
                        MessageBox.Show($"Failed to send access signal. Status: {response.StatusCode}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Debug.WriteLine($"Failed to send door access signal: {response.StatusCode}");
                    }

                    // Restore previous connection status
                    UpdateConnectionStatus(isConnected ?
                        $"Connected: Listening for requests for {lecturerName}" :
                        "Not connected");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error granting access: {ex.Message}");
                MessageBox.Show($"Error granting access: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateConnectionStatus(isConnected ?
                    $"Connected: Listening for requests for {lecturerName}" :
                    "Not connected");
            }
        }

        public void UpdateConnectionStatus(string status)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => UpdateConnectionStatus(status)));
                    return;
                }

                // Update the status label
                if (statusLabel != null)
                {
                    statusLabel.Text = status;
                    statusLabel.Refresh(); // Force refresh to ensure visibility
                    Debug.WriteLine($"Connection status updated: {status}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating connection status: {ex.Message}");
            }
        }

        private void btnVideoCall_Click(object sender, EventArgs e)
        {
            try
            {
                var videoWindow = new VideoFeedWindow();
                videoWindow.Show();
                Debug.WriteLine("Video call window opened");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening video call: {ex.Message}");
                MessageBox.Show($"Error opening video call: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnAudioCall_Click(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("Audio call button clicked");

                // Use the existing lecturerName field instead of txtLecturerName.Text
                // Use the existing firebaseService field instead of _firebaseService

                if (string.IsNullOrEmpty(lecturerName))
                {
                    MessageBox.Show("Lecturer name not available", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // First, add the direct call method to your Firebase service
                await firebaseService.InitiateDirectCall(lecturerName);

                // Create and show the audio call window
                var audioWindow = new AudioCallWindow();
                audioWindow.IsAcceptedCall = true; // This tells the window it's an accepted call
                audioWindow.Show();

                Debug.WriteLine("Audio call window opened");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening audio call: {ex.Message}");
                MessageBox.Show($"Error opening audio call: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Test button method - correctly connected in Designer.cs now
        private async void btnTestFirebase_Click(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("Test Firebase button clicked");
                if (firebaseService != null && !string.IsNullOrEmpty(lecturerName))
                {
                    UpdateConnectionStatus($"Testing connection for {lecturerName}...");
                    await firebaseService.TestFirebaseConnection(lecturerName);
                    UpdateConnectionStatus(isConnected ?
                        $"Connected: Listening for requests for {lecturerName}" :
                        "Connection test completed");
                }
                else
                {
                    MessageBox.Show("Firebase service or lecturer name not initialized.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error testing Firebase connection: {ex.Message}");
                MessageBox.Show($"Error testing Firebase connection: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateConnectionStatus("Error: Failed to test connection");
            }
        }

        // Restart Firebase listener
        private async void btnRestartListener_Click(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("Restart listener button clicked");
                if (firebaseService != null && !string.IsNullOrEmpty(lecturerName))
                {
                    UpdateConnectionStatus($"Restarting listener for {lecturerName}...");

                    // Stop current listener
                    firebaseService.StopListening();

                    // Wait a moment
                    await Task.Delay(1000);

                    // Start new listener
                    await firebaseService.ListenForRequests(lecturerName);

                    isConnected = true;
                    UpdateConnectionStatus($"Connected: Listening for requests for {lecturerName}");
                    Debug.WriteLine($"Firebase listener restarted for {lecturerName}");
                }
                else
                {
                    MessageBox.Show("Firebase service or lecturer name not initialized.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error restarting Firebase listener: {ex.Message}");
                MessageBox.Show($"Error restarting Firebase listener: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateConnectionStatus("Error: Failed to restart listener");
                isConnected = false;
            }
        }

        // Test request
        private async void btnTestRequest_Click(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("Test Request button clicked");

                if (firebaseService != null && !string.IsNullOrEmpty(lecturerName))
                {
                    // Create a test request
                    var testRequest = new VisitorRequest
                    {
                        visitor_name = "Test User " + DateTime.Now.ToString("HH:mm:ss"),
                        student_number = "999999999",
                        purpose = "Testing request handling",
                        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    // Update UI first - this is working as confirmed
                    UpdateVisitorMessageOnDashboard(testRequest);
                    Debug.WriteLine("Dashboard updated directly first");

                    // Update Firebase after UI is updated
                    Debug.WriteLine("Simulating Firebase request after UI update");
                    UpdateConnectionStatus($"Simulating visitor request for {lecturerName}...");
                    await firebaseService.SimulateVisitorRequest(lecturerName);
                    UpdateConnectionStatus($"Connected: Listening for requests for {lecturerName}");
                }
                else
                {
                    MessageBox.Show("Firebase service or lecturer name not initialized.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error simulating visitor request: {ex.Message}");
                MessageBox.Show($"Error simulating visitor request: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateConnectionStatus("Error: Failed to simulate request");
            }
        }

        // Direct UI Test
        private void btnDirectTest_Click(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("Direct UI Test button clicked");

                // Create a test visitor request
                var testRequest = new VisitorRequest
                {
                    visitor_name = "Test Visitor",
                    student_number = "123456789",
                    purpose = "Direct UI Testing",
                    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                // Update the UI directly
                UpdateVisitorMessageOnDashboard(testRequest);
                Debug.WriteLine("Direct update to dashboard completed");

                // Show a test popup
                try
                {
                    Debug.WriteLine("Creating test popup form");
                    var popupForm = new VisitorPopupForm(testRequest);
                    Debug.WriteLine("Showing test popup form");
                    var result = popupForm.ShowDialog();
                    Debug.WriteLine($"Popup form result: {result}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error showing popup form: {ex.Message}");
                    MessageBox.Show($"Error showing popup: {ex.Message}",
                        "Popup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in direct UI test: {ex.Message}");
                MessageBox.Show($"Direct UI test failed: {ex.Message}",
                    "Test Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public class Config
        {
            public string lecturer_name { get; set; }
        }

        // Keep your existing UI event handlers
        private void panelSidebar_Paint(object sender, PaintEventArgs e) { }
        private void lstLecturers_SelectedIndexChanged(object sender, EventArgs e) { }
        private void panelSidebar_Paint_1(object sender, PaintEventArgs e) { }
        private void lblWelcome_Click(object sender, EventArgs e) { }
        private void lblFacultyConnect_Click(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void btnEndCall_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void panelVisitorMessage_Paint(object sender, PaintEventArgs e) { }

        private async void btnDirectPathTest_Click(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("Direct Firebase path test requested");
                if (firebaseService != null)
                {
                    await firebaseService.DirectPathTest();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in direct path test: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Path Test Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblTimestamp_Click(object sender, EventArgs e)
        {

        }

        private void btnViewHistory_Click(object sender, EventArgs e)
        {
            ShowVisitorHistory();
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
        private void SetupMessagingLayout()
        {
            // Calculate the ideal position to be above the bottom buttons
            int bottomButtonsTop = btnAudioCall.Top;
            // Position the panel above these buttons with some margin
            panelMessaging.Top = bottomButtonsTop - panelMessaging.Height - 20;

            // Ensure the panel is centered horizontally
            panelMessaging.Left = (panel3.Width - panelMessaging.Width) / 2;
            try
            {
                // Your existing layout code...

                // Make sure txtRecentMessage and other messaging elements are visible and properly sized
                txtRecentMessage.Visible = true;
                txtMessageInput.Visible = true;
                btnSendMessage.Visible = true;
                btnMessageHistory.Visible = true;

                // Position elements relative to panelMessaging
                if (panelMessaging != null)
                {
                    // Make sure recent message textbox has enough height
                    txtRecentMessage.Height = panelMessaging.Height / 3;

                    // Position message input and send button
                    int inputY = txtRecentMessage.Bottom + 10;
                    txtMessageInput.Top = inputY;
                    btnSendMessage.Top = inputY;

                    // Message history button at the bottom
                    btnMessageHistory.Top = txtMessageInput.Bottom + 10;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in SetupMessagingLayout: {ex.Message}");
            }
        }

        private void InitializeMessaging()
        {
            try
            {
                Debug.WriteLine("Initializing messaging UI...");

                // Set up message input box placeholder text
                txtMessageInput.Text = "Type your message here...";

                txtMessageInput.Enter += (s, e) => {
                    if (txtMessageInput.Text == "Type your message here...")
                    {
                        txtMessageInput.Text = "";
                    }
                };

                txtMessageInput.Leave += (s, e) => {
                    if (string.IsNullOrWhiteSpace(txtMessageInput.Text))
                    {
                        txtMessageInput.Text = "Type your message here...";
                    }
                };

                // Set up button event handlers
                btnSendMessage.Click += btnSendMessage_Click;
                btnMessageHistory.Click += btnMessageHistory_Click;

                // Make sure Enter key in the textbox sends the message
                txtMessageInput.KeyDown += (s, e) => {
                    if (e.KeyCode == Keys.Enter && !e.Shift)
                    {
                        e.SuppressKeyPress = true;
                        btnSendMessage_Click(s, e);
                    }
                };

                // Load initial message history
                LoadMessageHistory();

                Debug.WriteLine("Messaging UI initialized successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing messaging: {ex.Message}");
            }
        }

        private async Task LoadMessageHistory()
        {
            try
            {
                Debug.WriteLine("Loading message history...");

                // Clear existing history
                messageHistory.Clear();

                // Get message history from Firebase
                if (firebaseService != null && !string.IsNullOrEmpty(lecturerName))
                {
                    messageHistory = await firebaseService.GetMessageHistory(lecturerName);

                    // Display the most recent message if available
                    if (messageHistory.Count > 0)
                    {
                        DisplayRecentMessage(messageHistory[0]);
                    }
                    else
                    {
                        txtRecentMessage.Clear();
                    }

                    Debug.WriteLine($"Loaded {messageHistory.Count} messages from history");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading message history: {ex.Message}");
                txtRecentMessage.Clear();
            }
        }

        // Method to display the most recent message
        private void DisplayRecentMessage(ChatMessage message)
        {
            try
            {
                if (message == null)
                {
                    txtRecentMessage.Clear();
                    return;
                }

                // Update the recent message textbox
                txtRecentMessage.Text = $"{message.SenderName}: {message.Content}";

                Debug.WriteLine($"Displayed recent message: {message.Content}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error displaying message: {ex.Message}");
                txtRecentMessage.Clear();
            }
        }



        private void MainDashboard_Load_1(object sender, EventArgs e)
        {

        }

        private void panelMessaging_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblMessagingTitle_Click(object sender, EventArgs e)
        {

        }

        private void btnMessageHistory_Click(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("Displaying message history...");

                // Create a string to display message history
                if (messageHistory.Count == 0)
                {
                    MessageBox.Show("No message history available.", "Message History", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Create a formatted message history
                StringBuilder historyText = new StringBuilder("Message History:\n\n");

                foreach (var message in messageHistory)
                {
                    // Add timestamp if available
                    string timestamp = !string.IsNullOrEmpty(message.Timestamp) ?
                        $" ({message.Timestamp})" : "";

                    historyText.AppendLine($"{message.SenderName}{timestamp}: {message.Content}");
                    historyText.AppendLine(); // Extra line for readability
                }

                // Show message box with history
                MessageBox.Show(historyText.ToString(), "Message History", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Debug.WriteLine("Message history displayed successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error showing message history: {ex.Message}");
                MessageBox.Show($"Failed to display message history: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnSendMessage_Click(object sender, EventArgs e)
        {
            try
            {
                string messageText = txtMessageInput.Text;

                // Don't send empty messages or placeholder text
                if (string.IsNullOrWhiteSpace(messageText) || messageText == "Type your message here...")
                {
                    return;
                }

                Debug.WriteLine($"Sending message: {messageText}");

                // Disable button while sending
                btnSendMessage.Enabled = false;

                // Send the message
                if (firebaseService != null && !string.IsNullOrEmpty(lecturerName))
                {
                    var message = await firebaseService.SendMessageToReceptionist(lecturerName, messageText);

                    // Clear the input box
                    txtMessageInput.Text = "Type your message here...";

                    // Display the sent message in the recent message textbox
                    DisplayRecentMessage(message);

                    // Add to local history
                    messageHistory.Insert(0, message);

                    // Keep only MAX_HISTORY messages locally
                    if (messageHistory.Count > MAX_HISTORY)
                    {
                        messageHistory.RemoveAt(messageHistory.Count - 1);
                    }

                    // Maintain Firebase message history limit
                    await firebaseService.MaintainMessageHistoryLimit(lecturerName, MAX_HISTORY);
                }
                else
                {
                    MessageBox.Show("Cannot send message: Firebase service or lecturer name not initialized.",
                        "Message Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Re-enable button
                btnSendMessage.Enabled = true;

                Debug.WriteLine("Message sent and displayed successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending message: {ex.Message}");
                MessageBox.Show($"Failed to send message: {ex.Message}", "Message Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSendMessage.Enabled = true;
            }
        }

        // Add this to your MainDashboard.cs
        public void TestAudioCallForm()
        {
            try
            {
                Debug.WriteLine("Testing audio call form directly");
                var form = new AudioCallRequestForm("Direct Test Caller");
                form.ShowDialog();
                Debug.WriteLine("Audio call form displayed successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error showing form: {ex.Message}");
            }
        }

        // Add these fields at the class level
        private AudioCallRequestForm _currentCallForm = null;
        private object _callFormLock = new object();
        private bool _isProcessingAudioCall = false;

        // Add this method to your MainDashboard class
        public void ShowAudioCallRequestForm(string callerName, string lecturerName)
        {
            // Ensure we're on the UI thread
            if (InvokeRequired)
            {
                Invoke(new Action(() => ShowAudioCallRequestForm(callerName, lecturerName)));
                return;
            }

            lock (_callFormLock)
            {
                // Prevent duplicate processing
                if (_isProcessingAudioCall)
                {
                    Debug.WriteLine("Already processing an audio call. Ignoring duplicate.");
                    return;
                }

                _isProcessingAudioCall = true;

                try
                {
                    // If there's already a form open, close it first
                    if (_currentCallForm != null && !_currentCallForm.IsDisposed)
                    {
                        try
                        {
                            _currentCallForm.Close();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error closing existing call form: {ex.Message}");
                        }
                        _currentCallForm = null;
                    }

                    // Create and show the new call form
                    Debug.WriteLine($"Creating audio call form for {callerName}");
                    _currentCallForm = new AudioCallRequestForm(callerName);

                    // Show as dialog and handle the result
                    DialogResult result = _currentCallForm.ShowDialog(this);
                    Debug.WriteLine($"Audio call form result: {result}");

                    // Forward the result to Firebase Service
                    firebaseService.ProcessCallRequestResponse(result, lecturerName).ConfigureAwait(false);

                    // Clear the reference
                    _currentCallForm = null;
                }
                finally
                {
                    // Always reset the processing flag when done
                    _isProcessingAudioCall = false;
                }
            }
        }

        private void txtRecentMessage_TextChanged(object sender, EventArgs e)
        {

        }
    }
}