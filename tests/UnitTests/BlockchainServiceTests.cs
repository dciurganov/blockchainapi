using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests
{
    public class BlockchainServiceTests
    {
        private readonly Mock<IBlockCypherClient> _mockClient;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ILogger<BlockchainService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IRepository<BlockchainData>> _mockRepository;
        private readonly BlockchainService _sut;

        public BlockchainServiceTests()
        {
            _mockClient = new Mock<IBlockCypherClient>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<BlockchainService>>();
            _mockMapper = new Mock<IMapper>();
            _mockRepository = new Mock<IRepository<BlockchainData>>();

            _mockUnitOfWork
                .Setup(x => x.Repository<BlockchainData>())
                .Returns(_mockRepository.Object);

            _sut = new BlockchainService(
                _mockClient.Object,
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task SyncBlockchainDataAsync_ShouldStoreData_WhenApiReturnsValidData()
        {
            // Arrange
            var apiResponse = new BlockchainApiDto
            {
                Name = "BTC.main",
                Height = 800000,
                Hash = "00000000000000000001234567890abcdef",
                Time = DateTime.UtcNow,
                Latest_url = "https://api.blockcypher.com/v1/btc/main/blocks/800000",
                Previous_hash = "previous_hash",
                Previous_url = "previous_url",
                Peer_count = 50,
                Unconfirmed_count = 1000,
                High_fee_per_kb = 50000,
                Medium_fee_per_kb = 30000,
                Low_fee_per_kb = 10000,
                Last_fork_height = 799999,
                Last_fork_hash = "fork_hash"
            };

            _mockClient
                .Setup(x => x.GetBlockchainDataAsync(BlockchainType.BTC, default))
                .ReturnsAsync(apiResponse);
            _mockMapper
                .Setup(x => x.Map<BlockchainDataDto>(It.IsAny<BlockchainData>()))
                .Returns(new BlockchainDataDto
                {
                    BlockchainType = "BTC",
                    Height = 800000
                });

            // Act
            var result = await _sut.SyncBlockchainDataAsync(BlockchainType.BTC);

            // Assert
            result.Should().NotBeNull();
            result.BlockchainType.Should().Be("BTC");
            result.Height.Should().Be(800000);

            _mockRepository.Verify(
                x => x.AddAsync(It.IsAny<BlockchainData>(), default),
                Times.Once);
            _mockUnitOfWork.Verify(
                x => x.CommitAsync(default),
                Times.Once);
        }

        [Theory]
        [InlineData(BlockchainType.ETH)]
        [InlineData(BlockchainType.BTC)]
        [InlineData(BlockchainType.DASH)]
        [InlineData(BlockchainType.LTC)]
        public async Task SyncBlockchainDataAsync_ShouldHandleAllBlockchainTypes(BlockchainType type)
        {
            // Arrange
            var apiResponse = new BlockchainApiDto
            {
                Name = $"{type}.main",
                Height = 100000,
                Hash = "test_hash",
                Time = DateTime.UtcNow,
                Latest_url = "test_url",
                Previous_hash = "prev_hash",
                Previous_url = "prev_url"
            };

            _mockClient
                .Setup(x => x.GetBlockchainDataAsync(type, default))
                .ReturnsAsync(apiResponse);
            _mockMapper
                .Setup(x => x.Map<BlockchainDataDto>(It.IsAny<BlockchainData>()))
                .Returns(new BlockchainDataDto
                {
                    BlockchainType = type.ToString(),
                });

            // Act
            var result = await _sut.SyncBlockchainDataAsync(type);

            // Assert
            result.BlockchainType.Should().Be(type.ToString());
        }
    }
}
