using QRCoder;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Fleck;
using System.Text.Json;
using System.Threading.Tasks;


namespace FYP
{

    


    public partial class MainWindow : Window
    {
        //
        private WebSocketServer server;
        private IWebSocketConnection connectedClient;



        private void UpdateInputProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var UpdateProfileWindow = new UpdateProfileWindow();
            UpdateProfileWindow.Owner = this;
            UpdateProfileWindow.ShowDialog(); // Use Show() if you want it modeless
        }
        
        private void CreateInputProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var InputProfileWindow = new InputProfileWindow();
            InputProfileWindow.Owner = this;
            InputProfileWindow.ShowDialog(); // Use Show() if you want it modeless
        }
        private void OpenInputProfileWindow_Click(object sender, RoutedEventArgs e)
        {
            var window = new InputProfileWindow();
            window.Show();
        }

        
        private void GenerateQrCode_Click(object sender, RoutedEventArgs e)
        {
            // Generate a one-time token (replace with your actual auth logic or GUID-based ID)
            //string oneTimeToken = Guid.NewGuid().ToString(); // You could also use a JWT or cryptographic token
            //string qrData = $"login:{oneTimeToken}";

            //this is for generating ip address qr code
            string localIp = GetLocalIPAddress();
            string qrData = $"{localIp}";

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q))
            using (QRCode qrCode = new QRCode(qrCodeData))
            using (Bitmap qrBitmap = qrCode.GetGraphic(20))
            {
                QrDisplayImage.Source = ConvertBitmapToBitmapImage(qrBitmap);
                //QrStatusText.Text = $"QR code generated with token:\n{oneTimeToken}";
                QrStatusText.Text = $"QR code generated with token:\n{localIp}";
            }

            // Optional: Store token server-side or in memory for validation later
            //StoreTokenForLogin(oneTimeToken);
        }

        //method to get local ip
        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList){
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString(); // IPv4
                }
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
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

        private void StoreTokenForLogin(string token)
        {
            // TODO: Send to server or store in secure session for verification when mobile logs in
            // Example: You could call an API here or store in a dictionary
            //Console.WriteLine($"Stored login token: {token}");
        }
        public MainWindow()
        {
            InitializeComponent();
            StartWebSocketServer();

        }

        //
        private void StartWebSocketServer()
        {
            FleckLog.Level = LogLevel.Info;
            server = new WebSocketServer("ws://0.0.0.0:8181");

            FleckLog.Level = LogLevel.Info;
            server = new WebSocketServer("ws://0.0.0.0:8181");

            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    connectedClient = socket;
                    Dispatcher.Invoke(() =>
                    {
                        QrStatusText.Text += "\nConnection opened.";
                    });
                };

                socket.OnClose = () =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        QrStatusText.Text += "\nConnection closed.";
                    });
                };

                socket.OnMessage = message =>
                {
                    try
                    {
                        var inputs = JsonSerializer.Deserialize<List<InputPacket>>(message);

                        foreach (var input in inputs)
                        {
                            string log = $"Received: {input.id} - {input.state}";
                            Console.WriteLine(log);
                            Dispatcher.Invoke(() =>
                            {
                                QrStatusText.Text += $"\n{log}";
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            QrStatusText.Text += $"\nError: {ex.Message}";
                        });
                    }
                };
            });

            Console.WriteLine("WebSocket server started.");
        }
      
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


    }


    public class InputPacket { 
        public string type {  get; set; }
        public string id { get; set; }
        public string state { get; set; }
        public long timestamp { get; set; }
    }

}