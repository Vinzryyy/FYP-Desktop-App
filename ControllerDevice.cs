using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FYP
{
    class ControllerDevice
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string SelectedInputProfile { get; set; }
        public string SelectedProfileNumber { get; set; }
        public bool IsLinked { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
