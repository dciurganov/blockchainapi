using Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace FunctionalTests
{
    public class BlockchainApiE2ETests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public BlockchainApiE2ETests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CompleteWorkflow_SyncAndRetrieve_ShouldWork()
        {
            // Act 1: Sync blockchain data
            var syncResponse = await _client.PostAsync(
                "/api/blockchain/sync/BTC",
                null);

            // Assert 1: Sync should succeed
            syncResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var syncResult = await syncResponse.Content.ReadFromJsonAsync<object>();
            syncResult.Should().NotBeNull();

            // Act 2: Retrieve history
            var historyResponse = await _client.GetAsync(
                "/api/blockchain/BTC/history?pageNumber=1&pageSize=10");

            // Assert 2: History should be retrievable
            historyResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var historyResult = await historyResponse.Content
                .ReadFromJsonAsync<PagedResult<BlockchainDataDto>>();

            historyResult.Should().NotBeNull();
            historyResult!.Items.Should().NotBeEmpty();
            historyResult.Items.First().BlockchainType.Should().Be("BTC");
        }

        [Fact]
        public async Task SyncAllBlockchains_ShouldSyncMultipleChains()
        {
            // Act: Sync all blockchains
            var response = await _client.PostAsync("/api/blockchain/sync-all", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<object>();
            result.Should().NotBeNull();

            // Verify all blockchains were synced
            var allHistoryResponse = await _client.GetAsync(
                "/api/blockchain/all-history?pageNumber=1&pageSize=100");

            var allHistory = await allHistoryResponse.Content
                .ReadFromJsonAsync<PagedResult<BlockchainDataDto>>();

            allHistory.Should().NotBeNull();
            allHistory!.Items.Should().NotBeEmpty();

            // Should have multiple blockchain types
            var types = allHistory.Items.Select(x => x.BlockchainType).Distinct();
            types.Should().HaveCountGreaterThan(1);
        }

        [Fact]
        public async Task GetHistory_WithPagination_ShouldReturnCorrectPage()
        {
            // Arrange: Sync some data first
            await _client.PostAsync("/api/blockchain/sync/ETH", null);
            await _client.PostAsync("/api/blockchain/sync/ETH", null);
            await _client.PostAsync("/api/blockchain/sync/ETH", null);

            // Act: Get first page
            var page1Response = await _client.GetAsync(
                "/api/blockchain/ETH/history?pageNumber=1&pageSize=2");

            var page1 = await page1Response.Content
                .ReadFromJsonAsync<PagedResult<BlockchainDataDto>>();

            // Assert
            page1.Should().NotBeNull();
            page1!.PageNumber.Should().Be(1);
            page1.PageSize.Should().Be(2);
            page1.Items.Should().HaveCountLessThanOrEqualTo(2);
            page1.HasNextPage.Should().BeTrue();
        }

        [Fact]
        public async Task GetHistory_OrderedByCreatedAtDescending()
        {
            // Arrange: Sync multiple times
            await _client.PostAsync("/api/blockchain/sync/LTC", null);
            await Task.Delay(100); // Small delay to ensure different timestamps
            await _client.PostAsync("/api/blockchain/sync/LTC", null);
            await Task.Delay(100);
            await _client.PostAsync("/api/blockchain/sync/LTC", null);

            // Act
            var response = await _client.GetAsync(
                "/api/blockchain/LTC/history?pageNumber=1&pageSize=10");

            var result = await response.Content
                .ReadFromJsonAsync<PagedResult<BlockchainDataDto>>();

            // Assert: Should be ordered by CreatedAt descending
            result.Should().NotBeNull();
            result!.Items.Should().HaveCountGreaterThanOrEqualTo(2);

            var timestamps = result.Items.Select(x => x.CreatedAt).ToList();
            timestamps.Should().BeInDescendingOrder();
        }

        [Theory]
        [InlineData("ETH")]
        [InlineData("BTC")]
        [InlineData("DASH")]
        [InlineData("LTC")]
        public async Task SyncBlockchain_ForAllTypes_ShouldSucceed(string blockchainType)
        {
            // Act
            var response = await _client.PostAsync(
                $"/api/blockchain/sync/{blockchainType}",
                null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<object>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task InvalidBlockchainType_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.PostAsync(
                "/api/blockchain/sync/INVALID",
                null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
