using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace FYP.Models
{
    public class DeviceProfile : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int SelectedProfileId { get; set; } // Links to InputProfile.Id

        private string _firebaseKey;
        public string FirebaseKey
        {
            get => _firebaseKey;
            set { _firebaseKey = value; OnPropertyChanged(); }
        }

        // Read-only property for display
        public string ProfileTypeDisplay => SelectedProfileNum switch
        {
            "1" => "Keyboard n Mouse",
            "2" => "Xbox emulation",
            "3" => "DS4 emulation",
            _ => "Unknown"
        };

        private string _profileName;
        public string ProfileName
        {
            get => _profileName;
            set { _profileName = value; OnPropertyChanged(); }
        }

        private string _dateCreated;
        public string DateCreated
        {
            get => _dateCreated;
            set { _dateCreated = value; OnPropertyChanged(); }
        }

        private string _lastUpdated;
        public string LastUpdated
        {
            get => _lastUpdated;
            set { _lastUpdated = value; OnPropertyChanged(); }
        }

        private string _deviceName;
        public string DeviceName
        {
            get => _deviceName;
            set { _deviceName = value; OnPropertyChanged(); }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        private InputProfile _selectedProfile;
        public InputProfile SelectedProfile
        {
            get => _selectedProfile;
            set { _selectedProfile = value; OnPropertyChanged(); }
        }
        
        private string _selectedProfileNum;
        public string SelectedProfileNum
        {
            get => _selectedProfileNum;
            set { _selectedProfileNum = value; OnPropertyChanged();
                    OnPropertyChanged(nameof(ProfileTypeDisplay)); // Update display text
             }
        }

        private bool _isLinked;
        public bool IsLinked
        {
            get => _isLinked;
            set { _isLinked = value; OnPropertyChanged(); OnPropertyChanged(nameof(LinkedBrush)); }
        }

        private bool _needsUpdate;
        public bool NeedsUpdate
        {
            get => _needsUpdate;
            set { _needsUpdate = value; OnPropertyChanged(); }
        }

        [JsonIgnore]
        public Brush LinkedBrush => IsLinked ? Brushes.Green : Brushes.Red;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
