using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FYP.Models
{
    public class InputProfile : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _profileName;
        private string _dateCreated;
        private string _lastUpdated;
        private ObservableCollection<KeyMapping> _mappings = new ObservableCollection<KeyMapping>();

        public string ProfileName
        {
            get => _profileName;
            set
            {
                if (_profileName != value)
                {
                    _profileName = value;
                    OnPropertyChanged(nameof(ProfileName));
                }
            }
        }

        public string DateCreated
        {
            get => _dateCreated;
            set
            {
                if (_dateCreated != value)
                {
                    _dateCreated = value;
                    OnPropertyChanged(nameof(DateCreated));
                }
            }
        }

        public string LastUpdated
        {
            get => _lastUpdated;
            set
            {
                if (_lastUpdated != value)
                {
                    _lastUpdated = value;
                    OnPropertyChanged(nameof(LastUpdated));
                }
            }
        }

        public ObservableCollection<KeyMapping> Mappings
        {
            get => _mappings;
            set
            {
                if (_mappings != value)
                {
                    _mappings = value;
                    OnPropertyChanged(nameof(Mappings));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class KeyMapping
    {
        public string ActionName { get; set; }
        public string Key { get; set; }
    }
}
