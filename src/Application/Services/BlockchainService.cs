using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Application.Services
{
    public class BlockchainService : IBlockchainService
    {
        private readonly IBlockCypherClient _blockCypherClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BlockchainService> _logger;
        private readonly IMapper _mapper;

        public BlockchainService(
            IBlockCypherClient blockCypherClient,
            IUnitOfWork unitOfWork,
            ILogger<BlockchainService> logger,
            IMapper mapper
            )
        {
            _blockCypherClient = blockCypherClient;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<BlockchainDataDto> SyncBlockchainDataAsync(
            BlockchainType blockchainType,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var apiData = await _blockCypherClient.GetBlockchainDataAsync(
                    blockchainType,
                    cancellationToken);

                var entity = new BlockchainData
                {
                    BlockchainType = blockchainType.ToString(),
                    Name = apiData.Name,
                    Height = apiData.Height,
                    Hash = apiData.Hash,
                    Time = apiData.Time,
                    LatestUrl = apiData.Latest_url,
                    PreviousHash = apiData.Previous_hash,
                    PreviousUrl = apiData.Previous_url,
                    PeerCount = apiData.Peer_count,
                    UnconfirmedCount = apiData.Unconfirmed_count,
                    HighFeePerKb = apiData.High_fee_per_kb,
                    MediumFeePerKb = apiData.Medium_fee_per_kb,
                    LowFeePerKb = apiData.Low_fee_per_kb,
                    LastForkHeight = apiData.Last_fork_height,
                    LastForkHash = apiData.Last_fork_hash,
                    CreatedAt = DateTime.UtcNow,
                    RawJsonData = JsonSerializer.Serialize(apiData)
                };

                var repository = _unitOfWork.Repository<BlockchainData>();
                await repository.AddAsync(entity, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation(
                    "Successfully synced {BlockchainType} at height {Height}",
                    blockchainType,
                    entity.Height);

                return _mapper.Map<BlockchainDataDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing {BlockchainType}", blockchainType);
                throw;
            }
        }
    }
}
