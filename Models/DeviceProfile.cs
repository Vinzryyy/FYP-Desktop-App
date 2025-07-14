using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class DeviceProfile
    {
        public int Id { get; set; }  // Number used in the UI
        public string ProfileName { get; set; }
        public string DateCreated { get; set; }
        public string LastUpdated { get; set; }
        public string DeviceName { get; set; }
        public string Status { get; set; }
        public InputProfile SelectedProfile { get; set; }  
        public string SelectedProfileNum { get; set; }
        public bool IsLinked { get; set; }
        public bool NeedsUpdate { get; set; }

        // ✅ This is handy for your rectangle in the XAML
        public Brush LinkedBrush
        {
            get
            {
                return IsLinked ? Brushes.Green : Brushes.Red;
            }
        }
    }
}
