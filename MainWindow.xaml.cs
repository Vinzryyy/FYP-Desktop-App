using Fleck;
using FYP.Models;
using QRCoder;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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

        private int nextInputProfileId = 4;

        // P/Invoke declarations for Windows API
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        // Constants for key events
        private const uint KEYEVENTF_KEYDOWN = 0x0000;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        // Constants for mouse events
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;

        // Virtual key codes
        private const byte VK_W = 0x57;
        private const byte VK_A = 0x41;
        private const byte VK_S = 0x53;
        private const byte VK_D = 0x44;

        //
        private WebSocketServer server;
        private IWebSocketConnection connectedClient;
      

        //for json interpreting
        private Dictionary<string, string> inputMap = new Dictionary<string, string>();

        private void SimulateInput(string mappedAction, string state)
        {
            switch (state)
            {
                case "down":
                case "hold":
                    SimulateKeyPress(mappedAction);
                    break;

                case "up":
                    SimulateKeyRelease(mappedAction);
                    break;

                default:
                    Console.WriteLine($"Unknown state: {state}");
                    break;
            }
        }

        private void SimulateKeyPress(string action)
        {
            switch (action)
            {
                case "Key_W":
                    keybd_event(VK_W, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    break;
                case "Key_A":
                    keybd_event(VK_A, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    break;
                case "Key_S":
                    keybd_event(VK_S, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    break;
                case "Key_D":
                    keybd_event(VK_D, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    break;
                case "LeftClick":
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
                    break;
                case "RightClick":
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
                    break;
                default:
                    Console.WriteLine($"Unknown press action: {action}");
                    break;
            }
        }

        private void SimulateKeyRelease(string action)
        {
            switch (action)
            {
                case "Key_W":
                    keybd_event(VK_W, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    break;
                case "Key_A":
                    keybd_event(VK_A, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    break;
                case "Key_S":
                    keybd_event(VK_S, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    break;
                case "Key_D":
                    keybd_event(VK_D, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    break;
                case "LeftClick":
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
                    break;
                case "RightClick":
                    mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, UIntPtr.Zero);
                    break;
                default:
                    Console.WriteLine($"Unknown release action: {action}");
                    break;
            }
        }

        // Maps input IDs to actions using the loaded JSON mapping
        private string MapInputIdToAction(string inputId)
        {
            if (inputMap.TryGetValue(inputId.ToLower(), out string mappedAction))
            {
                return mappedAction;
            }

            Console.WriteLine($"No mapping found for input ID: {inputId}");
            return null;
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
        private void ManualLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string localIp = GetLocalIPAddress();
                string serverInfo = $"WebSocket Server is running at: ws://{localIp}:8181";

                MessageBox.Show(serverInfo, "Manual Login Info", MessageBoxButton.OK, MessageBoxImage.Information);
                QrStatusText.Text += $"\n{serverInfo}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving server info: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void GenerateQrCode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Generate QR code with local IP address
                string localIp = GetLocalIPAddress();
                string qrData = $"{localIp}"; // Include port for clarity

                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q))
                using (QRCode qrCode = new QRCode(qrCodeData))
                using (Bitmap qrBitmap = qrCode.GetGraphic(20))
                {
                    QrDisplayImage.Source = ConvertBitmapToBitmapImage(qrBitmap);
                    QrStatusText.Text = $"QR code generated.\nWebSocket Server: {localIp}\nWaiting for connections...";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to generate QR code: {ex.Message}");
            }
        }

        // Method to get local IP address
        private string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString(); // IPv4
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting local IP: {ex.Message}");
            }

            return "127.0.0.1"; // Fallback to localhost
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
            StartWebSocketServer();

           
        
            InputProfiles.Add(new InputProfile { Id = 1 ,ProfileName = "FPS Profile", DateCreated = "2024-01-12", LastUpdated = "2024-05-01" });
            InputProfiles.Add(new InputProfile { Id = 2, ProfileName = "Racing Profile", DateCreated = "2024-02-15", LastUpdated = "2024-06-10" });
            InputProfiles.Add(new InputProfile { Id = 3, ProfileName = "Custom Profile", DateCreated = "2024-03-05", LastUpdated = "2024-07-01" });

            Controllers.Add(new DeviceProfile { Id = 1, DeviceName = "Alice iPhone 12", Status = "🔗", SelectedProfile = InputProfiles[0], SelectedProfileNum = "Xbox emulation" });
            Controllers.Add(new DeviceProfile { Id = 2, DeviceName = "Bob iPhone 14", Status = "🔗", SelectedProfile = InputProfiles[1], SelectedProfileNum = "Keyboard n Mouse" });
            // ✅ Now populate controllers
        }

        // WebSocket server setup
        private void StartWebSocketServer()
        {
            try
            {
                FleckLog.Level = LogLevel.Info;
                server = new WebSocketServer("ws://0.0.0.0:8181");

                server.Start(socket =>
                {
                    socket.OnOpen = () =>
                    {
                        connectedClient = socket;
                        Dispatcher.Invoke(() =>
                        {
                            QrStatusText.Text += "\nClient connected!";
                        });
                        Console.WriteLine("WebSocket client connected");
                    };

                    socket.OnClose = () =>
                    {
                        connectedClient = null;
                        Dispatcher.Invoke(() =>
                        {
                            QrStatusText.Text += "\nClient disconnected.";
                        });
                        Console.WriteLine("WebSocket client disconnected");
                    };

                    socket.OnMessage = message =>
                    {
                        try
                        {
                            Console.WriteLine($"Received message: {message}");

                            var inputs = JsonSerializer.Deserialize<List<InputPacket>>(message);

                            foreach (var input in inputs)
                            {

                                string mappedAction = MapInputIdToAction(input.id);

                                string log = $"Input: {input.id} - {input.state} | Mapped to: {mappedAction}";
                                Console.WriteLine(log);

                                Dispatcher.Invoke(() =>
                                {
                                    QrStatusText.Text += $"\n{log}";
                                });

                                // Map input ID to action using the loaded JSON mapping
                                //string mappedAction = MapInputIdToAction(input.id);

                                if (!string.IsNullOrEmpty(mappedAction))
                                {
                                    Console.WriteLine($"Executing action: {mappedAction}");
                                    SimulateInput(mappedAction, input.state);
                                }
                                else
                                {
                                    Console.WriteLine($"No action mapped for input: {input.id}");
                                }
                            }
                        }
                        catch (JsonException jsonEx)
                        {
                            string errorMsg = $"JSON parsing error: {jsonEx.Message}";
                            Console.WriteLine(errorMsg);
                            Dispatcher.Invoke(() =>
                            {
                                QrStatusText.Text += $"\n{errorMsg}";
                            });
                        }
                        catch (Exception ex)
                        {
                            string errorMsg = $"Error processing message: {ex.Message}";
                            Console.WriteLine(errorMsg);
                            Dispatcher.Invoke(() =>
                            {
                                QrStatusText.Text += $"\n{errorMsg}";
                            });
                        }
                    };

                    socket.OnError = exception =>
                    {
                        Console.WriteLine($"WebSocket error: {exception.Message}");
                        Dispatcher.Invoke(() =>
                        {
                            QrStatusText.Text += $"\nWebSocket error: {exception.Message}";
                        });
                    };
                });

                Console.WriteLine("WebSocket server started on ws://0.0.0.0:8181");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start WebSocket server: {ex.Message}");
                Console.WriteLine($"Server start error: {ex.Message}");
            }
        }
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

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Implementation for tab control selection change if needed
        }
        DeviceProfile profile = new DeviceProfile
        {
            ProfileName = "FPS Profile",
            DateCreated = DateTime.Now.ToString("yyyy-MM-dd"),
            // etc.
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