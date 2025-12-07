namespace Domain.Entities
{
    public class BlockchainData
    {
        public int Id { get; set; }
        public string BlockchainType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Height { get; set; }
        public string Hash { get; set; } = string.Empty;
        public DateTime Time { get; set; }
        public string LatestUrl { get; set; } = string.Empty;
        public string PreviousHash { get; set; } = string.Empty;
        public string PreviousUrl { get; set; } = string.Empty;
        public int PeerCount { get; set; }
        public int UnconfirmedCount { get; set; }
        public long HighFeePerKb { get; set; }
        public long MediumFeePerKb { get; set; }
        public long LowFeePerKb { get; set; }
        public double LastForkHeight { get; set; }
        public string LastForkHash { get; set; } = string.Empty;

        // Additional timestamp as per requirements
        public DateTime CreatedAt { get; set; }

        // Store full JSON response
        public string RawJsonData { get; set; } = string.Empty;
    }
}
