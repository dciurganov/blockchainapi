namespace Application.DTOs
{
    public class BlockchainDataDto
    {
        public int Id { get; set; }
        public string BlockchainType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Height { get; set; }
        public string Hash { get; set; } = string.Empty;
        public DateTime Time { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PeerCount { get; set; }
        public int UnconfirmedCount { get; set; }
    }
}
