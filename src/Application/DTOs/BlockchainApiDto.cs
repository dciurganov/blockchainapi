using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public class BlockchainApiDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("hash")]
        public string Hash { get; set; } = string.Empty;

        [JsonPropertyName("time")]
        public DateTime Time { get; set; }

        [JsonPropertyName("latest_url")]
        public string Latest_url { get; set; } = string.Empty;

        [JsonPropertyName("previous_hash")]
        public string Previous_hash { get; set; } = string.Empty;

        [JsonPropertyName("previous_url")]
        public string Previous_url { get; set; } = string.Empty;

        [JsonPropertyName("peer_count")]
        public int Peer_count { get; set; }

        [JsonPropertyName("unconfirmed_count")]
        public int Unconfirmed_count { get; set; }

        [JsonPropertyName("high_fee_per_kb")]
        public long High_fee_per_kb { get; set; }

        [JsonPropertyName("medium_fee_per_kb")]
        public long Medium_fee_per_kb { get; set; }

        [JsonPropertyName("low_fee_per_kb")]
        public long Low_fee_per_kb { get; set; }

        [JsonPropertyName("last_fork_height")]
        public double Last_fork_height { get; set; }

        [JsonPropertyName("last_fork_hash")]
        public string Last_fork_hash { get; set; } = string.Empty;
    }
}
