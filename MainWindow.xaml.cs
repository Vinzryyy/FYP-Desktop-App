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
      
      
        private void GenerateNewQrCode_Click(object sender, RoutedEventArgs e)
        {
            GenerateAndShowQrCode();
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

        // Method to get local IP address
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

        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
        public ObservableCollection<DeviceProfile> Controllers { get; set; } = new();
        public ObservableCollection<InputProfile> InputProfiles { get; set; } = new();

        // CONSTRUCTOR
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            LoadInputMappings();


   

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

          


            InputProfiles.Add(new InputProfile { Id = 1 ,ProfileName = "FPS Profile", DateCreated = "2024-01-12", LastUpdated = "2024-05-01" });
            InputProfiles.Add(new InputProfile { Id = 2, ProfileName = "Racing Profile", DateCreated = "2024-02-15", LastUpdated = "2024-06-10" });
            InputProfiles.Add(new InputProfile { Id = 3, ProfileName = "Custom Profile", DateCreated = "2024-03-05", LastUpdated = "2024-07-01" });

            Controllers.Add(new DeviceProfile { Id = 1, DeviceName = "Alice iPhone 12", Status = "🔗", SelectedProfile = InputProfiles[0], SelectedProfileNum = SelectedProfileOptions[1] });
            Controllers.Add(new DeviceProfile { Id = 2, DeviceName = "Bob iPhone 14", Status = "🔗", SelectedProfile = InputProfiles[1], SelectedProfileNum = SelectedProfileOptions[2] });
            // ✅ Now populate controllers
        }

        // WebSocket server setup
      
        private async void SaveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var inputProfile = InputProfiles.FirstOrDefault(p => p.ProfileName == "FPS Profile");

            if (inputProfile == null)
            {
                MessageBox.Show("FPS Profile not found in InputProfiles.");
                return;
            }

            var profile = new DeviceProfile
            {
                ProfileName = inputProfile.ProfileName,
                DateCreated = DateTime.Now.ToString("yyyy-MM-dd"),
                LastUpdated = DateTime.Now.ToString("yyyy-MM-dd"),
                DeviceName = "Alice iPhone 12",
                Status = "Linked",
                SelectedProfile = inputProfile, 
                SelectedProfileNum = "1",
                IsLinked = true,
                NeedsUpdate = false
            };

            var firebase = new FirebaseService();
            await firebase.SaveProfileAsync(profile);
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

        // Cleanup when window is closing
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                server?.Dispose();
                Console.WriteLine("WebSocket server disposed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disposing server: {ex.Message}");
            }
            base.OnClosing(e);
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