using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;

using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.HttpClients
{
    public class BlockCypherClient : IBlockCypherClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BlockCypherClient> _logger;

        public BlockCypherClient(HttpClient httpClient, ILogger<BlockCypherClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<BlockchainApiDto> GetBlockchainDataAsync(
            BlockchainType blockchainType,
            CancellationToken cancellationToken = default)
        {
            var endpoint = GetEndpoint(blockchainType);

            _logger.LogInformation("Fetching data from BlockCypher API: {Endpoint}", endpoint);

            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var data = JsonSerializer.Deserialize<BlockchainApiDto>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (data == null)
            {
                throw new InvalidOperationException($"Failed to deserialize response from {endpoint}");
            }

            return data;
        }

        private static string GetEndpoint(BlockchainType blockchainType)
        {
            return blockchainType switch
            {
                BlockchainType.ETH => "eth/main",
                BlockchainType.BTC => "btc/main",
                BlockchainType.DASH => "dash/main",
                BlockchainType.LTC => "ltc/main",
                BlockchainType.BTC_TEST3 => "btc/test3",
                _ => throw new ArgumentException($"Unknown blockchain type: {blockchainType}")
            };
        }
    }
}
