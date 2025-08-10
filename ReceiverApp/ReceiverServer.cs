using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Web;
using FYP.ReceiverApp;

using System.Security.Cryptography.X509Certificates;
using Fleck;
using QRCoder;

namespace ReceiverApp
{
    public class ReceiverServer
    {
        private static Dictionary<IWebSocketConnection, string> activePlayers = new();
        private static Dictionary<string, string> validTokens = new(); // token -> playerId
        private static Dictionary<string, string> inputMap = new();
        private static int playerCounter = 1;

        public static event Action? OnClientPaired;
        public static event Action<string>? OnClientDisconnected;
        public static event Action<string>? OnLog;

        public static void Run()
        {
            // Load Input Mapping
            Console.WriteLine("Loading input mapping...");
            string mappingJson = File.ReadAllText("input_mapping.json");
            inputMap = JsonSerializer.Deserialize<Dictionary<string, string>>(mappingJson) ?? new Dictionary<string, string>();

            // Start WebSocket Server
            FleckLog.Level = LogLevel.Info;
            string localIp = GetLocalIPAddress();
            string serverAddress = $"ws://{localIp}:8181";
            var server = new WebSocketServer(serverAddress);
            //server.Certificate = new X509Certificate2(@"inputpoc-dev.pfx", "P@ssw0rd");

            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    var token = ExtractToken(socket.ConnectionInfo.Path);
                    if (token != null && validTokens.TryGetValue(token, out var playerId))
                    {
                        activePlayers[socket] = playerId;
                        validTokens.Remove(token); // one-time use
                        Console.WriteLine($"Paired {playerId} with token {token}");

                        // Notify WPF UI to generate a new QR code
                        OnClientPaired?.Invoke();
                    }
                    else
                    {
                        Console.WriteLine("Invalid or missing pairing token. Closing connection.");
                        socket.Close();
                    }
                };

                socket.OnClose = () =>
                {
                    if (activePlayers.Remove(socket, out var playerId))
                    {
                        Console.WriteLine($"{playerId} disconnected.");
                        OnClientDisconnected?.Invoke(playerId);
                    }
                };

                socket.OnMessage = message =>
                {
                    if (activePlayers.TryGetValue(socket, out var playerId))
                    {
                        try
                        {
                            var inputs = JsonSerializer.Deserialize<List<InputPacket>>(message) ?? new List<InputPacket>();
                            foreach (var input in inputs)
                            {
                                input.id = input.id.ToLower();

                                var line1 = $"[{playerId}] {input.id} - {input.state}";
                                Console.WriteLine(line1);
                                OnLog?.Invoke(line1);

                                if (inputMap.TryGetValue(input.id, out string? mappedAction))
                                {
                                    var line2 = $"→ Mapped to action: {mappedAction}";
                                    Console.WriteLine(line2);
                                    OnLog?.Invoke(line2);

                                    ActionExecutor.Execute(mappedAction!, input.state);
                                }
                                else
                                {
                                    var line3 = $"→ No mapping found for input: {input.id}";
                                    Console.WriteLine(line3);
                                    OnLog?.Invoke(line3);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var err = $"Error parsing input from {playerId}: {ex.Message}";
                            Console.WriteLine(err);
                            OnLog?.Invoke(err);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Unpaired connection sent input. Ignored.");
                    }
                };
            });
            Console.WriteLine($"WebSocket server started on {serverAddress}");
        }

        private static string GetLocalIPAddress()
        {
            foreach (var ip in System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address found.");
        }

        public static void AddToken(string token, string playerId)
        {
            validTokens[token] = playerId;
            Console.WriteLine($"Added token {token} for {playerId}");
        }

        private static string? ExtractToken(string path)
        {
            var parts = path.Split('?');
            if (parts.Length < 2) return null;

            var query = HttpUtility.ParseQueryString(parts[1]);
            return query["pair"];
        }

        public static string GenerateNewToken()
        {
            var token = Guid.NewGuid().ToString("N").Substring(0, 12);
            var playerId = $"Player{playerCounter++}";
            AddToken(token, playerId);
            return token;
        }

        public static IReadOnlyCollection<string> ConnectedPlayerIds =>
            activePlayers.Values.ToList();
    }
}