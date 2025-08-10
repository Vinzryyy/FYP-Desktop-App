using FYP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows;

namespace FYP.Services
{
    class FirebaseService
    {
        private static readonly HttpClient client = new HttpClient();
        private const string baseUrl = "https://fyp-mob-controller-default-rtdb.asia-southeast1.firebasedatabase.app";

        public async Task<List<DeviceProfile>> LoadProfilesAsync()
        {
            string url = $"{baseUrl}/device_profiles.json";

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(json) && json != "null")
                {
                    var profilesDict = JsonSerializer.Deserialize<Dictionary<string, DeviceProfile>>(json);
                    if (profilesDict != null)
                    {
                        // Set the FirebaseKey for each profile (used for updates/deletes)
                        foreach (var kvp in profilesDict)
                        {
                            kvp.Value.FirebaseKey = kvp.Key;
                        }
                        return profilesDict.Values.ToList();
                    }
                }
            }

            return new List<DeviceProfile>();
        }

        public async Task DeleteProfileAsync(DeviceProfile profile)
        {
            if (string.IsNullOrEmpty(profile.FirebaseKey))
            {
                throw new InvalidOperationException("Profile doesn't have a Firebase key");
            }

            string url = $"{baseUrl}/device_profiles/{profile.FirebaseKey}.json";
            var response = await client.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }
        // Add JsonSerializerOptions at class level
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            MaxDepth = 128 // Increase if needed
        };

        public async Task<bool> SaveProfileAsync(DeviceProfile profile, bool showSuccessMessage = true)
        {
            try
            {
                // Create a clean DTO for Firebase
                var profileDto = new
                {
                    profileName = profile.ProfileName,
                    dateCreated = profile.DateCreated,
                    lastUpdated = DateTime.Now.ToString("yyyy-MM-dd"),
                    deviceName = profile.DeviceName,
                    status = profile.Status,
                    selectedProfileNum = profile.SelectedProfileNum,
                    isLinked = profile.IsLinked,
                    needsUpdate = profile.NeedsUpdate,
                    selectedProfileId = profile.SelectedProfile?.Id ?? 0
                };

                string url;
                HttpMethod method;
                string operation;

                if (string.IsNullOrEmpty(profile.FirebaseKey))
                {
                    url = $"{baseUrl}/device_profiles.json";
                    method = HttpMethod.Post;
                    operation = "created";
                }
                else
                {
                    url = $"{baseUrl}/device_profiles/{profile.FirebaseKey}.json";
                    method = HttpMethod.Put;
                    operation = "updated";
                }

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                string json = JsonSerializer.Serialize(profileDto, jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(new HttpRequestMessage(method, url)
                {
                    Content = content
                });

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    if (method == HttpMethod.Post)
                    {
                        var responseObj = JsonSerializer.Deserialize<FirebasePostResponse>(responseContent);
                        if (responseObj != null)
                        {
                            profile.FirebaseKey = responseObj.Name;
                        }
                    }

                    if (showSuccessMessage)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"Profile '{profile.ProfileName}' successfully {operation} in Firebase!",
                                          "Success",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Information);
                        });
                    }
                    return true;
                }
                else
                {
                    var errorMessage = $"Failed to save profile. Status: {response.StatusCode}";
                    if (!string.IsNullOrWhiteSpace(responseContent))
                    {
                        errorMessage += $"\nResponse: {responseContent}";
                    }
                    throw new HttpRequestException(errorMessage);
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Error saving profile: {ex.Message}",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                });
                return false;
            }
        }

        // Add this class to handle Firebase POST responses
        public class FirebasePostResponse
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }

        // Add this DTO class inside FirebaseService.cs
        public class DeviceProfileDto
        {
            public string FirebaseKey { get; set; }
            public string ProfileName { get; set; }
            public string DateCreated { get; set; }
            public string LastUpdated { get; set; }
            public string DeviceName { get; set; }
            public string Status { get; set; }
            public int SelectedProfileId { get; set; }
            public string SelectedProfileNum { get; set; }
            public bool IsLinked { get; set; }
            public bool NeedsUpdate { get; set; }
        }

        // Update LoadProfilesAsync to use the DTO
        public async Task<List<DeviceProfile>> LoadProfilesAsync(ObservableCollection<InputProfile> availableProfiles)
        {
            string url = $"{baseUrl}/device_profiles.json";

            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(json) && json != "null")
                    {
                        var profilesDict = JsonSerializer.Deserialize<Dictionary<string, DeviceProfileDto>>(json, _jsonOptions);
                        if (profilesDict != null)
                        {
                            var result = new List<DeviceProfile>();
                            foreach (var kvp in profilesDict)
                            {
                                var dto = kvp.Value;
                                var profile = new DeviceProfile
                                {
                                    FirebaseKey = kvp.Key,
                                    ProfileName = dto.ProfileName,
                                    DateCreated = dto.DateCreated,
                                    LastUpdated = dto.LastUpdated,
                                    DeviceName = dto.DeviceName,
                                    Status = dto.Status,
                                    SelectedProfileNum = dto.SelectedProfileNum,
                                    IsLinked = dto.IsLinked,
                                    NeedsUpdate = dto.NeedsUpdate,
                                    // Find the matching InputProfile from availableProfiles
                                    SelectedProfile = availableProfiles.FirstOrDefault(p => p.Id == dto.SelectedProfileId)
                                };
                                result.Add(profile);
                            }
                            return result;
                        }
                    }
                }
                return new List<DeviceProfile>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading profiles: {ex.Message}", "Error");
                return new List<DeviceProfile>();
            }
        }
    }
}
