using Application.DTOs;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace IntegrationTests
{
    public class BlockchainControllerTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public BlockchainControllerTests(TestWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllHistory_ShouldReturnPagedResults()
        {
            // Act
            var response = await _client.GetAsync("/api/blockchain/all-history?pageNumber=1&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<PagedResult<BlockchainDataDto>>();
            result.Should().NotBeNull();
            result!.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(10);
        }

        [Fact]
        public async Task HealthCheck_ShouldReturnHealthy()
        {
            // Act
            var response = await _client.GetAsync("/health");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
