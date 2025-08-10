using FYP.Models;
using QRCoder;
using System.Drawing;
using System.IO;
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


namespace FYP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class UpdateProfileWindow : Window
    {
        
        private InputProfile _profile;

        public UpdateProfileWindow(InputProfile profile)
        {
            InitializeComponent();
            _profile = profile;

            // Populate fields with existing data
            if (_profile != null)
            {
                ProfileNameTextBox.Text = _profile.ProfileName;
                // You can load other settings here as needed
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_profile == null)
            {
                MessageBox.Show("Profile not found.");
                return;
            }

            string updatedName = ProfileNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(updatedName))
            {
                MessageBox.Show("Profile name cannot be empty.");
                return;
            }

            _profile.ProfileName = updatedName;
            _profile.LastUpdated = DateTime.Now.ToString("yyyy-MM-dd");

            MessageBox.Show("Profile updated.");
            this.DialogResult = true;
            this.Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}