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
    
    public partial class InputProfileWindow : Window
    {
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public InputProfile NewProfile {
            get; private set;
        }
        private InputProfile _existingProfile;
        private bool _isEditMode = false;

        public InputProfileWindow(InputProfile profileToEdit = null)
        {
            InitializeComponent();

            if (profileToEdit != null)
            {
                _existingProfile = profileToEdit;
                _isEditMode = true;

                // Load values into UI
                ProfileNameTextBox.Text = _existingProfile.ProfileName;
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string profileName = ProfileNameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(profileName))
            {
                MessageBox.Show("Please enter a profile name.", "Missing Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string dateNow = DateTime.Now.ToString("yyyy-MM-dd");

            NewProfile = new InputProfile
            {
                ProfileName = profileName,
                DateCreated = dateNow,
                LastUpdated = dateNow
            };

            this.DialogResult = true;
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}