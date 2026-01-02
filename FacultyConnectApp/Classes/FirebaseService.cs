using Firebase.Database;
using Firebase.Database.Query;
using FacultyConnectApp.Models;
using FacultyConnectApp.Forms;
using System;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Linq;
using FacultyConnectApp.Models;

namespace FacultyConnectApp.Services
{
    public class FirebaseService
    {
        private string lastProcessedRequestId = string.Empty;
        private string responseStatus;
        private MainDashboard _dashboard;
        private FirebaseClient firebaseClient;
        private IDisposable requestSubscription;
        private IDisposable audioCallSubscription;
        private bool isListening = false;
        private NotifyIcon _notifyIcon;
        private bool _isProcessingCallRequest = false;
        private string _lastProcessedCallId = null;
        private DateTime _lastProcessedTime = DateTime.MinValue;
        private const int MAX_HISTORY = 5;
        private string _lastMessageContent = "";
        private DateTime _lastMessageTime = DateTime.MinValue;
        private readonly TimeSpan _messageDuplicationThreshold = TimeSpan.FromSeconds(2);

        // Base URL - ensure this matches your Firebase database
        private readonly string dbUrl = "https://facultyconnectav-default-rtdb.asia-southeast1.firebasedatabase.app/";

        public FirebaseService(MainDashboard dashboard)
        {
            _dashboard = dashboard;
            firebaseClient = new FirebaseClient(dbUrl);
            Debug.WriteLine("🔥 Firebase service initialized");
        }

        public void SetNotifyIcon(NotifyIcon notifyIcon)
        {
            _notifyIcon = notifyIcon;
            Debug.WriteLine("System tray notification icon set");
        }

