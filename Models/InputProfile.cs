using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class InputProfile : INotifyPropertyChanged
    {
        public int Id { get; set; }
        private string _profileName;
        private string _dateCreated;
        private string _lastUpdated;

        public string ProfileName
        {
            get => _profileName;
            set { _profileName = value; OnPropertyChanged(nameof(ProfileName)); }
        }

        public string DateCreated
        {
            get => _dateCreated;
            set { _dateCreated = value; OnPropertyChanged(nameof(DateCreated)); }
        }

        public string LastUpdated
        {
            get => _lastUpdated;
            set { _lastUpdated = value; OnPropertyChanged(nameof(LastUpdated)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
