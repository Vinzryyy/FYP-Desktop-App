using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class InputProfile
    {
        public string Id { get; set; }  // Optional for Firebase key
        public string ProfileName { get; set; }
        public string DateCreated { get; set; }
        public string LastUpdated { get; set; }
    }
}
