using FYP.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace FYP
{
    public partial class UpdateProfileWindow : Window
    {
        public ObservableCollection<KeyMapping> Mappings { get; set; }
        private DeviceProfile _profile;

        public UpdateProfileWindow(DeviceProfile profileToEdit)
        {
            InitializeComponent();

            _profile = profileToEdit;

            // Bind the profile's mappings
            Mappings = _profile.Mappings ?? new ObservableCollection<KeyMapping>();
            txtMappings.ItemsSource = Mappings;

            // Load other profile details
            txtProfileName.Text = _profile.ProfileName;
        }

        private void AddMapping_Click(object sender, RoutedEventArgs e)
        {
            Mappings.Add(new KeyMapping { ActionName = "NewAction", Key = "" });
        }

        private void RemoveMapping_Click(object sender, RoutedEventArgs e)
        {
            if (txtMappings.SelectedItem is KeyMapping selected)
            {
                Mappings.Remove(selected);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string profileName = txtProfileName.Text.Trim();

            if (string.IsNullOrEmpty(profileName))
            {
                MessageBox.Show("Profile name cannot be empty.");
                return;
            }

            // Update the profile object
            _profile.ProfileName = profileName;
            _profile.Mappings = Mappings;

            // TODO: Save _profile to database or JSON file here

            MessageBox.Show($"Profile '{_profile.ProfileName}' updated with {Mappings.Count} mappings.");
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

}
