using Fleck;
using FYP.Models;
using QRCoder;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FYP.ReceiverApp;
using ReceiverApp;
using System.Windows;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using FYP.Services;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FYP
{
    public partial class MainWindow : Window
    {
        private readonly Action clientPairedHandler;
        private readonly ObservableCollection<ConnectedDevice> connectedDevices = new();
        private int deviceCounter = 1;
        private int nextInputProfileId = 4;



        //
        private WebSocketServer server;
        private IWebSocketConnection connectedClient;
      

        //for json interpreting
        private Dictionary<string, string> inputMap = new Dictionary<string, string>();

        private void SimulateInput(string mappedAction, string state)
        {
            if (!string.IsNullOrWhiteSpace(mappedAction))
            {
                ActionExecutor.Execute(mappedAction, state);
            }
        }



        private void LoadInputMappings()
        {
            try
            {
                string mappingFilePath = "input_mapping.json";

                if (!File.Exists(mappingFilePath))
                {
                    MessageBox.Show("input_mapping.json file not found. Please ensure the file exists in the application directory.");
                    Console.WriteLine("input_mapping.json not found!");
                    return;
                }

                string mappingJson = File.ReadAllText(mappingFilePath);
                inputMap = JsonSerializer.Deserialize<Dictionary<string, string>>(mappingJson) ?? new Dictionary<string, string>();

                Console.WriteLine("Input mappings loaded successfully.");
                Console.WriteLine($"Loaded {inputMap.Count} mapping(s):");
                foreach (var mapping in inputMap)
                {
                    Console.WriteLine($"  {mapping.Key} -> {mapping.Value}");
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("input_mapping.json file not found. Please ensure the file exists in the application directory.");
                Console.WriteLine("input_mapping.json file not found!");
            }
            catch (JsonException jsonEx)
            {
                MessageBox.Show($"Invalid JSON format in input_mapping.json: {jsonEx.Message}");
                Console.WriteLine($"JSON parsing error: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load input mapping: " + ex.Message);
                Console.WriteLine($"Error loading mappings: {ex.Message}");
            }
        }

        private void UpdateInputProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var selector = new ProfileSelectorWindow(InputProfiles) { Owner = this };
            bool? result = selector.ShowDialog();

            if (result == true && selector.SelectedProfile != null)
            {
                var updateWindow = new UpdateProfileWindow(selector.SelectedProfile)
                {
                    Owner = this
                };

                if (updateWindow.ShowDialog() == true)
                {
                    MessageBox.Show("Profile updated successfully.");
                }
            }
        }

        private void CreateInputProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var inputProfileWindow = new InputProfileWindow
            {
                Owner = this
            };

            bool? result = inputProfileWindow.ShowDialog();

            if (result == true && inputProfileWindow.NewProfile != null)
            {
                // Assign new ID here if needed
                inputProfileWindow.NewProfile.Id = nextInputProfileId++;

                InputProfiles.Add(inputProfileWindow.NewProfile);
                MessageBox.Show($"Profile '{inputProfileWindow.NewProfile.ProfileName}' created successfully.");
            }
        }

        private void OpenInputProfileWindow_Click(object sender, RoutedEventArgs e)
        {
            var window = new InputProfileWindow();
            window.Show();
        }
    
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ReceiverServer.OnClientPaired -= clientPairedHandler;
        }
        public class ConnectedDevice
        {
            public string PlayerId { get; set; } = "";
            public string DisplayName => $"Connected Device {PlayerId}";
        }



        private string GetLocalIPAddress()
        {
            foreach (var ip in System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address found.");
        }

        private void GenerateAndShowQrCode()
        {
            // Generatae a new token
            var token = Guid.NewGuid().ToString("N").Substring(0, 12);

            // Build URL
            string localIp = GetLocalIPAddress();
            var url = $"ws://{localIp}:8181?pair={token}";

            // Display Session Token
            SessionInfo.Text = url;

            // Generate QR Code as byte[]
            using var qrGen = new QRCodeGenerator();
            var qrData = qrGen.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            var pngQr = new PngByteQRCode(qrData);
            var qrBytes = pngQr.GetGraphic(20);

            // Convert to BitmapImage for UI
            QrCodeImage.Source = ConvertToBitmapImage(qrBytes);

            // Register token to server (playerId will be derived on connect)
            var nextPlayerId = $"Player{deviceCounter}";
            ReceiverServer.AddToken(token, nextPlayerId);
        }

        private BitmapImage ConvertToBitmapImage(byte[] imageData)
        {
            var bi = new BitmapImage();
            using var ms = new MemoryStream(imageData);
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.StreamSource = ms;
            bi.EndInit();
            bi.Freeze();
            return bi;
        }




        public ObservableCollection<DeviceProfile> Controllers { get; set; } = new();
        public ObservableCollection<InputProfile> InputProfiles { get; set; } = new();
        public ObservableCollection<DeviceProfile> FirebaseProfiles { get; set; } = new();

        // CONSTRUCTOR
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            LoadInputMappings();

            // Initialize FirebaseProfiles collection
            FirebaseProfiles = new ObservableCollection<DeviceProfile>();

            ConnectedDevicesGrid.ItemsSource = connectedDevices;
            // Initial QR generation
            GenerateAndShowQrCode();

            // When client pairs → add to device list + refresh QR
            clientPairedHandler = () =>
            {
                Dispatcher.Invoke(() =>
                {
                    var playerId = $"Player{deviceCounter++}";
                    connectedDevices.Add(new ConnectedDevice { PlayerId = playerId });
                    GenerateAndShowQrCode();
                });
            };

            ReceiverServer.OnClientPaired += () =>
            {
                Dispatcher.Invoke(() =>
                {
                    var playerId = $"Player{deviceCounter++}";
                    connectedDevices.Add(new ConnectedDevice { PlayerId = playerId });
                    GenerateAndShowQrCode();
                });
            };

            ReceiverServer.OnClientDisconnected += (playerId) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var device = connectedDevices.FirstOrDefault(d => d.PlayerId == playerId);
                    if (device != null)
                        connectedDevices.Remove(device);
                });
            };

            ReceiverServer.OnLog += (msg) =>
            {
                Dispatcher.Invoke(() => Log(msg));
            };




            InputProfiles.Add(new InputProfile { Id = 1 ,ProfileName = "FPS Profile", DateCreated = "2024-01-12", LastUpdated = "2024-05-01" });
            InputProfiles.Add(new InputProfile { Id = 2, ProfileName = "Racing Profile", DateCreated = "2024-02-15", LastUpdated = "2024-06-10" });
            InputProfiles.Add(new InputProfile { Id = 3, ProfileName = "Custom Profile", DateCreated = "2024-03-05", LastUpdated = "2024-07-01" });

            Controllers.Add(new DeviceProfile { Id = 1, DeviceName = "Alice iPhone 12", Status = "🔗", SelectedProfile = InputProfiles[0], SelectedProfileNum = SelectedProfileOptions[1] });
            Controllers.Add(new DeviceProfile { Id = 2, DeviceName = "Bob iPhone 14", Status = "🔗", SelectedProfile = InputProfiles[1], SelectedProfileNum = SelectedProfileOptions[2] });
            // ✅ Now populate controllers
        }

        private void GenerateNewQrCode_Click(object sender, RoutedEventArgs e)
        {
            GenerateAndShowQrCode();
        }

        private async void LoadFirebaseProfiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var firebase = new FirebaseService();
                var profiles = await firebase.LoadProfilesAsync(InputProfiles); // Pass InputProfiles collection

                FirebaseProfiles.Clear();
                foreach (var profile in profiles)
                {
                    FirebaseProfiles.Add(profile);
                }

                MessageBox.Show($"Loaded {profiles.Count} profiles from Firebase.", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load profiles: {ex.Message}", "Error");
            }
        }

        private async void DeleteFirebaseProfile_Click(object sender, RoutedEventArgs e)
        {
            if (FirebaseProfilesGrid.SelectedItem is DeviceProfile selectedProfile)
            {
                if (MessageBox.Show($"Delete profile '{selectedProfile.ProfileName}'?",
                    "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        var firebase = new FirebaseService();
                        await firebase.DeleteProfileAsync(selectedProfile);
                        FirebaseProfiles.Remove(selectedProfile);
                        MessageBox.Show("Profile deleted successfully.", "Success");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to delete profile: {ex.Message}", "Error");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a profile to delete.", "Warning");
            }
        }

        private async void SaveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var firebase = new FirebaseService(); // Declare ONCE at the start

            // 1. Try to save currently connected device
            var currentDevice = Controllers.FirstOrDefault(c => c.IsLinked);
            if (currentDevice != null)
            {
                currentDevice.LastUpdated = DateTime.Now.ToString("yyyy-MM-dd");
                currentDevice.NeedsUpdate = false;

                if (await firebase.SaveProfileAsync(currentDevice))
                {
                    MessageBox.Show($"Saved current profile for {currentDevice.DeviceName}");
                    return;
                }
            }

            // 2. Fallback to test profile
            var inputProfile = InputProfiles.FirstOrDefault(p => p.ProfileName == "FPS Profile");
            if (inputProfile == null)
            {
                MessageBox.Show("No connected device and FPS Profile not found");
                return;
            }

            await firebase.SaveProfileAsync(new DeviceProfile
            {
                ProfileName = inputProfile.ProfileName,
                DateCreated = DateTime.Now.ToString("yyyy-MM-dd"),
                LastUpdated = DateTime.Now.ToString("yyyy-MM-dd"),
                DeviceName = "Alice iPhone 12",
                Status = "Linked",
                SelectedProfile = inputProfile,
                SelectedProfileNum = "DS4 emulation",
                IsLinked = true,
                NeedsUpdate = false
            });

            MessageBox.Show("Saved test profile");
        }

        private void DeleteDevice_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is DeviceProfile profileToDelete)
            {
                if (MessageBox.Show($"Are you sure you want to delete '{profileToDelete.DeviceName}'?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Controllers.Remove(profileToDelete);
                }
            }
        }

        private void UpdateDevice_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is DeviceProfile deviceToUpdate)
            {
                // Example update logic
                deviceToUpdate.LastUpdated = DateTime.Now.ToString("yyyy-MM-dd");
                deviceToUpdate.NeedsUpdate = false;

                MessageBox.Show($"'{deviceToUpdate.DeviceName}' updated successfully.", "Update");
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Implementation for tab control selection change if needed
        }
        
        public List<string> SelectedProfileOptions = new(){
        "Keyboard n Mouse",
        "Xbox emulation",
        "DS4 emulation"
            };

     
        private void Log(string message)
        {
            LogListBox.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");

            // Auto-scroll to the newest item
            LogListBox.ScrollIntoView(LogListBox.Items[LogListBox.Items.Count - 1]);
        }
    }


    public class InputPacket
    {
        public string type { get; set; }
        public string id { get; set; }
        public string state { get; set; }
        public long timestamp { get; set; }
    }
}