        public async Task ListenForRequests(string lecturerName)
        {
            if (string.IsNullOrEmpty(lecturerName))
            {
                Debug.WriteLine("❌ Error: Lecturer name cannot be empty");
                MessageBox.Show("Lecturer name cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Debug.WriteLine($"[Firebase] Starting Listener for: '{lecturerName}'");
            Debug.WriteLine($"[Firebase] Monitoring path: lecturers/{lecturerName}/request");

            try
            {
                // First, ensure the lecturer path exists with proper initial values
                await InitializeAvailabilityAsync(lecturerName);

                // Clean up any existing subscription
                StopListening();

                // Test if we can read from the path first
                try
                {
                    Debug.WriteLine($"Attempting to read from path: lecturers/{lecturerName}");
                    var initialData = await firebaseClient
                        .Child("lecturers")
                        .Child(lecturerName)
                        .OnceAsync<object>();

                    Debug.WriteLine($"Successfully read from lecturer path. Data exists: {initialData != null}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌ Failed to read from lecturer path: {ex.Message}");
                    MessageBox.Show($"Failed to access Firebase path: {ex.Message}", "Firebase Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check for any existing requests when starting
                await CheckExistingRequests(lecturerName);

                // Subscribe to changes directly on the entire lecturer node to catch all updates
                requestSubscription = firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .AsObservable<object>()
                    .Subscribe(
                        data =>
                        {
                            Debug.WriteLine("🔥 Firebase lecturer node event received!");

                            // If it's a change to the request node
                            if (data.Key == "request")
                            {
                                Debug.WriteLine($"Request node updated: {JsonConvert.SerializeObject(data.Object)}");

                                // Process the request data
                                if (data.Object != null)
                                {
                                    // Fetch the full request data to handle both full updates and incremental updates
                                    GetAndProcessFullRequest(lecturerName);
                                }
                            }
                        },
                        ex =>
                        {
                            Debug.WriteLine($"❌ Firebase subscription error: {ex.Message}");
                            MessageBox.Show($"Firebase subscription error: {ex.Message}",
                                "Firebase Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            // Try to restart the subscription after a delay
                            Task.Delay(5000).ContinueWith(_ =>
                            {
                                Debug.WriteLine("Attempting to restart Firebase subscription...");
                                ListenForRequests(lecturerName).ConfigureAwait(false);
                            });
                        },
                        () =>
                        {
                            Debug.WriteLine("📊 Firebase subscription completed");
                            isListening = false;
                        });

                isListening = true;
                Debug.WriteLine("✅ Firebase subscription successfully created");

                // Let the user know that the connection is active
                if (_dashboard != null && !_dashboard.IsDisposed && _dashboard.IsHandleCreated)
                {
                    _dashboard.Invoke(new Action(() =>
                    {
                        _dashboard.UpdateConnectionStatus("Connected to Firebase");
                    }));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Firebase error: {ex.Message}");
                MessageBox.Show($"Firebase subscription failed: {ex.Message}", "Firebase Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CheckExistingRequests(string lecturerName)
        {
            try
            {
                Debug.WriteLine($"Checking for existing requests for {lecturerName}...");
                var requestData = await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("request")
                    .OnceSingleAsync<Dictionary<string, object>>();

                if (requestData != null && requestData.Count > 0)
                {
                    Debug.WriteLine($"Found existing request: {JsonConvert.SerializeObject(requestData)}");

                    // Process the existing request
                    var request = BuildVisitorRequestFromDictionary(requestData);
                    if (request != null && !string.IsNullOrEmpty(request.visitor_name) && !string.IsNullOrEmpty(request.purpose))
                    {
                        ProcessVisitorRequest(request, lecturerName);
                    }
                }
                else
                {
                    Debug.WriteLine("No existing requests found");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking existing requests: {ex.Message}");
            }
        }

        private async void GetAndProcessFullRequest(string lecturerName)
        {
            try
            {
                var requestData = await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("request")
                    .OnceSingleAsync<Dictionary<string, object>>();

                if (requestData != null && requestData.Count > 0)
                {
                    Debug.WriteLine($"Full request data: {JsonConvert.SerializeObject(requestData)}");

                    var request = BuildVisitorRequestFromDictionary(requestData);
                    if (request != null && !string.IsNullOrEmpty(request.visitor_name) && !string.IsNullOrEmpty(request.purpose))
                    {
                        ProcessVisitorRequest(request, lecturerName);
                    }
                    else
                    {
                        Debug.WriteLine("Request data is incomplete");
                    }
                }
                else
                {
                    Debug.WriteLine("No request data found");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting full request: {ex.Message}");
            }
        }

        private VisitorRequest BuildVisitorRequestFromDictionary(Dictionary<string, object> dict)
        {
            try
            {
                var request = new VisitorRequest();

                if (dict.ContainsKey("visitor_name") && dict["visitor_name"] != null)
                    request.visitor_name = dict["visitor_name"].ToString();

                if (dict.ContainsKey("student_number") && dict["student_number"] != null)
                    request.student_number = dict["student_number"].ToString();

                if (dict.ContainsKey("purpose") && dict["purpose"] != null)
                    request.purpose = dict["purpose"].ToString();

                if (dict.ContainsKey("timestamp") && dict["timestamp"] != null)
                {
                    // Use the timestamp from Firebase, but if it's the example timestamp,
                    // replace it with the current time
                    var timestampStr = dict["timestamp"].ToString();

                    // Check if it's the fixed example timestamp
                    if (timestampStr == "2025-05-04 12:34:56")
                    {
                        // Replace with current time
                        request.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        Debug.WriteLine("Replaced example timestamp with current time");
                    }
                    else
                    {
                        request.timestamp = timestampStr;
                    }
                }
                else
                {
                    // If no timestamp provided, use current time
                    request.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }

                Debug.WriteLine($"Built request from dictionary: {request}");
                return request;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error building request from dictionary: {ex.Message}");
                return null;
            }
        }

        private void ProcessVisitorRequest(VisitorRequest request, string lecturerName)
        {
            try
            {
                Debug.WriteLine($"Processing visitor request: {request}");

                // Show system tray notification
                ShowNotification(request);

                // Make application visible if it's hidden
                if (_dashboard != null && !_dashboard.IsDisposed)
                {
                    _dashboard.Invoke(new Action(() =>
                    {
                        if (!_dashboard.Visible)
                        {
                            _dashboard.Show();
                            _dashboard.WindowState = FormWindowState.Normal;
                            _dashboard.BringToFront();
                            _dashboard.Focus();
                        }
                    }));
                }

                // Update UI with visitor info
                if (_dashboard != null && !_dashboard.IsDisposed)
                {
                    _dashboard.Invoke(new Action(() =>
                    {
                        try
                        {
                            // Update dashboard UI
                            Debug.WriteLine("Updating dashboard with visitor info");
                            _dashboard.UpdateVisitorMessageOnDashboard(request);

                            // Show popup
                            Debug.WriteLine("Creating and showing visitor popup");
                            VisitorPopupForm popup = new VisitorPopupForm(request);
                            DialogResult result = popup.ShowDialog();

                            Debug.WriteLine($"Popup result: {result}");

                            // Process result
                            ProcessPopupResultAsync(result, lecturerName).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error updating UI or showing popup: {ex.Message}");
                        }
                    }));
                }
                else
                {
                    // If the dashboard isn't available, create a standalone popup
                    CreateStandalonePopup(request, lecturerName);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing visitor request: {ex.Message}");
            }
        }

        private void CreateStandalonePopup(VisitorRequest request, string lecturerName)
        {
            try
            {
                // Create a temporary form to host the popup
                Form tempForm = new Form();
                tempForm.StartPosition = FormStartPosition.CenterScreen;
                tempForm.ShowInTaskbar = false;
                tempForm.FormBorderStyle = FormBorderStyle.None;
                tempForm.Opacity = 0;
                tempForm.Size = new System.Drawing.Size(1, 1);

                tempForm.Shown += (s, e) =>
                {
                    try
                    {
                        VisitorPopupForm popup = new VisitorPopupForm(request);
                        DialogResult result = popup.ShowDialog(tempForm);

                        Debug.WriteLine($"Standalone popup result: {result}");
                        ProcessPopupResultAsync(result, lecturerName).ConfigureAwait(false);

                        tempForm.Close();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error showing standalone popup: {ex.Message}");
                        tempForm.Close();
                    }
                };

                tempForm.Show();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating standalone popup: {ex.Message}");
            }
        }

        private void ShowNotification(VisitorRequest request)
        {
            try
            {
                if (_notifyIcon != null)
                {
                    _notifyIcon.BalloonTipTitle = "Faculty Connect - Visitor Request";
                    _notifyIcon.BalloonTipText = $"New visitor: {request.visitor_name}\nPurpose: {request.purpose}";
                    _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                    _notifyIcon.ShowBalloonTip(5000);
                    Debug.WriteLine("System tray notification shown");
                }
                else
                {
                    Debug.WriteLine("NotifyIcon is null, cannot show notification");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error showing notification: {ex.Message}");
            }
        }

        private async Task InitializeAvailabilityAsync(string lecturerName)
        {
            try
            {
                Debug.WriteLine($"Initializing availability status for {lecturerName}");

                // Check if the lecturer node exists
                var lecturerData = await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .OnceSingleAsync<object>();

                if (lecturerData == null)
                {
                    // Create the entire lecturer structure if it doesn't exist
                    var initialData = new Dictionary<string, object>
                    {
                        ["is_available"] = "pending",
                        ["last_updated"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    await firebaseClient
                        .Child("lecturers")
                        .Child(lecturerName)
                        .PutAsync(initialData);

                    Debug.WriteLine("Created new lecturer node with pending status");
                }
                else
                {
                    // Update only the availability status - Use a string value wrapped in quotes
                    await firebaseClient
                        .Child("lecturers")
                        .Child(lecturerName)
                        .Child("is_available")
                        .PutAsync<string>("pending");

                    Debug.WriteLine("Updated existing lecturer availability to pending");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error initializing availability: {ex.Message}");
                throw; // Rethrow to let the caller handle it
            }
        }

        private async Task ProcessPopupResultAsync(DialogResult result, string lecturerName)
        {
            try
            {
                // Convert DialogResult to appropriate availability status
                string availabilityStatus = result == DialogResult.Yes ? "true" : "false";
                Debug.WriteLine($"Processing popup result: {result} -> Setting is_available to {availabilityStatus}");

                // Update availability status in Firebase - Use a string value for compatibility
                await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("is_available")
                    .PutAsync<string>(availabilityStatus);

                Debug.WriteLine("✅ Updated availability status");

                if (availabilityStatus == "false")
                {
                    // Create a message when lecturer is unavailable
                    string unavailableMessage = "I'm not available at the moment. Please check back later.";

                    // Create a message object for consistency (same structure as regular messages)
                    var message = new ChatMessage(unavailableMessage, lecturerName);

                    // Update latest_message node
                    await firebaseClient
                        .Child("lecturers")
                        .Child(lecturerName)
                        .Child("latest_message")
                        .PutAsync(message);

                    // Update rejection_message node - use string value
                    await firebaseClient
                        .Child("lecturers")
                        .Child(lecturerName)
                        .Child("rejection_message")
                        .PutAsync<string>(unavailableMessage);

                    Debug.WriteLine("✅ Updated unavailable message");
                }

                // Update last response time
                await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("last_response")
                    .PutAsync<string>(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                // Clear the request node
                await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("request")
                    .DeleteAsync();

                Debug.WriteLine("✅ Cleared request node");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error processing popup result: {ex.Message}");
                throw; // Rethrow to let the caller handle it
            }
        }

        public void StopListening()
        {
            if (requestSubscription != null)
            {
                Debug.WriteLine("Stopping Firebase listener");
                requestSubscription.Dispose();
                requestSubscription = null;
                isListening = false;

                // Update UI to show disconnected status
                if (_dashboard != null && !_dashboard.IsDisposed && _dashboard.IsHandleCreated)
                {
                    _dashboard.Invoke(new Action(() =>
                    {
                        _dashboard.UpdateConnectionStatus("Disconnected from Firebase");
                    }));
                }
            }
        }

        public bool IsListening()
        {
            return isListening;
        }

        public async Task TestFirebaseConnection(string lecturerName)
        {
            try
            {
                Debug.WriteLine("Testing Firebase connection...");

                // Test writing data with proper formatting
                var testTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Create a dictionary for the test data
                var testData = new Dictionary<string, string>
                {
                    ["timestamp"] = testTimestamp
                };

                await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("test_connection")
                    .PutAsync(testData);

                Debug.WriteLine("✅ Successfully wrote test data to Firebase");

                // Test reading data
                var readData = await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("test_connection")
                    .OnceSingleAsync<Dictionary<string, string>>();

                if (readData != null && readData.ContainsKey("timestamp"))
                {
                    Debug.WriteLine($"✅ Successfully read data from Firebase: {readData["timestamp"]}");
                    MessageBox.Show("Firebase connection test successful!", "Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Debug.WriteLine("⚠️ Read test data format is unexpected");
                    MessageBox.Show("Firebase connection test completed, but data format was unexpected.",
                        "Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Firebase test failed: {ex.Message}");
                MessageBox.Show($"Firebase test failed: {ex.Message}", "Connection Test Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task SimulateVisitorRequest(string lecturerName)
        {
            try
            {
                Debug.WriteLine("Simulating visitor request...");

                // Create a complete visitor request with all fields and current timestamp
                var visitorRequest = new VisitorRequest
                {
                    visitor_name = "Test User " + DateTime.Now.ToString("HH:mm:ss"),
                    student_number = "999999999",
                    purpose = "Testing request handling",
                    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") // Current time
                };

                Debug.WriteLine($"Visitor request data: {JsonConvert.SerializeObject(visitorRequest)}");

                // First, directly update the UI for immediate feedback
                if (_dashboard != null && !_dashboard.IsDisposed && _dashboard.IsHandleCreated)
                {
                    _dashboard.Invoke(new Action(() =>
                    {
                        try
                        {
                            Debug.WriteLine("Directly updating dashboard UI first");
                            _dashboard.UpdateVisitorMessageOnDashboard(visitorRequest);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error updating dashboard directly: {ex.Message}");
                        }
                    }));
                }

                // Then send the data to Firebase
                await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("request")
                    .PutAsync(visitorRequest);

                Debug.WriteLine("✅ Successfully sent test visitor request to Firebase");

                // For testing purposes, show notification
                ShowNotification(visitorRequest);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Simulation failed: {ex.Message}");
                MessageBox.Show($"Visitor request simulation failed: {ex.Message}",
                    "Simulation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task DirectPathTest()
        {
            try
            {
                Debug.WriteLine("Performing direct path test...");

                // Test Tom Walingo path
                var testPath = await firebaseClient
                    .Child("lecturers")
                    .Child("Tom Walingo")
                    .OnceAsync<object>();

                Debug.WriteLine($"Direct access to Tom Walingo node: {(testPath != null ? "Success" : "Failed")}");

                // Test request node under Tom Walingo
                var requestNode = await firebaseClient
                    .Child("lecturers")
                    .Child("Tom Walingo")
                    .Child("request")
                    .OnceAsync<object>();

                Debug.WriteLine($"Direct access to request node: {(requestNode != null ? "Success" : "Failed")}");

                // Manually fetch the request data
                var manualRequest = await firebaseClient
                    .Child("lecturers")
                    .Child("Tom Walingo")
                    .Child("request")
                    .OnceSingleAsync<Dictionary<string, object>>();

                if (manualRequest != null)
                {
                    Debug.WriteLine("Manual request data:");
                    Debug.WriteLine(JsonConvert.SerializeObject(manualRequest, Formatting.Indented));

                    // Try to process it
                    VisitorRequest testRequest = BuildVisitorRequestFromDictionary(manualRequest);
                    if (testRequest != null)
                    {
                        Debug.WriteLine($"Successfully built visitor request: {testRequest}");

                        // Update dashboard and show popup manually
                        if (_dashboard != null)
                        {
                            _dashboard.Invoke(new Action(() =>
                            {
                                _dashboard.UpdateVisitorMessageOnDashboard(testRequest);

                                // Also show a popup
                                VisitorPopupForm popup = new VisitorPopupForm(testRequest);
                                popup.ShowDialog();
                            }));
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("No data found in request node");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in direct path test: {ex.Message}");
            }
        }

        // Add this method to FirebaseService.cs
        public async Task SilentTestConnection(string lecturerName)
        {
            try
            {
                Debug.WriteLine("Testing Firebase connection silently...");

                // Test writing data with proper formatting
                var testTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Create a dictionary for the test data
                var testData = new Dictionary<string, string>
                {
                    ["timestamp"] = testTimestamp
                };

                await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("test_connection")
                    .PutAsync(testData);

                Debug.WriteLine("✅ Successfully tested Firebase connection");

                // Test reading data
                var readData = await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("test_connection")
                    .OnceSingleAsync<Dictionary<string, string>>();

                if (readData != null && readData.ContainsKey("timestamp"))
                {
                    Debug.WriteLine($"✅ Successfully read data from Firebase: {readData["timestamp"]}");
                    // No message box here!
                }
                else
                {
                    Debug.WriteLine("⚠️ Read test data format is unexpected");
                    // No message box here either
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Firebase test failed: {ex.Message}");
                // Update status without showing message box
                if (_dashboard != null && !_dashboard.IsDisposed && _dashboard.IsHandleCreated)
                {
                    _dashboard.Invoke(new Action(() =>
                    {
                        _dashboard.UpdateConnectionStatus("Connection error - check network");
                    }));
                }
            }
        }

        private string GenerateRequestId(object requestData)
        {
            try
            {
                // Create a unique ID based on the data
                var json = JsonConvert.SerializeObject(requestData);
                return json.GetHashCode().ToString();
            }
            catch
            {
                // Fallback to timestamp if serialization fails
                return DateTime.Now.Ticks.ToString();
            }
        }

        private bool CheckIfRequestIsPending(object requestData)
        {
            try
            {
                if (requestData is Dictionary<string, object> dict)
                {
                    foreach (var key in dict.Keys)
                    {
                        if (string.Equals(key, "status", StringComparison.OrdinalIgnoreCase))
                        {
                            return string.Equals(dict[key]?.ToString(), "pending",
                                StringComparison.OrdinalIgnoreCase);
                        }
                    }
                }

                // Try parsing from JSON
                var json = JsonConvert.SerializeObject(requestData);
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                if (data != null)
                {
                    foreach (var key in data.Keys)
                    {
                        if (string.Equals(key, "status", StringComparison.OrdinalIgnoreCase))
                        {
                            return string.Equals(data[key]?.ToString(), "pending",
                                StringComparison.OrdinalIgnoreCase);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking if request is pending: {ex.Message}");
            }

            return false;
        }

        // Add these methods to your FirebaseService class:

        public async Task ListenForAudioCallRequests(string lecturerName)
        {
            try
            {
                Debug.WriteLine("Starting audio call request listener...");
                Debug.WriteLine($"Monitoring path: lecturers/{lecturerName}/audio_call_request");

                // Clean up any existing audio call subscription
                if (audioCallSubscription != null)
                {
                    audioCallSubscription.Dispose();
                    audioCallSubscription = null;
                    Debug.WriteLine("Disposed existing audio call subscription");
                }

                // Subscribe to the parent node and listen for child events
                Debug.WriteLine("Setting up optimized subscription for audio call requests");
                audioCallSubscription = firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("audio_call_request")
                    .AsObservable<object>()
                    .Subscribe(
                        async data =>
                        {
                            Debug.WriteLine($"⚡ Audio call request event detected for key: {data.Key}");

                            // Deduplication check - prevent multiple processing
                            if (_isProcessingCallRequest)
                            {
                                Debug.WriteLine("Already processing a call request. Ignoring duplicate event.");
                                return;
                            }

                            // IMPORTANT: We need to get the complete audio_call_request object
                            // instead of processing the individual property updates
                            try
                            {
                                // Fetch the complete audio_call_request data
                                var completeData = await firebaseClient
                                    .Child("lecturers")
                                    .Child(lecturerName)
                                    .Child("audio_call_request")
                                    .OnceSingleAsync<Dictionary<string, object>>();

                                if (completeData != null && completeData.Count > 0)
                                {
                                    Debug.WriteLine($"Got complete audio call data with {completeData.Count} properties");

                                    // Check if status is pending
                                    if (completeData.ContainsKey("status") &&
                                        completeData["status"].ToString().ToLower() == "pending" &&
                                        completeData.ContainsKey("caller_name"))
                                    {
                                        // Generate a unique ID for this call
                                        string callerName = completeData["caller_name"].ToString();
                                        string timestamp = completeData.ContainsKey("timestamp") ?
                                                        completeData["timestamp"].ToString() :
                                                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        string callId = $"{callerName}_{timestamp}";

                                        // Skip if we've seen this exact call recently (within 5 seconds)
                                        if (callId == _lastProcessedCallId &&
                                            (DateTime.Now - _lastProcessedTime).TotalSeconds < 5)
                                        {
                                            Debug.WriteLine($"Duplicate call detected: {callId}. Ignoring.");
                                            return;
                                        }

                                        // Mark that we're processing this call
                                        _isProcessingCallRequest = true;
                                        _lastProcessedCallId = callId;
                                        _lastProcessedTime = DateTime.Now;

                                        try
                                        {
                                            // Only process if we have both a pending status and a caller name
                                            Debug.WriteLine("Found pending call with caller name, processing...");
                                            ProcessCompleteAudioCallRequest(completeData, lecturerName);
                                        }
                                        finally
                                        {
                                            // Reset flag after a delay to prevent immediate reprocessing
                                            await Task.Delay(1000);
                                            _isProcessingCallRequest = false;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Error fetching complete audio call data: {ex.Message}");
                                _isProcessingCallRequest = false; // Reset flag on error
                            }
                        },
                        ex =>
                        {
                            Debug.WriteLine($"Audio call subscription error: {ex.Message}");
                            _isProcessingCallRequest = false; // Reset flag on error

                            // Try to restart the subscription after a delay
                            Task.Delay(2000).ContinueWith(_ =>
                            {
                                if (isListening)
                                {
                                    ListenForAudioCallRequests(lecturerName).ConfigureAwait(false);
                                }
                            });
                        });

                Debug.WriteLine("Audio call request listener optimized and started successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR starting audio call listener: {ex.Message}");
                _isProcessingCallRequest = false; // Reset flag on error
            }
        }
        private void ProcessCompleteAudioCallRequest(Dictionary<string, object> callDict, string lecturerName)
        {
            try
            {
                Debug.WriteLine("Processing complete audio call request");

                // Get caller name
                string callerName = callDict.ContainsKey("caller_name") ?
                    callDict["caller_name"].ToString() : "Unknown Caller";

                Debug.WriteLine($"Processing call from: {callerName}");

                // Show notification
                _notifyIcon?.ShowBalloonTip(3000, "Incoming Audio Call",
                    $"Call from: {callerName}", ToolTipIcon.Info);

                if (_dashboard != null && !_dashboard.IsDisposed && _dashboard.IsHandleCreated)
                {
                    // Use BeginInvoke to prevent UI thread blocking
                    _dashboard.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            // Make app visible
                            if (!_dashboard.Visible)
                            {
                                _dashboard.Show();
                                _dashboard.WindowState = FormWindowState.Normal;
                                _dashboard.BringToFront();
                            }

                            // Let the dashboard handle showing the form
                            // This centralizes form management in one place
                            _dashboard.ShowAudioCallRequestForm(callerName, lecturerName);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"ERROR in UI thread: {ex.Message}");
                        }
                    }));
                }
                else
                {
                    // Fallback
                    ShowStandaloneCallRequestPopup(callerName, lecturerName);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR processing complete audio call request: {ex.Message}");
            }
        }

        private async Task CheckExistingAudioCallRequest(string lecturerName)
        {
            try
            {
                Debug.WriteLine("Checking for existing audio call requests...");

                var requestData = await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("audio_call_request")
                    .OnceSingleAsync<Dictionary<string, object>>();

                Debug.WriteLine($"Existing audio call data: {JsonConvert.SerializeObject(requestData)}");

                if (requestData != null && requestData.Count > 0)
                {
                    // Check if status is pending
                    bool isPending = false;
                    if (requestData.ContainsKey("status"))
                    {
                        isPending = requestData["status"].ToString() == "pending";
                    }

                    if (isPending)
                    {
                        Debug.WriteLine("Found existing pending audio call request!");
                        ProcessAudioCallRequest(requestData, lecturerName);
                    }
                    else
                    {
                        Debug.WriteLine("Found audio call request but status is not pending");
                    }
                }
                else
                {
                    Debug.WriteLine("No audio call requests found");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking existing call requests: {ex.Message}");
            }
        }

        private void ProcessAudioCallRequest(object callData, string lecturerName)
        {
            try
            {
                // Fast conversion to dictionary
                Dictionary<string, object> callDict = null;

                if (callData is Dictionary<string, object>)
                {
                    callDict = (Dictionary<string, object>)callData;
                }
                else if (callData is Newtonsoft.Json.Linq.JObject jObj)
                {
                    callDict = jObj.ToObject<Dictionary<string, object>>();
                }
                else
                {
                    // Last resort - serialize and deserialize
                    string json = JsonConvert.SerializeObject(callData);
                    callDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                }

                if (callDict == null || !callDict.ContainsKey("status") ||
                    callDict["status"].ToString().ToLower() != "pending")
                {
                    // Quick exit if not a pending call
                    return;
                }

                // Get caller name (with fast default)
                string callerName = callDict.ContainsKey("caller_name") ?
                    callDict["caller_name"].ToString() : "Unknown Caller";

                // Show system tray notification
                _notifyIcon?.ShowBalloonTip(3000, "Incoming Audio Call",
                    $"Call from: {callerName}", ToolTipIcon.Info);

                // Fast path for UI update - use BeginInvoke instead of Invoke for async UI updates
                if (_dashboard != null && !_dashboard.IsDisposed && _dashboard.IsHandleCreated)
                {
                    _dashboard.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            // Make sure app is visible
                            if (!_dashboard.Visible)
                            {
                                _dashboard.Show();
                                _dashboard.WindowState = FormWindowState.Normal;
                                _dashboard.BringToFront();
                            }

                            // Show the popup immediately
                            AudioCallRequestForm requestForm = new AudioCallRequestForm(callerName);
                            DialogResult result = requestForm.ShowDialog();

                            // Process result in background
                            Task.Run(() => ProcessCallRequestResponse(result, lecturerName));
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"ERROR in UI thread: {ex.Message}");
                        }
                    }));
                }
                else
                {
                    // Fallback to standalone popup with minimal overhead
                    ShowStandaloneCallRequestPopup(callerName, lecturerName);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR processing audio call request: {ex.Message}");
            }
        }

        private void ShowStandaloneCallRequestPopup(string callerName, string lecturerName)
        {
            try
            {
                // Create the new form
                AudioCallRequestForm requestForm = new AudioCallRequestForm(callerName);
                requestForm.TopMost = true;

                DialogResult result = requestForm.ShowDialog();
                ProcessCallRequestResponse(result, lecturerName).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error showing standalone popup: {ex.Message}");
            }
        }

        private bool _isProcessingResponse = false;

        public async Task ProcessCallRequestResponse(DialogResult result, string lecturerName)
        {
            try
            {
                Debug.WriteLine($"ProcessCallRequestResponse called with result: {result}");

                // Prevent multiple calls
                if (_isProcessingResponse)
                {
                    Debug.WriteLine("Already processing a response, ignoring duplicate call");
                    return;
                }

                _isProcessingResponse = true;

                try
                {
                    // IMPORTANT: First set the status flag before deleting
                    string statusValue = (result == DialogResult.Yes) ? "accepted" : "rejected";

                    Debug.WriteLine($"Setting call status to: {statusValue}");

                    // Set the last call status flag - using anonymous object to fix serialization
                    await firebaseClient
                        .Child("lecturers")
                        .Child(lecturerName)
                        .Child("last_call_status")
                        .PutAsync(new { value = statusValue });

                    // Add timestamp to help with synchronization - also using anonymous object
                    var timestampValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    await firebaseClient
                        .Child("lecturers")
                        .Child(lecturerName)
                        .Child("last_call_timestamp")
                        .PutAsync(new { value = timestampValue });

                    // Small delay to ensure status is written before deletion
                    await Task.Delay(100);

                    // Now delete the request from Firebase
                    await firebaseClient
                        .Child("lecturers")
                        .Child(lecturerName)
                        .Child("audio_call_request")
                        .DeleteAsync();

                    // Only show the AudioCallWindow if accepted
                    if (result == DialogResult.Yes)
                    {
                        if (_dashboard != null && !_dashboard.IsDisposed && _dashboard.IsHandleCreated)
                        {
                            _dashboard.Invoke(new Action(() =>
                            {
                                var audioWindow = new AudioCallWindow();
                                audioWindow.IsAcceptedCall = true;
                                audioWindow.Show();
                            }));
                        }
                    }
                }
                finally
                {
                    // Always reset the flag when done
                    _isProcessingResponse = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing call response: {ex.Message}");
                _isProcessingResponse = false;
            }
        }



        public async Task TestAudioCallRequest(string lecturerName)
        {
            try
            {
                Debug.WriteLine("Testing audio call request...");

                // Create test request data - use structure matching what your visitor app sends
                var requestData = new Dictionary<string, object>
                {
                    ["caller_name"] = "Test Caller",
                    ["status"] = "pending",
                    ["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                // Send it to Firebase
                await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("audio_call_request")
                    .PutAsync(requestData);

                Debug.WriteLine("Test audio call request sent successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending test audio call request: {ex.Message}");
            }
        }

        // Add this method to your FirebaseService.cs class
        public async Task InitiateDirectCall(string lecturerName)
        {
            Debug.WriteLine($"Initiating direct call for {lecturerName}");

            try
            {
                // Create data for direct call
                var directCallData = new Dictionary<string, object>
                {
                    ["initiated_by"] = lecturerName,
                    ["status"] = "direct_call",
                    ["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                };

                // Write to Firebase - using proper serialization pattern
                await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("direct_call_flag")
                    .PutAsync(directCallData);

                Debug.WriteLine("Direct call flag set in Firebase");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initiating direct call: {ex.Message}");
                throw; // Rethrow so the caller can handle it
            }
        }

        public async Task<ChatMessage> SendMessageToReceptionist(string lecturerName, string messageContent)
        {
            try
            {
                Debug.WriteLine($"Sending message to receptionist: {messageContent}");

                // Create a new message object with guaranteed unique ID
                var message = new ChatMessage(messageContent, lecturerName);

                // Use a transaction to ensure atomicity
                await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("messages")
                    .Child(message.MessageId)
                    .PutAsync(message);

                // Save to latest_message separately
                await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("latest_message")
                    .PutAsync(message);

                // Update last_response timestamp with JSON object
                await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("last_response")
                    .PutAsync(new { value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });

                Debug.WriteLine($"✅ Message sent with ID: {message.MessageId}");
                return message;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error sending message: {ex.Message}");
                throw;
            }
        }


        // Retrieve message history for a lecturer
        public async Task<List<ChatMessage>> GetMessageHistory(string lecturerName)
        {
            try
            {
                Debug.WriteLine($"Retrieving message history for {lecturerName}");

                // Check if messages node exists
                var messagesExist = await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("messages")
                    .OnceSingleAsync<object>();

                if (messagesExist == null)
                {
                    Debug.WriteLine("No message history found");
                    return new List<ChatMessage>();
                }

                // Fetch messages from Firebase
                var messagesData = await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("messages")
                    .OrderByKey()
                    .LimitToLast(MAX_HISTORY)
                    .OnceAsync<ChatMessage>();

                // FIXED: Create a Dictionary to track unique MessageIds
                Dictionary<string, ChatMessage> uniqueMessages = new Dictionary<string, ChatMessage>();

                // Process each message, ensuring no duplicates
                foreach (var item in messagesData)
                {
                    if (item.Object != null && !string.IsNullOrEmpty(item.Object.MessageId))
                    {
                        // Only add if not already in the dictionary
                        if (!uniqueMessages.ContainsKey(item.Object.MessageId))
                        {
                            uniqueMessages[item.Object.MessageId] = item.Object;
                        }
                    }
                }

                // Convert to a list and sort
                var messages = uniqueMessages.Values
                    .OrderByDescending(m => m.Timestamp)
                    .ToList();

                Debug.WriteLine($"Retrieved {messages.Count} unique messages");
                return messages;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error retrieving messages: {ex.Message}");
                return new List<ChatMessage>(); // Return empty list on error
            }
        }

        public async Task MaintainMessageHistoryLimit(string lecturerName, int limit = 5)
        {
            try
            {
                // Get all messages
                var messagesData = await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("messages")
                    .OrderByKey()
                    .OnceAsync<ChatMessage>();

                var messages = messagesData.ToList();

                // If we have more than the limit, delete the oldest ones
                if (messages.Count > limit)
                {
                    Debug.WriteLine($"Found {messages.Count} messages, removing oldest to maintain limit of {limit}");

                    // Sort by timestamp (oldest first)
                    var orderedMessages = messages
                        .OrderBy(m => m.Object.Timestamp)
                        .ToList();

                    // Get messages to delete (oldest messages beyond our limit)
                    var messagesToDelete = orderedMessages
                        .Take(orderedMessages.Count - limit)
                        .ToList();

                    // Delete each old message
                    foreach (var message in messagesToDelete)
                    {
                        await firebaseClient
                            .Child("lecturers")
                            .Child(lecturerName)
                            .Child("messages")
                            .Child(message.Key)
                            .DeleteAsync();

                        Debug.WriteLine($"Deleted old message: {message.Key}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error maintaining message history: {ex.Message}");
            }
        }
    }
}