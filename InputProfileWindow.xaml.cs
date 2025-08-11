using System;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FYP.Models;

namespace FYP
{
    public partial class InputProfileWindow : Window
    {
        private MappingReceiver receiver;
        private CancellationTokenSource cts = new();
        public InputProfile NewProfile { get; private set; }

        public InputProfileWindow()
        {
            InitializeComponent();
            StartListening();
        }

        private void StartListening()
        {
            receiver = new MappingReceiver();
            receiver.OnMappingReceived += mapping =>
            {
                Dispatcher.Invoke(() =>
                {
                    txtMappings.Items.Add(new KeyMapping { ActionName = "DefaultAction", Key = mapping });
                });
            };

            _ = receiver.StartListeningAsync(5005, cts.Token);
        }

        private void AddMapping_Click(object sender, RoutedEventArgs e)
        {
            // Add an empty mapping for the user to fill in
            txtMappings.Items.Add(new KeyMapping { ActionName = "NewAction", Key = "" });
        }

        private void RemoveMapping_Click(object sender, RoutedEventArgs e)
        {
            if (txtMappings.SelectedItem is KeyMapping selectedMapping)
            {
                txtMappings.Items.Remove(selectedMapping);
            }
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string profileName = txtProfileName.Text;

            var profile = new InputProfile
            {
                ProfileName = profileName,
                DateCreated = DateTime.Now.ToString("yyyy-MM-dd "),
                LastUpdated = DateTime.Now.ToString("yyyy-MM-dd "),
                Mappings = new ObservableCollection<FYP.Models.KeyMapping>() // Explicitly specify the correct KeyMapping type
            };

            foreach (FYP.Models.KeyMapping mapping in txtMappings.Items) // Ensure the correct type is used here
            {
                if (!string.IsNullOrWhiteSpace(mapping?.ActionName) && !string.IsNullOrWhiteSpace(mapping?.Key))
                {
                    profile.Mappings.Add(mapping);
                }
            }

            // Set NewProfile so MainWindow can read it
            NewProfile = profile;
            DialogResult = true; // So ShowDialog() returns true
            Close();
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            cts.Cancel();
            receiver?.Dispose();
            base.OnClosed(e);
        }
    }


    public class MappingReceiver : IDisposable
    {
        public event Action<string> OnMappingReceived;
        private UdpClient udpClient;

        public async Task StartListeningAsync(int port, CancellationToken token)
        {
            udpClient = new UdpClient(port);

            try
            {
                while (!token.IsCancellationRequested)
                {
                    var result = await udpClient.ReceiveAsync();
                    string mapping = Encoding.UTF8.GetString(result.Buffer);
                    OnMappingReceived?.Invoke(mapping);
                }
            }
            catch (ObjectDisposedException) { }
            catch (SocketException) { }
        }

        public void Dispose()
        {
            udpClient?.Close();
            udpClient?.Dispose();
        }
    }
}
