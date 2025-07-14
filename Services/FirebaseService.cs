using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using FYP.Models;
using System.Threading.Tasks;

namespace FYP.Services
{
    class FirebaseService
    {
        private static readonly HttpClient client = new HttpClient();
        private const string baseUrl = "https://fyp-mob-controller-default-rtdb.asia-southeast1.firebasedatabase.app";

        public async Task SaveProfileAsync(DeviceProfile profile)
        {
            string url = $"{baseUrl}/device_profiles.json";

            string json = JsonSerializer.Serialize(profile);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Profile saved to Firebase!");
            }
            else
            {
                MessageBox.Show("Failed to save profile. Error: " + response.StatusCode);
            }
        }
    }
}
