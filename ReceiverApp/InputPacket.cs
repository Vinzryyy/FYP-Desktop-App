using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace ReceiverApp
{
    public class InputPacket
    {
        [JsonPropertyName("type")]
        public required string type { get; set; }

        [JsonPropertyName("id")]
        public required string id { get; set; }

        [JsonPropertyName("state")]
        public required string state { get; set; }

        [JsonPropertyName("timestamp")]
        public long timestamp { get; set; }
    }
}

